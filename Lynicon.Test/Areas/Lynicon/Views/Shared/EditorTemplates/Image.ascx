<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Models.Image>" %>
<div class="l24-image">
    <div class="l24-image-content">
        <% if (string.IsNullOrEmpty(Model.Url))
            { %>
        no image
        <% }
            else
            { %>
        <img class='file-image-thumb' src="<%= Model.Url %>" />
        <% } %>
    </div>
    <div class="l24-image-url">
        <button class='l24-image-load'>Find File</button>
        <span>Url</span>
        <input type="text" class="l24-file-url" name="<%= ViewData.TemplateInfo.HtmlFieldPrefix %>.Url" value="<%= Model.Url %>" />
    </div>
    <div class='l24-image-alt'>
        <span>Alt</span>
        <%= Html.EditorFor(m => m.Alt) %>
    </div>
</div>
<%= Html.Partial("EnsureFileEditor") %>

