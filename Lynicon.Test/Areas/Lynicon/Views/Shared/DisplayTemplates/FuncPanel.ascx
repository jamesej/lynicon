<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Lynicon.Extensibility" %>
<%
    LyniconUi ui = LyniconUi.Instance;
     %>
<div id='funcPanel'>
    <% foreach (var btn in ui.CurrentFuncPanelButtons) {
         string bgCol = string.IsNullOrEmpty(btn.BackgroundColor) ? "" : "style='background-color: " + btn.BackgroundColor + "'";
         string url = ui.ApplySubstitutions(btn.Url, ViewContext, ViewBag); %>
    <<%= url == "" ? "div" : "a href=\"" + url + "\"" %> id="<%= btn.Id %>" class="func-button" style="background-color: <%= bgCol %>">
        <%= btn.Caption %>
    </<%= url == "" ? "div" : "a"%>>
    <% } %>
    <div id="save" class="action-button">SAVE</div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        <% foreach (var btn in ui.FuncPanelButtons.Where(fpb => !string.IsNullOrEmpty(fpb.ClientClickScript))) { %>
        $('#<%= btn.Id %>').click(function () {
            <%= ui.ApplySubstitutions(btn.ClientClickScript, ViewContext, ViewBag) %>
        });
        <% } %>
    });
</script>

