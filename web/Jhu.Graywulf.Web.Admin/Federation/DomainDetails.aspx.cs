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

namespace Jhu.Graywulf.Web.Admin.Federation
{
    public partial class DomainDetails : EntityDetailsPageBase<Registry.Domain>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            ShortTitle.Text = item.ShortTitle;
            LongTitle.Text = item.LongTitle;
            Email.Text = item.Email;
            StandardUserGroup.EntityReference.Value = item.StandardUserGroup;
        }

        protected override void InitLists()
        {
            base.InitLists();

            FederationList.ParentEntity = item;
        }

        /*
        protected void FederationList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/federation/FederationDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void AddFederation_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.Federation));
        }
         * */
    }
}