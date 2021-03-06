﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;


namespace Jhu.Graywulf.Web.UI.Controls
{
    [DefaultProperty("Text"), ParseChildren(false), PersistChildren(true), ToolboxData("<{0}:CodeView runat=server></{0}:CodeView>"), ControlValueProperty("Text")]
    public class Query : System.Web.UI.WebControls.WebControl, ITextControl
    {
        private LinkButton edit;
        private bool textSetByAddParsedSubObject;

        [Bindable(true), Localizable(true), PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public string Text
        {
            get { return (string)ViewState["Text"] ?? ""; }
            set { ViewState["Text"] = value; }
        }

        protected override void AddParsedSubObject(object obj)
        {
            if (this.HasControls())
            {
                base.AddParsedSubObject(obj);
            }
            else if (obj is LiteralControl)
            {
                if (this.textSetByAddParsedSubObject)
                {
                    this.Text = this.Text + ((LiteralControl)obj).Text;
                }
                else
                {
                    this.Text = ((LiteralControl)obj).Text;
                }
                this.textSetByAddParsedSubObject = true;
            }
            else
            {
                string text = this.Text;
                if (text.Length != 0)
                {
                    this.Text = string.Empty;
                    base.AddParsedSubObject(new LiteralControl(text));
                }
                base.AddParsedSubObject(obj);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            edit = new LinkButton();
            edit.Text = "Try this query";
            edit.Click += new EventHandler(edit_Click);

            this.Controls.Add(edit);

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "xregexp", VirtualPathUtility.ToAbsolute("~/SyntaxHighlighter/scripts/XRegExp.js"));
            ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "core", VirtualPathUtility.ToAbsolute("~/SyntaxHighlighter/scripts/shCore.js"));
            ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "autoloader", VirtualPathUtility.ToAbsolute("~/SyntaxHighlighter/scripts/shAutoLoader.js"));
            ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "brushsql", VirtualPathUtility.ToAbsolute("~/SyntaxHighlighter/scripts/shBrushSql.js"));

            ScriptManager.RegisterStartupScript(this, this.GetType(), "all", "SyntaxHighlighter.all();", true);

            /*HtmlLink css1 = new HtmlLink();
            css1.Href = VirtualPathUtility.ToAbsolute("~/SyntaxHighlighter/styles/shCore.css");
            css1.Attributes["rel"] = "stylesheet";
            css1.Attributes["type"] = "text/css";
            css1.Attributes["media"] = "all";
            Page.Header.Controls.Add(css1);*/

            base.OnPreRender(e);
        }

        void edit_Click(object sender, EventArgs e)
        {
            Util.QueryEditorUtil.SetQueryInSession(this.Page, Text, null, true);
            Page.Response.Redirect(Jhu.Graywulf.Web.UI.Query.Default.GetUrl());
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            writer.Write(String.Format("<script type=\"syntaxhighlighter\" class=\"brush: {0};\"><![CDATA[\r\n", "sql"));
            writer.Write(Text);
            writer.Write("]]></script>");

            edit.RenderControl(writer);
        }
    }
}
