<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Models.Summary>" %>
<div class="lyn-item-entry">
    <% if (ViewData.ContainsKey("UrlPermission") && (bool)ViewData["UrlPermission"]) { %>
    <a class="move-link cmd-link" href="/<%= Model.Url %>?$urlget=true" title="Move Url">Mv</a>
    <a class="del-link cmd-link" title="Delete Url">Del</a>
    <% } %>
    <a class="item-link" href="/<%= Model.Url%>" title="<%= Model.Id %>"><%= Model.DisplayTitle() %></a>
</div>


