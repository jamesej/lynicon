<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Models.Link>" %>
<%@ Import Namespace="Lynicon.Models" %>
<% if (Model != null) { %>
    <div class='l24-link'>
        <div class='l24-link-url'>
            <span>Url</span>
            <input type="text" name="<%= ViewData.TemplateInfo.HtmlFieldPrefix %>.Url" value="<%= Model.Url %>" />
        </div>
        <div class='l24-link-content'>
            <span>Content</span>
            <input type="text" class="bb-text" name="<%= ViewData.TemplateInfo.HtmlFieldPrefix %>.Content" value="<%= (Model.Content ?? BbText.Empty).Text %>" />
        </div>
    </div>
<% } %>

