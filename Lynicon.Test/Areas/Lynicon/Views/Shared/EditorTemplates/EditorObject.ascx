<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Lynicon.Utility" %>
<%@ Import Namespace="Lynicon.Attributes" %>
<%@ Import Namespace="Lynicon.Extensibility" %>
<script runat="server">
    bool ShouldShow(ModelMetadata metadata) {
        return metadata.ShowForEdit
            //&& !metadata.IsComplexType
            && !ViewData.TemplateInfo.Visited(metadata);
    }
</script>

<div class='object level-0'>
<%
    var dispProps = ViewData.ModelMetadata.Properties
        .Where(pm => ShouldShow(pm))
        .Select(pm => new {
            pm,
            title = pm.AdditionalValues.GetSelectOrDefault<DisplayBlockAttribute, string>("DisplayBlock", dba => dba.Title),
            order = pm.AdditionalValues.GetSelectOrDefault<DisplayBlockAttribute, int>("DisplayBlock", dba => dba.Order)
        })
        .ToList();
        
    var propGroups = dispProps
       .Where(pmi => pmi.title != null)
       .GroupBy(pmi => (string)pmi.title)
       .Select(pmig => new {
           title = pmig.Key,
           order = pmig.Max(pmi => pmi.order),
           display = (object)pmig.Select(pmi => pmi.pm).ToList()
       });

    var singleProps = dispProps
        .Where(pmi => pmi.title == null)
        .Select(pmi => new
        {
            title = (string)null,
            order = pmi.order == default(int) ? int.MaxValue : pmi.order,
            display = (object)pmi.pm
        });

    var uiBlocks = LyniconUi.Instance.CurrentEditorPanels(ViewData.ModelMetadata.ModelType)
        .Select(ep => new
        {
            title = ep.Title,
            order = ep.Order,
            display = (object)ep
        });

    var allBlocks = propGroups.Concat(singleProps).Concat(uiBlocks).OrderBy(bi => bi.order);
       
    foreach (var block in allBlocks)
    {
        var blockProperties = block.display as List<ModelMetadata>;
        var blockSingleProp = block.display as ModelMetadata;
        var editorPanel = block.display as EditorPanel;
        if (blockProperties != null) {
            ViewData["addDepth"] = 1;%>

    <div class="editor-unit parent-unit level-0">
        <div class="editor-label indent-0 parent child-closed"><%= Html.Label(block.title) %></div>
        <div class="editor-field indent-0">
            <div class="object level-1">
            <% foreach (var prop in blockProperties) { %>
                <div class="editor-unit level-1">
                <% if (prop.HideSurroundingHtml) { %>
                    <div class="editor-field indent-1"><%= Html.Editor(prop.PropertyName) %></div>
                <% } else { %>
                    <% if (!String.IsNullOrEmpty(Html.Label(prop.PropertyName).ToHtmlString())) { %>
                        <div class="editor-label indent-1"><%= Html.Label(prop.PropertyName) %></div>
                    <% } %>
                    <div class="editor-field indent-1">
                        <%= Html.Editor(prop.PropertyName) %>
                        <%= Html.ValidationMessage(prop.PropertyName, "*") %>
                    </div>
                <% } %>
                </div>
            <% } %>
            </div>
        </div>
    </div>

<%          ViewData["addDepth"] = 0;
        } else if (blockSingleProp != null) {
   %>

    <div class="editor-unit level-0">
    <% if (blockSingleProp.HideSurroundingHtml) { %>
        <div class="editor-field indent-0"><%= Html.Editor(blockSingleProp.PropertyName) %></div>
    <% } else { %>
        <% if (!String.IsNullOrEmpty(Html.Label(blockSingleProp.PropertyName).ToHtmlString())) { %>
            <div class="editor-label indent-0"><%= Html.Label(blockSingleProp.PropertyName) %></div>
        <% } %>
        <div class="editor-field indent-0">
            <%= Html.Editor(blockSingleProp.PropertyName) %>
            <%= Html.ValidationMessage(blockSingleProp.PropertyName, "*") %>
        </div>
    <% } %>
    </div>

<%      } else if (editorPanel != null) { %>

        <div class="editor-unit parent-unit level-0">
        <div class="editor-label indent-0 parent child-closed"><%= Html.Label(editorPanel.Title) %></div>
        <div class="editor-field indent-0">
            <div class="object level-1">
            <%= Html.Partial(editorPanel.ViewName) %>
            </div>
        </div>
    </div>
<%      }
    } %>
</div>  