<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Models.PagingSpec>" %>
<%
    Uri currentUri = ViewContext.HttpContext.Request.Url;
     %>
<% if (Model.NeedsPager) { %>
<div class="lynicon-number-pager">
    <% foreach (int p in Model.PageRange(6)) { %>
        <span class="<%= Model.Page == p ? "current" : "" %>">
            <a href="<%= Model.GetUrl(currentUri, p) %>">
                <%= Model.IsSpacerPage(6, p) ? "..." : (p + 1).ToString() %>
            </a>
        </span>
    <% } %>
</div>
<% } %>

