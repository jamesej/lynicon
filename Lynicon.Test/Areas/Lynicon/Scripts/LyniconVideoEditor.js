$(document).ready(function () {

    function setVideoFilename($input, $posterInput, fname) {
        var posterFname = null;
        if (fname.indexOf("|") >= 0) {
            posterFname = fname.split("|")[1];
            fname = fname.split("|")[0];
            $posterInput.val(posterFname);
            $posterInput.siblings(".text-display").text(posterFname);
            $posterInput.closest(".lyn-image").find(".lyn-image-content").show().html("<div class='file-image-thumb' style='background-image:url(" + posterFname + ")'></div>");
            $posterInput.closest(".lyn-image").find(".lyn-image-url, .lyn-image-alt").hide();
        }
        $input.val(fname);
        $input.siblings(".text-display").text(fname);
        $input.closest(".lyn-video").find(".lyn-video-content").show().html("<video class=\"thumb-vid\" src=\"" + fname + "\"" + (posterFname ? " poster=\"" + posterFname + "\"" : "") + " controls/>");
        $input.closest(".lyn-video").find(".lyn-video-url, .lyn-video-poster").hide();
        setTimeout(notifyLayout, 200);
    }

    $("body").on("click", ".lyn-video", function (ev) {
        if ($(ev.target).is(".lyn-video-load, input, .lyn-image") || $(ev.target).closest(".lyn-image").length)
            return;
        $(this).find(".lyn-video-content, .lyn-video-url, .lyn-video-poster").toggle();
    }).on("click", ".lyn-video-load", function () {
        var $this = $(this);
        var $fname = $this.closest(".lyn-video").find(".lyn-video-url input");
        var $posterFname = $this.closest(".lyn-video").find(".lyn-video-poster .lyn-image-url input");
        var info = null;
        top.getFile(null, info, function (fname) {
            var files = fname.split(",");
            if ($this.hasClass("lyn-video-load")) {
                for (var i = 0; i < files.length; i++) {
                    var suffix = files[i].split("|")[0].afterLast(".").upTo("?").toLowerCase();
                    if (suffix && suffix.length && "mp4|webm".indexOf(suffix) < 0)
                        return "Please only video files";
                }
            }
            if (files.length == 1) {
                setVideoFilename($fname, $posterFname, fname);
            } else {
                if (confirm("You have selected " + files.length + " files, do you want to add them all?")) {
                    var $addButton = $this.closest(".collection").children(".add-button");
                    setVideoFilename($fname, $posterFname, $.trim(files[0]));
                    for (var i = 1; i < files.length; i++) {
                        addItem($addButton, i, function ($added, idx) {
                            setVideoFilename($added.find(".lyn-file-url"), $posterFname, $.trim(files[idx]));
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