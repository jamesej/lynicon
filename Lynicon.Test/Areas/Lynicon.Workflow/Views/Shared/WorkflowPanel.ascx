<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Lynicon.Workflow.Models" %>
<%@ Import Namespace="Lynicon.Membership" %>
<%
    var lmi = LayerManager.Instance;
    var wfu = (WorkflowUser)LyniconSecurityManager.Current.User;
    %>
<select id="wf-layer-select">
    <% foreach (var layer in lmi.GetUserLayers(wfu.Id)) { %>
        <option value="<%= layer.Layer %>"><%= layer.Name %></option>
    <% } %>
</select>
<input type="text" id="wf-layer-name" />
<a class="func-button" href="/Lynicon/Items" target="_blank">Get Layer</a>


