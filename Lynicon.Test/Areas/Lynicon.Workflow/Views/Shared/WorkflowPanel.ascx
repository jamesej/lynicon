<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Lynicon.Workflow.Models" %>
<%@ Import Namespace="Lynicon.Membership" %>
<%@ Import Namespace="Lynicon.Utility" %>
<%
    var lmi = LayerManager.Instance;
    var wfu = (IWorkflowUser)LyniconSecurityManager.Current.User;
    var layers = lmi.GetUserLayers(wfu.Id);
    %>
<div id="wf-working-layer" <%= layers != null && layers.Count > 0 ? "" : "style='display:none'" %>>
    <span>Working Layer</span>
    <select id="wf-layer-select">
        <% foreach (var layer in layers) { %>
            <option value="<%= layer.Level %>" <%= wfu.CurrentLevel == layer.Level ? "selected" : "" %>>
                <%= layer.Name %>/<%= layer.Level %>
            </option>
        <% } %>
    </select>
</div>

<input type="text" id="wf-layer-name" />
<a id="wf-get-layer" class="func-button">Get Layer</a>
<a id="wf-view-layers" class="func-button" href="/lynicon/workflow/viewlayers">View Layers</a>
<script type="text/javascript">
$('#wf-get-layer').click(function () {
    if (!$('#wf-layer-name').val()) {
        alert('Please enter a name for the new layer');
        return;
    }
    $.post('/lynicon/workflow/getlayer', { name: $('#wf-layer-name').val() }, function (layer) {
        if (!layer['Name']) {
            alert(layer);
            return;
        }
        var newOpt = $('<option/>').prop('value', layer.Level).text(layer.Name);
        $('#wf-layer-select').append(newOpt).val(layer.Level);
        $('#wf-working-layer').css('display', 'block');
    });
});
$('#wf-layer-select').change(function () {
    $.post('/lynicon/workflow/setcurrentlayer', { level: $('#wf-layer-select').val() }, function (resp) {
        if (resp != "OK")
            alert(resp);
    });
});
</script>


