; (function ($, $$) {
    $("#save").click(function () {
        if (!$$.IsMobileFormat($("#recommender").val())) {
            $$.Message.ShowMsg('请输入正确的"推荐人电话号码"格式');
            return false;
        }
        if ($("#recommender_name").val() === "") {
            $$.Message.ShowMsg('"推荐人姓名"不可为空');
            return false;
        }
        if (!$$.IsMobileFormat($("#recommended").val())) {
            $$.Message.ShowMsg('请输入正确的"被推荐人电话号码"格式');
            return false;
        }
       

        $$.Net.Ajax("custbind/bind", "post", {
            usermobile: $("#recommender").val(),
            username: $("#recommender_name").val(),
            recommended_mobile: $("#recommended").val(),
            storecode: $("#storecode").val(),
        }).then(function (rtn) {
            if (rtn.issuccess) {
                $("#recommender").val("");
                $("#recommended").val("");
                $("#recommender_name").val("");
                $("#storecode option:first").attr("selected", true);
                $$.Message.ShowMsg("操作成功");
            } else {
                $$.Message.ShowMsg(rtn.msg);
            }
        })


    })

    $("#cancel").click(function () {
        $("#recommender").val("");
        $("#recommender_name").val("");
        $("#recommended").val("");
        $("#storecode option:first").attr("selected", true);
    })
}(jQuery, effc))