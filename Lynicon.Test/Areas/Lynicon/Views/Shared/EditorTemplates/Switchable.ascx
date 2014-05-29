<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Models.Switchable>" %>
<%@ Import Namespace="Lynicon.Models" %>
<%  int useDepth = ViewData.TemplateInfo.TemplateDepth + ((ViewData["addDepth"] as int?) ?? 0) - 1;
    var props = ViewData.ModelMetadata.Properties.Where(pm => pm.PropertyName != "SelectedProperty"); %>
    <div class='object level-<%= useDepth %>'>
    <% foreach (var prop in props) { %>
        <input class="lyn-switchable-radio" type="radio" name="selector" value="<%= prop.PropertyName %>"/><span><%= prop.PropertyName %></span>
    <% } %>
    <% foreach (var prop in props) { %>
        <div class="editor-unit level-<%= useDepth %>" <%= prop.PropertyName == Model.SelectedProperty ? "" : "style='display:none'" %>>
            <div class="editor-field indent-<%= useDepth %>"><%= Html.Editor(prop.PropertyName) %></div>
        </div>
    <% } %>
    </div>

