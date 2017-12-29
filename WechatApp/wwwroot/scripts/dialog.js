///
function dialog(msg, title, buttons, modal, width) {
    if (msg == null) return;

    var o;
    if (msg.jquery)
        o = msg;
    else if (!(msg.charAt(0) === "<" && msg.charAt(msg.length - 1) === ">" && msg.length >= 3))
        o = $("<div>" + msg + "</div>");
    else
        o = $(msg);

    var d = o.dialog({
        title: title || "",
        modal: (modal && modal.toString().toLowerCase() == "false") ? false : true,
        resizable: false,
        draggable: false,
        position: "center",
        //dialogClass: "scv-dialog",
        width: (!width || isNaN(width)) ? undefined : width,
        buttons: buttons || []
    });

    var e = "resize." + d.attr("id");

    d.on("dialogclose", function () {
        $(window).unbind(e);
        d.dialog("destroy");
    });

    $(window).on(e, function () {
        d.dialog("widget").css("max-height", ($(window).height() - 48) + "px")
        d.dialog("option", "position", { my: "center", at: "center", of: window });
    }).trigger(e);

    return d;
}

function Alert(msg, button, defaultButton) {
    if (!defaultButton)
        defaultButton = 0;

    if (!$.isArray(button) || button.length == 0)
        button = [{ text: "确定" }];

    $.each(button, function (i, j) {
        var callback = button[i].click;
        button[i].click = function () {
            if (typeof callback === "function")
                callback.call(this);
            defaultButton = undefined;
            $(this).dialog("close");
        }
    });

    dialog(msg, "提示", button)
    .on("dialogclose", function (event, ui) {
        if (defaultButton && button.length > defaultButton)
            button[defaultButton].click();
    });
}

//function ToUrl(url) {
//    if (!url) return;
//    var pos = url.indexOf(":");
//    var t = url.substring(0, pos);
//    var u = url.substring(pos + 1);
//    switch (t.toLowerCase()) {
//        case "javascript":
//            eval(u);
//            break;
//        case "blank":
//            window.open(u);
//            break;
//        case "self":
//            location.href = u;
//            break;
//        case "opener":
//            try {
//                if (window.opener.location.protocol == window.location.protocol &&
//                    window.opener.location.host == window.location.host &&
//                    window.opener.location.port == window.location.port) {
//                    window.opener.location.href = u;
//                    window.close();
//                } else {
//                    location.href = u;
//                }
//            } catch (e) {
//                location.href = u;
//            }
//            break;
//        case "opener_reload":
//            try {
//                if (window.opener.location.protocol == window.location.protocol &&
//                    window.opener.location.host == window.location.host &&
//                    window.opener.location.port == window.location.port) {
//                    window.opener.location.reload();
//                    window.close();
//                } else {
//                    location.href = u;
//                }
//            } catch (e) {
//                location.href = u;
//            }
//            break;
//        default:
//            location.href = url;
//            break;
//    }
//}

//function DoPost(url, form, callback) {
//    DoProcessDiv();
//    $.post(url, form.jquery ? form.serialize() : form, null, "json")
//    .done(function (data, textStatus, jqXHR) {
//        if (data.ErrorCode == "") {
//            if (typeof callback === "function") {
//                if (callback.call(this, data, form) === false)
//                    return;
//            }

//            if (data.Content.errors && $.validator) {
//                form.validate().showErrors(data.Content.errors);
//            }

//            if (data.Content.message) {
//                Alert(data.Content.message, data.Content.url, data.Content.button);
//            }
//            else if (data.Content.url) {
//                ToUrl(data.Content.url);
//            }
//        } else {
//            Alert(data.ErrorCode + "\n" + data.ErrorMsg);
//        }
//    })
//    .fail(function (jqXHR, textStatus, errorThrown) {
//        Alert(textStatus + "-" + errorThrown);
//    })
//    .always(function () {
//        CloseProcessDiv();
//    });
//}

//function DoGetJson(url, callback) {
//    DoProcessDiv();
//    $.getJSON(url)
//    .done(function (data, textStatus, jqXHR) {
//        if (data.ErrorCode == "") {
//            if (typeof callback === "function") {
//                if (callback.call(this, data) === false)
//                    return;
//            }

//            if (data.Content.message) {
//                Alert(data.Content.message, data.Content.url, data.Content.button);
//            }
//            else if (data.Content.url) {
//                ToUrl(data.Content.url);
//            }
//        } else {
//            Alert(data.ErrorCode + "\n" + data.ErrorMsg);
//        }
//    })
//    .fail(function (jqXHR, textStatus, errorThrown) {
//        Alert(textStatus + "-" + errorThrown);
//    })
//    .always(function () {
//        CloseProcessDiv();
//    });
//}
