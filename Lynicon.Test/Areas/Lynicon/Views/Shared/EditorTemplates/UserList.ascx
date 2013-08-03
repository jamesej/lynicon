<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.List<Lynicon.Membership.User>>" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Lynicon.Membership" %>
<%@ Import Namespace="Lynicon.Models" %>
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
<% if (ViewBag.CanAdd) { %>
<div id="lynicon-list-add-button">ADD</div>
<% } %>
<% if (ViewContext.RouteData.DataTokens.ContainsKey("@Paging")) { %>
<div><%= Html.Partial("PagingSpec", this.ViewContext.RouteData.DataTokens["@Paging"]) %></div>
<% } %>
<script type="text/javascript">
    $('.list-table').on('click', 'tr', function () {
        var idx = $(this).prop('id').after('u-');
        loadDetail(parseInt(idx));
        $('#lynicon_itemIndex').val(idx);
        $(this).closest('.list-table').find('.selected').removeClass('selected');
        $(this).addClass('selected');
    });
    $('#lynicon-list-add-button').on('click', function () {
        $('#lynicon_itemIndex').val('-1');
        loadDetail(-1);
        $('.list-table').find('.selected').removeClass('selected');
    });
</script>
