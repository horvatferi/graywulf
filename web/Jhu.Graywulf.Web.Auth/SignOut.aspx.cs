﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace Jhu.Graywulf.Web.Auth
{
    public partial class SignOut : PageBase
    {
        public static string GetUrl(string returnUrl)
        {
            return String.Format("~/SignOut.aspx?ReturnUrl={0}", returnUrl);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session[Constants.SessionUsername] = null;
            Session[Constants.SessionContextGuid] = null;
            Session.Abandon();

            ShortTitle.Text = (string)Application[Jhu.Graywulf.Web.Constants.ApplicationShortTitle];
            Ok.Attributes.Add("onClick", Util.UrlFormatter.GetClientRedirect(ReturnUrl));
        }
    }
}