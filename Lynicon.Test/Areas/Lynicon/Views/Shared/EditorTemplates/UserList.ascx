<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.List<Lynicon.Membership.User>>" %>

<table class="list-table user-list-table">
    <tr>
    <% foreach (var fld in (List<string>)ViewData["displayFields"]) { %>
        <th><%= Html.Label(fld) %></th>
    <% } %>
    </tr>
    <% int i = 0;  foreach (var u in Model)
       { %>
    <tr id="u-<%= i++ %>" class="<%= i == 1 ? "selected" : "" %>">
        <% foreach (var fld in (List<string>)ViewData["displayFields"])
           { %>
            <td><%= u.GetType().GetProperty(fld).GetValue(u)%></td>
        <% } %>
    </tr>
    <% } %>
</table>
<%= Html.Partial("FilterPanel", new Dictionary<string, List<string>> { { "Filter", new List<string> { "UserName", "Email" } } }) %>
