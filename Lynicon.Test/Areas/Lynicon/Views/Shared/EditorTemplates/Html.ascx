<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<% if (!ViewData.ModelMetadata.HideSurroundingHtml)
   { %>
<div class='_L24Html'>
<%= string.IsNullOrEmpty(ViewData.TemplateInfo.FormattedModelValue as string)
    ? "&lt;Empty&gt;"
        : ViewData.TemplateInfo.FormattedModelValue %>
</div>
<% } %>
<%= Html.Hidden("", ViewData.TemplateInfo.FormattedModelValue) %>

