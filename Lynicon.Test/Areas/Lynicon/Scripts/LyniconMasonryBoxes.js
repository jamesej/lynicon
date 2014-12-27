
function notifyLayout() {
    $('#editPanel .object.level-0').masonry('layout');
}

function notifyVisible($container) {
    $container.trigger('shown');
    $container.find('select.post-load-select:visible').each(function () {
        loadRefSelect($(this));
    });
    if ($.fn.chosen)
        $container.find('.chosen-select:visible').chosen({ search_contains: true });
}

function notifyAddSelectOption($container, type, val, txt)
{
    for (var list in lynSelectLists) {
        if (list.indexOf(type) >= 0)
            lynSelectLists[list].push({ value: val, text: txt });
    }
    $container.find('.chosen-container').each(function () {
        var $listId = $(this).siblings('input.select-list-id');
        if ($listId.val().indexOf(type) >= 0) {
            $(this).siblings('select.chosen-select')
                .append($('<option value=""' + val + '"">' + txt + '</option>'))
                .trigger('chosen:updated');
        }
    });
}

$(document).ready(function () {
    $('body').on('click', '.editor-label.parent', function (ev) {
        if (!$(ev.target).hasClass('parent')) // ignore click on buttons on bar
            return;
        var $collobj = $(this).next('.editor-field').children('.collection, .object');
        if ($collobj.length == 0) return;
        $(this).toggleClass('child-closed').toggleClass('child-open');
        $collobj.toggleClass('closed');
        var formState = $('#formState').val();
        if ($collobj.hasClass('closed')) {
            if (formState || (formState == ''))
                $('#formState').val(formState.replace($collobj.prop('id') + ";", ""));
        } else {
            //$('#formState').val(formState + $collection.prop('id') + ";");
            notifyVisible($collobj);
        }
        notifyLayout();
    }).on('dblclick', '.editor-unit.level-0 > .editor-field', function (ev) {
        $(this).closest('.editor-unit').toggleClass('wide');
        notifyLayout();
    });
});

