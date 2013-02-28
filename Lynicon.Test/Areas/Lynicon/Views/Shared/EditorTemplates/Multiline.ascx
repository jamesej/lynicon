<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Lynicon.Models" %>
<%= Html.TextArea("",
    typeof(BbText).IsAssignableFrom(ViewData.ModelMetadata.ModelType)
        ? ((Model as BbText) ?? BbText.Empty).Text
        : ViewData.TemplateInfo.FormattedModelValue.ToString(),
    ViewData["rows"] != null ? (int)ViewData["rows"] : 4,
    ViewData["cols"] != null ? (int)ViewData["cols"] : 25,
    new { @class = ViewData["classes"] })  %>

