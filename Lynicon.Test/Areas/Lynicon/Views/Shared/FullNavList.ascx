<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Map.NavTreeSummary>" %>
<a class="nav-link" href="/<%= Model.Url %>"><%= Model.Title %></a>
<% if (Model.Children != null && Model.Children.Count > 0) { %>
<ul>
<% foreach (var child in Model.Children) { %>
    <li>
        <%= Html.Partial("~/Areas/Lynicon/Views/Shared/FullNavList.ascx", child) %>
    </li>
<% } %>
</ul>
<% } %>

