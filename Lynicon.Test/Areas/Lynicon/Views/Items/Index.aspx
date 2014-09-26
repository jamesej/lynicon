<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>Lynicon - Items</title>
    <script id="jquery" src="/Lynicon/Embedded/Scripts/jquery.js/" type="text/javascript"></script>

    <link type="text/css" href="/Lynicon/Embedded/Content/jquery-ui.css/" rel="stylesheet" />
    <link href="/Areas/Lynicon/Content/LyniconMain.css" rel="stylesheet" type="text/css" />
    <link type="text/css" href="/Areas/Lynicon/Content/font-awesome.css" rel="stylesheet" />
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/jquery-ui.js/"></script>
    <script src="/Lynicon/Embedded/Scripts/jquery.tmpl.js/" type="text/javascript"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/masonry.js/"></script>
    <script type="text/javascript" src="/Lynicon/Embedded/Scripts/LyniconMain.js/"></script>
    <script type="text/javascript" src="/Areas/Lynicon/Scripts/LyniconMasonryBoxes.js"></script>
</head>
<body style="overflow-y: scroll">
    <%= Html.Partial("LyniconItems") %>
</body>
</html>
