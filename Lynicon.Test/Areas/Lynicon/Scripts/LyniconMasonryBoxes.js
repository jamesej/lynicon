
function notifyLayout() {
    $("#editPanel .object.level-0").masonry("layout");
}

function notifyVisible($container) {
    $container.trigger("shown");
    $container.find("select.post-load-select:visible").each(function () {
        loadRefSelect($(this));
    });
    if ($.fn.chosen)
        $container.find("select.chosen-select:visible").chosen({ search_contains: true, allow_single_deselect: true });
    if ($.fn.selectize)
        $container.find("select.lyn-selectize:visible").each(function () {
            initSelectize($(this));
        });
}

function notifyAddSelectOption($container, type, val, txt)
{
    for (var list in lynSelectLists) {
        if (list.indexOf(type) >= 0)
            lynSelectLists[list].push({ value: val, text: txt });
    }
    $container.find(".chosen-container").each(function () {
        var $listId = $(this).siblings("input.select-list-id");
        if ($listId.val().indexOf(type) >= 0) {
            $(this).siblings("select.chosen-select")
                .append($("<option value=\"\"" + val + "\"\">" + txt + "</option>"))
                .trigger("chosen:updated");
        }
    });
}

function setBoxSpinner($box, isStart, margin) {
    var $bar = $box.children('.editor-label');
    if (isStart) {
        $bar.data('spinner', setTimeout(function () {
            var $spinner = $('<img src="/areas/lynicon/content/ajax-loader.gif"/>').addClass('spinner');
            if (margin) $spinner.css('right-margin', margin);
            $bar.append($spinner);
            $bar.data('spinner', '');
        }, 800));
    } else {
        $bar.children('.spinner').remove();
        if ($bar.data('spinner')) {
            clearTimeout($bar.data('spinner'));
            $bar.data('spinner', '');
        }
    }
}

$(document).ready(function () {
    $("body").on("click", ".editor-label.parent", function (ev) {
        if (!$(ev.target).hasClass("parent")) // ignore click on buttons on bar
            return;
        var $editor = $(this).next(".editor-field")
        var $collobj = $editor.children(".collection, .object, .object-wrapper");
        if ($collobj.hasClass("object-wrapper"))
            $collobj = $collobj.children(".collection, .object");
        if ($collobj.length == 0) return;
        $(this).toggleClass("child-closed").toggleClass("child-open");
        $collobj.toggleClass("closed");
        var formState = $("#formState").val();
        if ($collobj.hasClass("closed")) {
            if (formState || (formState == ""))
                $("#formState").val(formState.replace($collobj.prop("id") + ";", ""));
        } else {
            //$('#formState').val(formState + $collection.prop('id') + ";");
            notifyVisible($collobj);
        }
        notifyLayout();
    }).on("dblclick", ".editor-unit.level-0 > .editor-label", function (ev) {
        $(this).closest(".editor-unit").toggleClass("wide");
        notifyLayout();
    });
});

