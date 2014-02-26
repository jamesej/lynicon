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
    <div id="reveal-button"><img id="reveal-arrow" src="/areas/lynicon/content/down-arrow-white.png" /></div>
    <div id="reveal" style="display:none">
        <% bool isAlt = false; foreach (var viewKvp in ui.RevealPanelViews)
        {%>
        <div class="reveal-panel-view <%= isAlt ? "alt" : "" %>">
           <%= Html.Partial(viewKvp.Value) %>
           <div style="clear:both"></div>
        </div>
       <%
       isAlt = !isAlt;}
        %>
    </div>
    <div id="save" class="action-button">SAVE</div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        <% foreach (var btn in ui.FuncPanelButtons.Where(fpb => !string.IsNullOrEmpty(fpb.ClientClickScript))) { %>
        $('#<%= btn.Id %>').click(function () {
            <%= ui.ApplySubstitutions(btn.ClientClickScript, ViewContext, ViewBag) %>
        });
        <% } %>
        $('#reveal-button').click(function () {
            if ($('#reveal').length == 0) {
                $("<div id='reveal'></div>")
                    .hide()
                    .insertBefore($('#save'))
                    .load('/lynicon/ui/functionreveal', function () {
                        $('#reveal').show('slow');
                    });
                $('#reveal-arrow').attr('src', '/areas/lynicon/content/up-arrow-white.png');
            } else if ($('#reveal').is(':visible')) {
                $('#reveal').hide('slow');
                $('#reveal-arrow').attr('src', '/areas/lynicon/content/down-arrow-white.png');
            } else {
                $('#reveal').show('slow');
                $('#reveal-arrow').attr('src', '/areas/lynicon/content/up-arrow-white.png');
            }
            
        });
    });
</script>

