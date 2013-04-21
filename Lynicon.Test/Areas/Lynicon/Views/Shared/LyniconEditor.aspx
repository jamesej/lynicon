<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
        <title>Editor</title>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.6.2/jquery.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/jquery-ui.js/"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/LyniconMain.js/"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/LyniconEditPanel.js/"></script>
    <!--<link type="text/css" href="/L24CM/Embedded/Content/L24Main.css" rel="stylesheet" />-->
    <link type="text/css" href="/Lynicon/Embedded/Content/jquery-ui.css/" rel="stylesheet" />
    <link type="text/css" href="/Areas/Lynicon/Content/LyniconMain.css" rel="stylesheet" />
</head>
<body>
    <div id='funcPanel'>
        <a href="/Lynicon/Login/Logout?returnUrl=<%= Request.Url.AbsolutePath %>" target="_top">Log out</a>
    </div>
    <div id='editPanel'>
        <% using (Html.BeginForm())
           { %>
            <%= Html.EditorForModel() %>
            <div id="save" class="action-button">SAVE</div>
            <%= Html.Hidden("formState") %>
        <% } %>
    </div>
</body>
</html>
