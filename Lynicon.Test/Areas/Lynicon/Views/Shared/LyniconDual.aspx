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
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/masonry.js/"></script>
    <link href="/Lynicon/Embedded/Content/chosen.css/" rel="Stylesheet" type="text/css" />
    <link href="/Lynicon/Embedded/Content/jquery.jstreelist.css/" rel="stylesheet" type="text/css" />
    <link href="/Lynicon/Embedded/Content/jquery.contextMenu.css/" rel="stylesheet" type="text/css" />
    <link type="text/css" href="/Lynicon/Embedded/Content/jquery-ui.css/" rel="stylesheet" />
    <link href="/Areas/Lynicon/Content/LyniconMain.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/jquery-ui.js/"></script>
    <script src="/Lynicon/Embedded/Scripts/jquery.tmpl.js/" type="text/javascript"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/LyniconMain.js/"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/chosen.jquery.js/"></script>
    <script type="text/javascript" id="simplemodal-script" src="/Lynicon/Embedded/Scripts/jquery.simplemodal.js/"></script>
    <script type="text/javascript" id="tinymce-script" src="/Areas/Lynicon/scripts/tiny_mce/jquery.tinymce.js"></script>
    <script src="/Lynicon/Embedded/Scripts/fileuploader.js/" type="text/javascript"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/jquery.contextMenu.js/"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/jquery.jstree.js/"></script>
    <script type="text/javascript" id="jstreelist-script" src="/Lynicon/Embedded/Scripts/jquery.jstreelist.js/"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/LyniconEditPanel.js/"></script>

    <script>
        $(document).ready(function() {
            var firstReload = true;
            $('#edit').load(function() {
                if (!firstReload)
                    $('#view')[0].contentDocument.location.reload(true);
                firstReload = false;
            });
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
    <iframe id="view" src="<%= ViewBag.BaseUrl as string %>?$mode=view<%= ViewBag.OriginalQuery as string %>"></iframe>
    <div id="edit">
        <div id="editPanelContainer">
            <div id='editPanel'>
             <% using (Html.BeginForm())
               { %>
                <%= Html.EditorForModel("EditorObject") %>
                <%= Html.Hidden("formState") %>
            <% } %>
            </div>
        </div>
        <%= Html.DisplayForModel("FuncPanel") %>
        <div id="opener">
            <img id="opener-out" src="/Areas/Lynicon/Content/left-arrow-white.png" />
            <img id="opener-in" src="/Areas/Lynicon/Content/right-arrow-white.png" />
        </div>
        <div style="clear:both; height: 44px;"></div>
    </div>
</div>
</body>
</html>
