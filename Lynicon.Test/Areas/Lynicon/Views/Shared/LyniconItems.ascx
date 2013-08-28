<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Lynicon.Caching" %>
<%
    var cache = ContentItemCache.Instance;
     %>
<% foreach (var typeKvp in cache.Cache) { %>
    <h2><%= typeKvp.Key.Name %></h2>
    <a>
        <% foreach (var val in typeKvp.Value.Items) { %>
            <a href="/<%= val.Url%>" title="<%= val.Id %>"><%=val.Url %></a>
        <% } %>
    </div>
<% } %>
