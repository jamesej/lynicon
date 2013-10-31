<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Lynicon.Utility" %>
<%= Html.RegisterHtmlBlock("lyn_fileeditor",
@"<div id='_L24FileMgrContainer' style='display:none'>
    <div id='outer'>
        <div id='treeContainer' class='treeContainer ui-layout-west'></div>
        <div id='listContainer' class='listContainer ui-layout-center'></div>
    </div>
    <div id='filenameBox'>
        <input id='filename' class='filename' type='text' />
        <div id='fileDetails' class='fileDetails'></div>
    </div>
</div>") %>
<%= Html.RegisterScript("lyn_fileeditor_script",
@"javascript:$(document).ready(function() {
    $('#_L24FileMgrContainer').jstreelist({ rootPath: '" + (ViewBag.FileManagerRoot as string) + @"' });
});
function getFile(current, updateFile) {
    var $fm = $('#_L24FileMgrContainer').css('display', 'block');
    $fm.find('#outer').layout();
    $fm.css({ 'z-index': '1010', position: 'fixed' });
    $(""<div id='modalPlaceholder' style='background-color: White;'></div>"")
        .width($fm.width()).height($fm.height())
        .modal({
            overlayClose: true,
            onClose: function(dialog) {
                var msg = updateFile($('#filename').val());
                if (msg) {
                    alert(msg);
                    this.bindEvents();
                    this.occb = false;
                } else {
                    $('#_L24FileMgrContainer').css('display', 'none');
                    $.modal.getContainer().unbind('move.modal');
                    $.modal.close();
                }
            }
        });

    $('.simplemodal-close').css({
        'z-index': '1003', position: 'fixed', display: 'block',
        'background-image': 'url(/lynicon/embedded/Content/Images/close-white.png/)',
        width: '16px', height: '16px'
    });
    positionTool('#_L24FileMgrContainer');
    $.modal.getContainer().bind('move.modal', function() { positionTool('#_L24FileMgrContainer'); });
            
}", new List<string> { "jstreelist-script" })%>
