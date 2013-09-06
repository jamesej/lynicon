<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Lynicon.Map" %>
<%
    var map = ContentMap.Instance;
     %>
<% foreach (var typeKvp in map.Cache) { %>
    <h2><%= typeKvp.Key.Name %></h2>
    <div>
        <% foreach (var val in typeKvp.Value.Items) { %>
            <a href="/<%= val.Url%>" title="<%= val.Id %>"><%=val.Url %></a>
        <% } %>
    </div>
<% } %>
