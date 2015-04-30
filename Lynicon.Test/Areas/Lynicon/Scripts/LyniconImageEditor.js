$(document).ready(function() {

    function setFilename($input, fname) {
        if (fname.indexOf('|') >= 0) {
            fname = fname.split('|')[0];
        }
        $input.val(fname);
        $input.siblings("span").text(fname);
        $input.closest(".lyn-image").find(".lyn-image-content").show().html("<div class='file-image-thumb' style='background-image:url(" + fname + ")'></div>");
        $input.closest(".lyn-image").find(".lyn-image-url, .lyn-image-alt").hide();
        setTimeout(notifyLayout, 200);
    }

    $("body").on("click", ".lyn-image", function (ev) {
        if ($(ev.target).is(".lyn-image-load, input"))
            return;
        $(this).find(".lyn-image-content, .lyn-image-url, .lyn-image-alt").toggle();
    }).on("click", ".lyn-image-load, .lyn-media-load", function () {
        var $this = $(this);
        var $fname = $this.closest(".lyn-image").find(".lyn-image-url input");
        var info = $this.data("get-file-info");
        top.getFile($fname.val(), info, function (fname) {
            var files = fname.split(",");
            if ($this.hasClass("lyn-image-load")) {
                for (var i = 0; i < files.length; i++) {
                    var suffix = fname.upTo("|").afterLast(".").toLowerCase();
                    if (suffix && suffix.length && "png|jpg|gif".indexOf(suffix) < 0)
                        return "Please only image files";
                }
            }
            if (files.length == 1) {
                setFilename($fname, fname);
            } else {
                if (confirm("You have selected " + files.length + " files, do you want to add them all?")) {
                    var $addButton = $this.closest(".collection").children(".add-button");
                    setFilename($fname, $.trim(files[0]));
                    for (var i = 1; i < files.length; i++) {
                        addItem($addButton, i, function ($added, idx) {
                            setFilename($added.find(".lyn-file-url"), $.trim(files[idx]));
                        });
                    }
                } else
                    return "Please select your files";
            }
            return null;
        });
        return false;
    });
});