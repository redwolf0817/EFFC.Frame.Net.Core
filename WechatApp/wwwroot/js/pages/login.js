; (function ($, $$) {
    $("#loginid").val(localStorage.getItem("my_loginid"));
    $("#loginpass").val(localStorage.getItem("my_loginpass"));
    if ($("#loginpass").val() != "") {
        $(".checkbox input").attr("checked", "true")
    }

    $(".btn.btn-lg.btn-primary.btn-block").click(function () {
        $$.Net.Ajax("/admin/login", "post", {
            loginid: $("#loginid").val(),
            loginpass: $("#loginpass").val()
        }).then(function (rtn) {
            if (rtn.issuccess) {
                if ($(".checkbox input").is(':checked')) {
                    localStorage.setItem("my_loginid", $("#loginid").val());
                    localStorage.setItem("my_loginpass", $("#loginpass").val())
                } else {
                    localStorage.removeItem("my_loginid");
                    localStorage.removeItem("my_loginpass");
                }

                sessionStorage.setItem("menu", JSON.stringify(rtn.info.function));
                var leftmenu = sessionStorage.getItem("currentLeftMenuClick");
                var left = leftmenu? JSON.parse(leftmenu):null;
                if (left && left.length > 0) {
                    location.href = left[left.length - 1].url;
                } else {
                    location.href = "/admin/index";
                }
            } else {
                $$.Message.ShowMsg(rtn.msg);
            }
        })

        
    })
}(jQuery,effc))