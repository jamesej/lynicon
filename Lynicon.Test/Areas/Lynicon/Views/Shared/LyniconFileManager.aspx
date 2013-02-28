<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>L24CMFileManager</title>
    
    <link href="/L24CM/Embedded/Content/jquery.jstreelist.css" rel="stylesheet" type="text/css" />
    <link href="/L24CM/Embedded/Content/jquery.layout.css" rel="stylesheet" type="text/css" />
    <link href="/L24CM/Embedded/Content/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <link href="/L24CM/Embedded/Content/L24Main.css" rel="stylesheet" type="text/css" />

    <script src="/L24CM/Embedded/Scripts/jquery.js" type="text/javascript"></script>
    <script src="/L24CM/Embedded/Scripts/jquery-ui.js" type="text/javascript"></script>
    <script src="/L24CM/Embedded/Scripts/jquery.tmpl.js" type="text/javascript"></script>
    <script src="/L24CM/Embedded/Scripts/fileuploader.js" type="text/javascript"></script>
    <script type="text/javascript" src="/L24CM/Embedded/Scripts/jquery.jstree.js"></script>
    <script type="text/javascript" src="/L24CM/Embedded/Scripts/jquery.jstreelist.js"></script>
    <script type="text/javascript" src="/L24CM/Embedded/Scripts/jquery.layout.js"></script>
    <script type="text/javascript" src="/L24CM/Embedded/Scripts/jquery.contextMenu.js"></script>

    <script type="text/javascript">
    
        $(document).ready(function() {
            $('body').jstreelist({ rootPath: '/' });
            $('#createFolder').click(function() {
                var newPath = $(".file-name").text() + $("#newFolder").val();
                $.post('/L24CM/Ajax/CreateFolder',
                    { path: newPath },
                    function() {
                        $("#container_id")[0].insertItem(newPath);
                    });
            });
            $('#outer').layout();
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
<body>
    <div id='filemgr'>
        <div id='outer'>
            <div id='treeContainer' class='treeContainer ui-layout-west'></div>
            <div id='listContainer' class='listContainer ui-layout-center'></div>
        </div>
        <div id='filenameBox'>
            <input id='filename' class='filename' type='text' />
        </div>
    </div>
</body>
</html>
