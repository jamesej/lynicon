
function notifyLayout() {
    $('#editPanel .object.level-0').masonry('layout');
}

function notifyVisible($container) {
    $container.trigger('shown');
    if ($.fn.chosen)
        $container.find('.chosen-select:visible').chosen();
}


$(document).ready(function () {
    $('body').on('click', '.editor-label.parent', function (ev) {
        if (!$(ev.target).hasClass('parent')) // ignore click on buttons on bar
            return;
        var $collection = $(this).next('.editor-field').children('.collection, .object');
        if ($collection.length == 0) return;
        $(this).toggleClass('child-closed').toggleClass('child-open');
        $collection.toggleClass('closed');
        var formState = $('#formState').val();
        if ($collection.hasClass('closed')) {
            if (formState || (formState == ''))
                $('#formState').val(formState.replace($collection.prop('id') + ";", ""));
        } else {
            //$('#formState').val(formState + $collection.prop('id') + ";");
            notifyVisible($collection);
        }
        notifyLayout();
    });
});

