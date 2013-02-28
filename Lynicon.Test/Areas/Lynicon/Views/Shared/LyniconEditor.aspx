<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
        <title>Editor</title>
    <script type="text/javascript" src="/L24CM/Embedded/Scripts/jquery-ui.js"></script>
    <script type="text/javascript" src="/L24CM/Embedded/Scripts/L24EditPanel.js"></script>
    <!--<link type="text/css" href="/L24CM/Embedded/Content/L24Main.css" rel="stylesheet" />-->
    <link type="text/css" href="/L24CM/Embedded/Content/jquery-ui.css" rel="stylesheet" />
    <link type="text/css" href="/Areas/L24CM/Content/L24Main.css" rel="stylesheet" />
</head>
<body>
    <div id='funcPanel'>
        <a href="/L24CM/Login/Logout?returnUrl=<%= Request.Url.AbsolutePath %>" target="_top">Log out</a>
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
