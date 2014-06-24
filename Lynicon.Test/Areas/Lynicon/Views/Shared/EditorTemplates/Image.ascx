<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Models.Image>" %>
<%@ Import Namespace="Lynicon.Utility" %>
<div class="lyn-image">
    <div class="lyn-image-content">
        <% if (string.IsNullOrEmpty(Model.Url))
            { %>
        no image
        <% }
            else
            { %>
        <img class='file-image-thumb' src="<%= Model.Url %>" />
        <% } %>
    </div>
    <div class="lyn-image-url" style="display:none">
        <span>Url</span>
        <input type="text" class="lyn-file-url" name="<%= ViewData.TemplateInfo.HtmlFieldPrefix %>.Url" value="<%= Model.Url %>" />
    </div>
    <div class='lyn-image-alt' style="display:none">
        <button class='lyn-image-load'>Find File</button>
        <span>Alt</span>
        <%= Html.EditorFor(m => m.Alt) %>
    </div>
</div>
<%= Html.Partial("EnsureFileEditor") %>
<%= Html.RegisterScript("lyn_image_control", @"javascript:$(document).ready(function() {
    $('body').on('click', '.lyn-image', function (ev) {
            if ($(ev.target).is('.lyn-image-load, input'))
                return;
            $(this).find('.lyn-image-content, .lyn-image-url, .lyn-image-alt').toggle();
        }).on('click', '.lyn-image-load, .lyn-media-load', function () {
            var $this = $(this);
            var $fname = $this.closest('.lyn-image').find('input.lyn-file-url');
            top.getFile($fname.val(), function (fname) {
                var files = fname.split(',');
                if ($this.hasClass('lyn-image-load')) {
                    for (var i = 0; i < files.Length; i++) {
                        var suffix = fname.afterLast('.').toLowerCase();
                        if (suffix && suffix.length && 'png|jpg|gif'.indexOf(suffix) < 0)
                            return 'Please only image files';
                    }
                }
                if (files.length == 1) {
                    setFilename($fname, fname);
                } else {
                    if (confirm('You have selected ' + files.length + ' files, do you want to add them all?')) {
                        var $addButton = $this.closest('.collection').children('.add-button');
                        setFilename($fname, $.trim(files[0]));
                        for (var i = 1; i < files.length; i++) {
                            addItem($addButton, i, function ($added, idx) {
                                setFilename($added.find('.lyn-file-url'), $.trim(files[idx]));
                            });
                        }
                    } else
                        return 'Please select your files';
                }
                return null;
            });
            return false;
        });
    });
", new List<string> { "jquery" })
%>

