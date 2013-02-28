<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Models.ImageLink>" %>
<% if (Model != null) { %>
<div class="l24-imagelink">
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
            <input type="text" name="<%= ViewData.TemplateInfo.HtmlFieldPrefix %>.Content" value="<%= Model.Content %>" />
        </div>
    </div>
    <div class="l24-image">
        <div class="l24-image-content">
            <% if (string.IsNullOrEmpty(Model.Image.Url))
                { %>
            no image
            <% }
                else
                { %>
            <img class='file-image-thumb' src="<%= Model.Image.Url %>" />
            <% } %>
        </div>
        <div class="l24-image-url">
            <button class='l24-image-load'>Find File</button>
            <span>Url</span>
            <input type="text" class="l24-file-url" name="<%= ViewData.TemplateInfo.HtmlFieldPrefix %>.Image.Url" value="<%= Model.Image.Url %>" />
        </div>
        <div class='l24-image-alt'>
            <span>Alt</span>
            <%= Html.EditorFor(m => m.Image.Alt) %>
        </div>
    </div>
    <div style="clear:both"></div>
</div>
<div style="clear:both"></div>
<% } %>

