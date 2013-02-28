<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Models.Link>" %>
<%@ Import Namespace="Lynicon.Models" %>
<% if (Model != null) { %>
    <div class='l24-link'>
        <div class='l24-link-isinternal'>
            <input type="checkbox" name="<%= ViewData.TemplateInfo.HtmlFieldPrefix %>.IsInternal" <%= Model.IsInternal ? "checked" : "" %> />
            <span>Internal</span>
        </div>
        <div class='l24-link-controller'>
            <span>Controller</span>
            <input type="text" name="<%= ViewData.TemplateInfo.HtmlFieldPrefix %>.Controller" value="<%= Model.Controller %>" />
        </div>
        <div class='l24-link-action'>
            <span>Action</span>
            <input type="text" name="<%= ViewData.TemplateInfo.HtmlFieldPrefix %>.Action" value="<%= Model.Action %>" />
        </div>
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

