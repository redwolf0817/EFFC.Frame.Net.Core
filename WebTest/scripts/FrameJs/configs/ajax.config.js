define(['jQuery', 'effc'], function ($, fns) {
    fns.Config.Ajax =
        {
            beforeAjax: function (ajaxopts) {
                if (ajaxopts) {
                    //加入翻页器的数据
                    if ($("#toPage").size() > 0) {
                        ajaxopts.postdata["toPage"] = $("#toPage").val();
                    }
                    if ($("#Count_per_Page").size() > 0) {
                        ajaxopts.postdata["Count_per_Page"] = $("#Count_per_Page").val();
                    }
                }

                return true;
            },
            //请求成功，并处理返回值
            successAction: function (ajaxopts, rtn) {
                //判斷jsonstr是否就是json對象
                var jobj = rtn;
                if (ajaxopts.returntype.toLowerCase() == "json") {
                    if (jobj.Content.__isneedlogin__) {
                        fns.Message.Dialog({
                            msg: "请先登录！",
                            buttons: [{
                                text: "确定",
                                click: function () {
                                    location.href = jobj.Content.__loginurl__;
                                }
                            }
                            ]
                        });
                        return false;
                    } else {
                        return true;
                    }
                } else {
                    return true;
                }
            }
        }
});
