<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="container">
<%= Html.Editor(ViewData["propertyPath"] as string) %>
</div>

