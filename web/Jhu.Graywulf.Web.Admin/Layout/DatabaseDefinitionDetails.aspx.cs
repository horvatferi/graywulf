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

namespace Jhu.Graywulf.Web.Admin.Layout
{
    public partial class DatabaseDefinitionDetails : EntityDetailsPageBase<DatabaseDefinition>
    {
        protected override void InitLists()
        {
            base.InitLists();

            DatabaseInstanceList.ParentEntity = item;
        }

        public override void OnButtonCommand(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Map":
                    Response.Redirect(MappingWizard.GetUrl(item.Guid));
                    break;
                default:
                    base.OnButtonCommand(sender, e);
                    break;
            }
        }

    }
}