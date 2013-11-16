<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%
    ViewBag.BaseUrl = (string)Url.Action(ViewBag.OriginalAction, ViewBag.OriginalController, new { area = ViewBag.OriginalArea });
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%; width: 100%" >
<head runat="server">
        <title>Editor</title>
    <!-- <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.6.2/jquery.min.js" type="text/javascript"></script> -->
    <script id="jquery" src="/Lynicon/Embedded/Scripts/jquery.js/" type="text/javascript"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/jquery-ui.js/"></script>
    <link href="/Lynicon/Embedded/Content/jquery.jstreelist.css/" rel="stylesheet" type="text/css" />
    <link href="/Lynicon/Embedded/Content/jquery.layout.css/" rel="stylesheet" type="text/css" />
    <link href="/Lynicon/Embedded/Content/jquery.contextMenu.css/" rel="stylesheet" type="text/css" />
    <link type="text/css" href="/Lynicon/Embedded/Content/jquery-ui.css/" rel="stylesheet" />
    <link href="/Areas/Lynicon/Content/LyniconMain.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/jquery-ui.js/"></script>
    <script src="/Lynicon/Embedded/Scripts/jquery.tmpl.js/" type="text/javascript"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/LyniconMain.js/"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/jquery.layout.js/"></script>
    <script type="text/javascript" id="simplemodal-script" src="/Lynicon/Embedded/Scripts/jquery.simplemodal.js/"></script>
    <script type="text/javascript" id="tinymce-script" src="/Areas/Lynicon/scripts/tiny_mce/jquery.tinymce.js"></script>
    <script src="/Lynicon/Embedded/Scripts/fileuploader.js/" type="text/javascript"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/jquery.contextMenu.js/"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/jquery.jstree.js/"></script>
    <script type="text/javascript" id="jstreelist-script" src="/Lynicon/Embedded/Scripts/jquery.jstreelist.js/"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/LyniconEditPanel.js/"></script>

    <script>
        function processField(v) {
            if (typeof v == "string" && v.substr(0, 6) == "/Date(") {
                var d = new Date(parseInt(v.substr(6)));
                return d.toLocaleDateString();
            } else
                return v;
        }
        function mapForm(func) {
            $('form').find('input, textarea, select').each(function () {
                if ($(this).prop('name').substr(0, 5) == 'item.')
                    func($(this));
            });
        }
        function loadDetail(idx) {
            var formNames = '';
            mapForm(function ($fld) {
                formNames += $fld.prop('name') + ' ';
            });
            $.post("<%= ViewBag.BaseUrl as string %>?$mode=getValues<%= ViewBag.OriginalQuery as string %>",
                { formNames: formNames, idx: idx },
                function (d) {
                    $('form').find('input, textarea, select').each(function () {
                        var nm = $(this).prop('name');
                        if (d.hasOwnProperty(nm))
                            $(this).val(processField(d[nm]));
                    });
                });
        }
        function clearForm() {
            mapForm(function ($fld) { $fld.val(null) });
        }
        $(document).ready(function() {
            $('#container').layout({ east: { size: '280', spacing_open: 10, spacing_closed: 14, togglerLength_open: 60 } });
            loadDetail(parseInt($('#lynicon_itemIndex').val()));
        });
        
    </script>
    <script id="fileListTemplate" type="text/x-jquery-tmpl">
        <table style='width:300px'>
        <tr><th></th><th>Name</th><th>Size</th></tr>
        {{each dirs}}<tr title='${dir}${name}/'><td class='dir jstree-default'><ins style='width:16px;height:16px;display:inline-block' class='ext ext_dir'/></td><td><span>${name}</span></td><td></td></tr>{{/each}}
        {{each files}}<tr title='${dir}${name}'><td><ins style='width:16px;height:16px;display:inline-block' class='ext ext_${ext}'/></td><td><span>${name}</span></td><td>${size}</td></tr>{{/each}}
        </table>
    </script>
</head>
<body style="height: 100%; width: 100%">
<div id='container' style="height: 100%; width: 100%; position:relative;">
<div class="ui-layout-center">
    <%= Html.EditorForModel((string)ViewBag.ListView, new { displayFields = ViewBag.DisplayFields })%>

    <% if (ViewBag.CanAdd) { %>
    <div id="lynicon-list-add-button" class="lynicon-ctl-button">ADD</div>
    <% } %>
    <% if (ViewContext.RouteData.DataTokens.ContainsKey("@Paging")) { %>
    <div><%= Html.Partial("PagingSpec", this.ViewContext.RouteData.DataTokens["@Paging"]) %></div>
    <% } %>
    <script type="text/javascript">
        $('.list-table').on('click', 'tr', function () {
            var idx = $(this).prop('id').after('-');
            loadDetail(parseInt(idx));
            $('#lynicon_itemIndex').val(idx);
            $(this).closest('.list-table').find('.selected').removeClass('selected');
            $(this).addClass('selected');
        });
        $('#lynicon-list-add-button').on('click', function () {
            $('#lynicon_itemIndex').val('-1');
            loadDetail(-1);
            $('.list-table').find('.selected').removeClass('selected');
        });
    </script>
</div>
<div class="ui-layout-east" id="edit">
    <%= Html.DisplayForModel("FuncPanel") %>
    <div id="editPanelContainer">
        <div id='editPanel'>
        <% var item = ((ICollection)Model).Cast<object>().FirstOrDefault();
           if (item == null)
           {
               item = Activator.CreateInstance(Model.GetType().GetGenericArguments()[0]);
               ViewBag.ItemIndex = -1;
           }
           using (Html.BeginForm())
           { %>
            <%= Html.EditorFor(m => item)%>
            <%= Html.Hidden("formState")%>
            <%= Html.Hidden("lynicon_itemIndex", (int)ViewBag.ItemIndex) %>
        <% } %>
        </div>
    </div>
</div>
</div>
</body>
</html>
