﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Web
{
    public class ApplicationBase : System.Web.HttpApplication
    {
        protected virtual void Application_Start(object sender, EventArgs e)
        {
            HostingEnvironment.RegisterVirtualPathProvider(new EmbeddedVirtualPathProvider());
        }

        protected virtual void Session_Start(object sender, EventArgs e)
        {
            Session[Constants.SessionContextGuid] = Guid.NewGuid();
            Session[Constants.SessionUsername] = null;
        }

        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected virtual void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected virtual void Application_Error(object sender, EventArgs e)
        {

        }

        protected virtual void Session_End(object sender, EventArgs e)
        {

        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
            
        }
    }
}
