﻿<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.ClusterDetails"
    MasterPageFile="~/EntityChildren.master" CodeBehind="ClusterDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server">
        <jgwac:EntityList runat="server" ID="DomainList" ChildrenType="Domain" EntityGroup="Federation" />
        <jgwac:EntityList runat="server" ID="DatabaseDefinitionList" ChildrenType="DatabaseDefinition"
            Text="Cluster Database Definitions" EntityGroup="Federation">
            <columns>
                <asp:BoundField DataField="DeploymentState" HeaderText="Deployment State" />
                <asp:BoundField DataField="RunningState" HeaderText="Running State" />
                <asp:BoundField DataField="AlertState" HeaderText="Alert State" />
            </columns>
        </jgwac:EntityList>
    </jgwac:EntityChildren>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
