<%@ Assembly Name="SharePointLiveCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6448cdf074042079" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SharePointLiveCode.aspx.cs" Inherits="SharePointLiveCode.SharePointLiveCode" %>

<html>
<head>
	<asp:PlaceHolder ID="PlaceHolderHeader" runat="server"></asp:PlaceHolder>
</head>
<body>
	<asp:PlaceHolder ID="PlaceHolderContent" runat="server"></asp:PlaceHolder>
</body>
</html>