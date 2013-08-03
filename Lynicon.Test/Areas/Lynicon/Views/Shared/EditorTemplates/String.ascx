<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<object>" %>
<%@ Import Namespace="Lynicon.Models" %>
<%= Html.TextBox("", ViewData.Model,
ViewData.ModelMetadata.IsReadOnly ? 
(object)new { @class = "text-box single-line " + ViewData["classes"], @readonly = true } :
(object)new { @class = "text-box single-line " + ViewData["classes"]})  %>

