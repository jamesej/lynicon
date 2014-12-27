<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.IEnumerable>" %>

<table class="list-table object-list-table">
    <tr>
    <% foreach (var fld in (List<string>)ViewBag.ListFields) { %>
        <th><%= Html.Label(fld) %></th>
    <% } %>
    </tr>
    <% int i = 0;  foreach (object o in Model)
       { %>
    <tr id="o-<%= i++ %>" class="<%= i == 1 ? "selected" : "" %>">
        <% foreach (var fld in (List<string>)ViewBag.ListFields)
           { %>
            <td><%= o.GetType().GetProperty(fld).GetValue(o)%></td>
        <% } %>
    </tr>
    <% } %>
</table>
