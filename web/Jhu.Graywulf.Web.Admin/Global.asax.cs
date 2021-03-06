﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Data.SqlClient;
using Jhu.Graywulf;

namespace Jhu.Graywulf.Web.Admin
{
    public class Global : ApplicationBase
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);

            Application[Web.Constants.ApplicationShortTitle] = "Graywulf admin";
            Application[Web.Constants.ApplicatonLongTitle] = "Graywulf admin interface";

            Components.AppDomainManager.Instance.BaseDirectory = Activities.AppSettings.WorkflowAssemblyPath;
        }

        protected override void Session_Start(object sender, EventArgs e)
        {
            base.Session_Start(sender, e);

            var csb = new SqlConnectionStringBuilder(Registry.AppSettings.ConnectionString);
            Session[Web.Constants.SessionDatabase] = String.Format("{0}\\{1}", csb.DataSource, csb.InitialCatalog);
        }
    }
}