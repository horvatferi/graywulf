﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Jhu.Graywulf.RemoteService
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [NetDataContract]
    public interface IRemoteService : Jhu.Graywulf.Tasks.ICancelableTask
    {
        [OperationContract]
        void Execute();

        [OperationContract]
        void BeginExecute();

        [OperationContract]
        void EndExecute();
    }
}
