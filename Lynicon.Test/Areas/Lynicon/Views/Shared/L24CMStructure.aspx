<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<L24CM.Routing.SiteStructure>" %>
<%@ Import Namespace="L24CM.Routing" %>
<%@ Import Namespace="L24CM.Utility" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Structure Manager</title>
</head>
<body>
    <%= Html.RegisterCss("/L24CM/Embedded/Content/L24Main.css") %>
    <%= Html.RegisterScript("/L24CM/Embedded/Scripts/jquery.js") %>
    <%= Html.RegisterScript("/L24CM/Embedded/Scripts/jquery.tmpl.js") %>
    <script id="templatePanel" type="text/x-jquery-tmpl">
        <div class="template-panel">
            <div class="inst-list">
            {{each insts}}
                <div class="inst">
                    <input type="checkbox" name="${$value}"/>
                    <a href="${$value}">${$value}</a>
                </div>
            {{/each}}
            </div>
            <button class="delete">Delete</button>
            <span class="rename-panel">
                From <input name="renameFrom"/>
                To <input name="renameTo"/>
                <button class="rename">Rename</button>
            </span>
            <div class="edit-area">
                Click on a url format above to create a new url
            </div>
        </div>
    </script>
    <script id="urlBuilder" type="text/x-jquery-tmpl">
        {{if $data.substr(0,1) == "{"}}
            / <input name='${$data.slice(1,-1)}'/>
        {{else}}
            / ${$data}
        {{/if}}
    </script>
    
    <div id="structureManager">
    <% foreach (ControllerInfo ci in Model.Controllers)
       { %>
       <div class="editor-label">
         <div class="template-name"><%= ci.Name %></div>
         <div class="url-pattern">
            <% for (int i = 0; i < ci.DisplayPatterns.Count; i++)
               { %>
                <span class="p<%= i %>"><%= ci.DisplayPatterns[i] %></span><%= i == ci.DisplayPatterns.Count - 1 ? "" : ", "  %>
            <% } %>
         </div>
       </div>
    <% } %>
    </div>
    <button id="buildIndex">Build Index</button>
    <div><label>New Password</label><input type="password" id="newPassword" /></div>
    <button id="changePassword">Change Password</button>
    
    <script type="text/javascript">
        $(document).ready(function() {
            var isLoading = false;
            var loadMsgTO = null;

            function loadPanel($editorLabel, mode) {
                if (isLoading) return;

                var $label = $editorLabel.closest('.editor-label');
                var $panel = $label.next('.template-panel');
                if (mode == "toggle" && $panel.length) {
                    $panel.remove();
                } else if (!(mode == "ensure" && $panel.length)) {
                    if ($panel.length) {
                        $panel.remove();
                    }
                    $.get("/L24CM/Structure/Instances", { name: $editorLabel.find('.template-name').text() })
                    .success(function(d) {
                        isLoading = false;
                        clearTimeout(loadMsgTO);
                        $label.find('.load-msg').remove();
                        d.enterAction = (d.actions.length > 1);
                        $('#templatePanel').tmpl(d).insertAfter($editorLabel);
                    });
                    isLoading = true;
                    loadMsgTO = setTimeout(function() { $label.append("<span class='load-msg'>&nbsp;Loading...</span>"); }, 1500);
                }
            }
            $('#buildIndex').click(function() {
                $.post("/L24CM/Structure/BuildIndex")
                    .success(function() { alert('Done'); });
            });

            $('#structureManager').delegate('.editor-label', 'click', function(ev) {
                loadPanel($(this), $(ev.target).is('span') ? "ensure" : "toggle");
            })
            .delegate('.add-inst', 'click', function() {
                var $panel = $(this).closest('.template-panel');
                var $hdg = $panel.prev();
                var data = { name: $hdg.find('.template-name').text(), pattern: $(this).data('pattern'), fieldNames: [], fieldValues: [] };
                $panel.find('.edit-area input').each(function() {
                    data.fieldNames.push($(this).attr('name'));
                    data.fieldValues.push($(this).val());
                });
                $.ajax({
                    type: 'POST',
                    url: "/L24CM/Structure/AddInstance",
                    data: data,
                    traditional: true
                })
                .done(function(d) {
                    if (d == "OK")
                        loadPanel($panel.prev('.editor-label'), "reload");
                })
                .fail(function(xhr, status, error) {
                    alert(xhr.responseText);
                });
            })
            .delegate('.url-pattern span', 'click', function(ev) {
                var $panel = $(this).closest('.editor-label').next('.template-panel');
                if ($panel.length) {
                    ev.stopPropagation();
                    var urlInputs = $(this).text().split('/');
                    var pos = $(this).attr('class').substr(1);
                    $panel.find(".edit-area").empty().append($('#urlBuilder').tmpl(urlInputs)).append($('<button class="add-inst">Create</button>').data('pattern', pos));
                }
            })
            .delegate('button.delete', 'click', function(ev) {
                ev.preventDefault();
                var $panel = $(this).closest('.template-panel');
                var $hdg = $panel.prev('.editor-label');
                var $deletions = $panel.find(':checked');
                if ($deletions.length) {
                    if (confirm("Are you sure you want to delete the ticked items?"))
                        $.ajax({
                            type: 'POST',
                            url: '/L24CM/Structure/DeleteInstances',
                            data: {
                                name: $hdg.find('.template-name').text(),
                                urls: $.map($deletions, function(el) { return $(el).attr('name'); })
                            },
                            traditional: true
                        })
                    .success(function(d) {
                        if (d == "OK")
                            loadPanel($hdg, "reload");
                    });
                } else {
                    alert("No items selected");
                }
                return false;
            })
            .delegate('button.rename', 'click', function(ev) {
                ev.preventDefault();
                var $panel = $(this).closest('.template-panel');
                var $hdg = $panel.prev('.editor-label');
                var $renames = $panel.find(':checked');
                if ($renames.length) {
                    if (confirm("Are you sure you want to rename the variable parts of the urls of the ticked items?"))
                        $.ajax({
                            type: 'POST',
                            url: '/L24CM/Structure/RenameInstances',
                            data: {
                                name: $hdg.find('.template-name').text(),
                                urls: $.map($renames, function(el) { return $(el).attr('name'); }),
                                from: $panel.find('[name="renameFrom"]').val(),
                                to: $panel.find('[name="renameTo"]').val()
                            },
                            traditional: true
                        })
                        .success(function(d) {
                            if (d == "OK")
                                loadPanel($hdg, "reload");
                        });
                } else {
                    alert("No items selected");
                }
                return false;
            });


        });
    </script>
</body>
</html>
