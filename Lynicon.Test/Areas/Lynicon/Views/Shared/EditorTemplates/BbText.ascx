<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Models.BbText>" %>
<%@ Import Namespace="Lynicon.Models" %>
<%= Html.TextBox("", (ViewData.Model ?? BbText.Empty).Text,
    new { @class = "text-box single-line bb-text " + ViewData["classes"] })  %>

