﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.IO
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults=true)]
    public class FileCopy : RemoteServiceBase, IFileCopy
    {
        private string source;
        private string destination;
        private bool overwrite;

        public string Source
        {
            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            get { return source; }

            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            set { source = value; }
        }

        public string Destination
        {
            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            get { return destination; }

            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            set { destination = value; }
        }

        public bool Overwrite
        {
            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            get { return overwrite; }

            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            set { overwrite = value; }
        }

        #region Constructors and initializers

        public FileCopy()
        {
            InitializeMembers();
        }

        public FileCopy(string source, string destination, bool overwrite)
        {
            InitializeMembers();

            this.source = source;
            this.destination = destination;
            this.overwrite = overwrite;
        }

        private void InitializeMembers()
        {
            this.source = null;
            this.destination = null;
            this.overwrite = false;
        }

        #endregion

        protected override void OnExecute()
        {
            // Check if file can be overwritten
            if (File.Exists(Destination))
            {
                if (!Overwrite)
                {
                    throw new IOException(ExceptionMessages.FileAlreadyExists);
                }
                else
                {
                    File.Delete(Destination);
                }
            }

            // Create destination folder
            if (!String.IsNullOrEmpty(Path.GetDirectoryName(destination)) && !Directory.Exists(Path.GetDirectoryName(destination)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
            }

            // Figure out the working directory from the service's exe
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Execute eseutil to perform copy
            var info = new ProcessStartInfo(
                Path.Combine(path, "eseutil.exe"),
                String.Format("/y \"{0}\" /d \"{1}\"", source, destination));

            // These are important to run program under the delegated account
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            var guid = Guid.NewGuid();
            var cproc = new CancelableProcess(info);
            RegisterCancelable(guid, cproc);

            cproc.Execute();

            UnregisterCancelable(guid);

            if (cproc.IsCanceled || cproc.ExitCode == -1073741510)
            {
                throw new OperationCanceledException(ExceptionMessages.FileCopyCanceled);
            }
            else if (cproc.ExitCode > 0)
            {
                throw new Exception(String.Format(ExceptionMessages.FileCopyFailed, cproc.ExitCode));
            }

            // rename destination file if file names differ
            // **** TODO: test this
            /* delete
            if (StringComparer.InvariantCultureIgnoreCase.Compare(Path.GetDirectoryName(source), Path.GetDirectoryName(destination)) != 0 &&
                StringComparer.InvariantCultureIgnoreCase.Compare(Path.GetFileName(source), Path.GetFileName(destination)) != 0)
            {
                string from = Path.Combine(Path.GetDirectoryName(destination), Path.GetFileName(source));
                Console.WriteLine("{0} -> {1}", from, source);

                File.Move(from, destination);
            }*/
        }
    }
}
