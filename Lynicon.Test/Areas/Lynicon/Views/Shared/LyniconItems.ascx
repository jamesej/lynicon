<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Lynicon.Map" %>
<%
    var map = ContentMap.Instance;
     %>
<div id='lyn-item-selector' <%= ViewData.ContainsKey("popup") ? "style='display:none'" : "" %>>
    <div id="lyn-item-selector-inner">
    <% foreach (var typeKvp in map.Cache) { %>
        <h2><%= typeKvp.Key.Name %><%= Html.Hidden("typeName", typeKvp.Key.FullName) %></h2>
        <div class="lyn-type-items">
            <% foreach (var val in typeKvp.Value.Items) { %>
                <a class="item-link" href="/<%= val.Url%>" title="<%= val.Id %>">
                    <% if (string.IsNullOrEmpty(val.Title)) { %>
                        /<%= val.Url %>
                    <% } else { %>
                        <%= val.Title %>
                    <% } %>
                </a>
            <% } %>
        </div>
    <% } %>
        <input type="hidden" id="lyn-item-selected" />
    </div>
    <div style="clear:both"></div>
</div>
<script type="text/javascript">
    $('#lyn-item-selector-inner').accordion({ heightStyle: 'content' });
</script>
