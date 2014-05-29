<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Lynicon.Utility" %>

<%= Html.RegisterScript("lyn_fileeditor_script",
@"javascript:var lynFileEditors = [];
function getFile(current, updateFile) {
    var mediaWin = window.open('http://media.greatbritishchefs.com/library?selectTarget=any', 'mediaLib', 'width=800,height=600,scrollbars=yes');
    lynFileEditors.push({ win: mediaWin, updateFile: updateFile });
}
$(window).on('message', function (ev) {
    for (var i = 0; i < lynFileEditors.length; i++)
        if (lynFileEditors[i].win == ev.originalEvent.source) {
            if (ev.originalEvent.data == 'closed') {
                lynFileEditors.splice(i, 1);
            } else {
                lynFileEditors[i].updateFile(ev.originalEvent.data);
            }
        }
});
", new List<string>{ "jquery" })%>
