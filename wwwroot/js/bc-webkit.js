/* COPY OF be-wekit */
var ui_rightSlider = {
    init: function () {
        $("#uikit-rightslider").find(".bc-webkit--right-floater-panel-close").
            click(function () { ui_rightSlider.hide(); });
    },
    show: function (p) {
        var o = $("#uikit-rightslider");
        o.css({ "right": "-500px;" });
        o.css({ "right": "0" });
        return ui_rightSlider;
    },
    style: function (v) {
        // $("#uikit-rightslider").css(v);
    }
    ,
    content: function (v) {
        var _e = $("#uikit-rightslider-body");
        if (typeof v != "undefined") {
            _e.html(v);
            return ui_rightSlider;
        }
        else {
            return _e.html();
        }
    },
    append: function (o) {
        $("#uikit-rightslider-body").append(o);
    },
    hide: function () {
        var o = $("#uikit-rightslider");
        $("#uikit-rightslider-body").html('');
        o.hide();
        o.css({ "right": "-500px" }).delay(1000).show();
    }
}
var ui_tinyloader = {
    show: function (v) {
        if (typeof v == "undefined") {
            v = "loading..."
        }
        $("#uikit-tinyloader").show()
            .find(".loader")
            .first().html(v);
    },
    hide: function () {
        $("#uikit-tinyloader").fadeOut()
    }
}
var ui_mpanel = {
    active:null,
    show: function (p, t, f) {
        p = $("#" + p);
        ui_mpanel.active = p;
        if (typeof t !== "undefined") {
            $(p).find(".bckit-modal-box-header > label")
                .html(t);
        }
        if (typeof f !== "undefined") {
           p.css(f);
        }

        $("#bckit-modal").addClass("bckit-modal-overlay-open");
        $(p).find(".bckit-modal-box-closebtn").off().
            click(function () {
                ui_mpanel.hide();
            });
        $(ui_mpanel.active).show();
    },
    hide: function () {
        $("#bckit-modal").removeClass("bckit-modal-overlay-open");
        $(ui_mpanel.active).fadeOut();
    }
}
ui_rightSlider.init();
(function (w, j) {
    $.fn.bcKitTab = function () { let t = $(this); var a; a = t, $(a).find(".bc-webkit-tabshead li").click(function () { $(a).find(".bc-webkit-tabshead li").removeClass("tab-selected"), $(this).addClass("tab-selected"); let t = $(this).data("tabtarget"); var e = $(a).find(".tab-panels .tab-panel"); $(e).hide(), $.each(e, function (a) { $(this).data("tabsource") !== t || $(this).show() }) }) };
    $.fn.bcKitModal = function () { let t = $(this); $(t).find(".bc-webkit--modal-close").first().click(function () { $(t).fadeOut() }) };
})(window, jQuery);
/* -----^^---- */
