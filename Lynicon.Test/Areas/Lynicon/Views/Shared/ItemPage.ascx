<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Models.Summary[]>" %>
<%@ Import Namespace="Lynicon.Utility" %>
<%
    Uri currentUri = ViewContext.HttpContext.Request.Url;
     %>
<% foreach (var summ in Model) { %>
<div class="lyn-item-entry">
    <% if (ViewData.ContainsKey("UrlPermission") && (bool)ViewData["UrlPermission"]) { %>
    <a class="move-link cmd-link" href="/<%= summ.Url %>?$urlget=true" title="Move Url">Mv</a>
    <a class="del-link cmd-link" title="Delete Url">Del</a>
    <% } %>
    <a class="item-link" href="/<%= summ.Url%>" title="<%= summ.Id %>"><%= summ.DisplayTitle() %></a>
</div>
<% } %>
<% if (ViewContext.RouteData.DataTokens.ContainsKey("@Paging")) { %>
<div><%= Html.Partial("PagingSpec", this.ViewContext.RouteData.DataTokens["@Paging"]) %></div>
<% } %>
<%= Html.Partial("FilterPanel", new Dictionary<string, List<string>> { { "Title", new List<string> { "Title" } } }) %>
<%= Html.RegisterScript("override-filter-action", @"javascript:
function navigate($button, url) {
    var $panel = $button.closest('.lyn-type-items');
    $panel.load(url);
}
function thisUrl() {
    return """ + currentUri.OriginalString + @"""
}", new List<string> { "lynicon-filter-textbox" }) %>


