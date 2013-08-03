<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<script runat="server">
    bool ShouldShow(ModelMetadata metadata) {
        return metadata.ShowForEdit
            //&& !metadata.IsComplexType
            && !ViewData.TemplateInfo.Visited(metadata);
    }
</script>
<%
    if (ViewData.TemplateInfo.TemplateDepth > 3) { %>
    <% if (Model == null) { %>
        <%= ViewData.ModelMetadata.NullDisplayText %>
    <% } else { %>
        <%= ViewData.ModelMetadata.SimpleDisplayText %>
    <% } %>
<% } else { %>
    <div class='object'>
    <% foreach (var prop in ViewData.ModelMetadata.Properties.Where(pm => ShouldShow(pm))) {
           int useDepth = ViewData.TemplateInfo.TemplateDepth + ((ViewData["addDepth"] as int?) ?? 0) - 1;
           if (prop.HideSurroundingHtml) { %>
            <div class="editor-field indent-<%= useDepth %>"><%= Html.Editor(prop.PropertyName) %></div>
        <% } else { %>
            <% if (!String.IsNullOrEmpty(Html.Label(prop.PropertyName).ToHtmlString())) { %>
                <div class="editor-label indent-<%= useDepth %>"><%= Html.Label(prop.PropertyName) %></div>
            <% } %>
            <div class="editor-field indent-<%= useDepth %>">
                <%= Html.Editor(prop.PropertyName) %>
                <%= Html.ValidationMessage(prop.PropertyName, "*") %>
            </div>
        <% } %>
    <% } %>
    </div>  
<% } %>