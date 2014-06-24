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
    <link type="text/css" href="/Lynicon/Embedded/Content/jquery-ui.css/" rel="stylesheet" />
    <link href="/Areas/Lynicon/Content/LyniconMain.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/jquery-ui.js/"></script>
    <script src="/Lynicon/Embedded/Scripts/jquery.tmpl.js/" type="text/javascript"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/LyniconMain.js/"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/chosen.jquery.js/"></script>
    <script type="text/javascript" id="simplemodal-script" src="/Lynicon/Embedded/Scripts/jquery.simplemodal.js/"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/LyniconEditPanel.js/"></script>

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
