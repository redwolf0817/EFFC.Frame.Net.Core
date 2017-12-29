; (function ($, $$) {
    $("#result").hide();

    var templateColumnNames = ["recommededstorename", "usermobile", "username","recommendmobile"];
    $("#search").click(function () {
        $$.Net.Ajax("recommendquery/query", "post", {
            usermobile: $("#recommender").val(),
            recommended_mobile: $("#recommended").val()
        }).then(function (rtn) {
            if (rtn.issuccess) {
                var table = $(".table tbody");
                table.empty();
                if (rtn.data) {
                    if (rtn.data.length <= 0) {
                        var tr = $('<tr class="error"><td colspan="3">查无资料</td></tr>');
                        table.append(tr);
                    } else {
                        $(rtn.data).each(function (i, v) {
                            var tr = $('<tr></tr>');
                           
                            for (var i = 0; i < templateColumnNames.length; i++) {
                                var td = $('<td>' + v[templateColumnNames[i]] + '</td >');
                                tr.append(td);
                            }
                            table.append(tr);
                        })
                    }
                    $("#pagenav").remove();
                    var pn = $$.loadPageNavigation({
                        current_page: rtn.currentpage,
                        count_per_page: rtn.count_per_page,
                        total_page: rtn.total_page,
                        total_rows: rtn.total_rows,
                        queryfunction: function () {
                            $("#search").click();
                        }
                    })
                    $(pn).attr("id", "pagenav");
                    $(".table").after($(pn));

                    $("#result").show();
                } else {
                    var tr = $('<tr class="error"><td colspan="3">查无资料</td></tr>');
                    table.append(tr);
                }

                $(10).each(function (i, v) {
                    console.debug(i);
                })
            } else {
                $$.Message.ShowMsg(rtn.msg);
            }
        })


    })

    $("#search").click();
}(jQuery, effc))