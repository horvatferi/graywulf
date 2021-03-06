﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Admin;

namespace Jhu.Graywulf.Web.Admin.Security
{
    public partial class DomainDetails : EntityDetailsPageBase<Registry.Domain>
    {
        protected override void InitLists()
        {
            base.InitLists();

            UserList.ParentEntity = item;
            UserGroupList.ParentEntity = item;
        }

        /*
        protected void UserList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/security/UserDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void UserGroupList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/security/UserGroupDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void AddUser_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.User));
        }

        protected void AddUserGroup_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.UserGroup));
        }
         * */
    }
}