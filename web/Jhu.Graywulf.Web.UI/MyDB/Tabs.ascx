﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tabs.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.MyDB.Tabs" %>
<jgwc:TabHeader ID="TabHeader" runat="server">
    <tabs>
    <jgwc:Tab runat="server" Text="Summary" ID="Summary" />
    <jgwc:Tab runat="server" Text="Tables" ID="Tables" />
    <jgwc:Tab runat="server" Text="Import" ID="Import" />
    <jgwc:Tab runat="server" Text="Export" ID="Export" />
    <jgwc:Tab runat="server" Text="Download" ID="Download" />
    </tabs>
</jgwc:TabHeader>
