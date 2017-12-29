; (function ($, $$) {
    $('.tree')
    var tree = $(".tree");

    $("#save").click(function () {
        if ($("#functionno_seqno").val() == "") {
            $$.Message.ShowMsg("请输入功能编号");
            return;
        }
        if ($("#functionname").val() == "") {
            $$.Message.ShowMsg("请输入功能名称");
            return;
        }

        var url = "function/" + $("#op").val();
        
        $$.Net.Ajax(url, "post", {
            parentno: $("#functionno_parentno").val(),
            no: $("#functionno_parentno").val() + $("#functionno_seqno").val(),
            name: $("#functionname").val(),
            url: $("#functionurl").val(),
            remark: $("#remark").val(),
            level:$("#level").val()
        }).then(function (rtn) {
            if (rtn.issuccess) {
                loadTree();
                $("#close").click();
            } else {
                $$.Message.ShowMsg(rtn.msg);
            }
        })
    })

    function loadTree() {
        $$.Net.Ajax("function/tree", "post", {
        }).then(function (rtn) {
            if (rtn.issuccess) {
                renderTree(rtn.data);
            } else {
                $$.Message.ShowMsg(rtn.msg);
            }
        })
    }
    function renderTree(data) {
        tree.empty();
        builderTree(data, null,null);
        tree.treegrid({
            expanderExpandedClass: 'glyphicon glyphicon-minus',
            expanderCollapsedClass: 'glyphicon glyphicon-plus'
        });
    }
    var currentIndex = 0;
    function builderTree(data, parentIndex,parentNode) {
        $(data).each(function (i, v) {
            currentIndex++;
            var tr = $('<tr></tr>');
            if (parentIndex && !isNaN(parentIndex)) {
                tr.addClass(String.format("treegrid-{0} treegrid-parent-{1}", currentIndex, parentIndex));
            } else {
                tr.addClass(String.format("treegrid-{0}", currentIndex, parentIndex));
            }
            var tdName = $(String.format('<td>{0}</td>', v.text));
            var tdNo = $(String.format('<td>{0}</td>', v.no));
            var tdUrl = $(String.format('<td>{0}</td>', v.url));
            var tdtools = $(String.format('<td></td>'));
            var divtools = $('<div class="btn-group"></div>');
            tdtools.append(divtools);
            var addbutton = $('<button class="btn btn-primary btn-sm" data-toggle="modal" data-target="#myModal">添加子菜单</button>');
            var updatebutton = $('<button class="btn btn-warning btn-sm" data-toggle="modal" data-target="#myModal">修改</button>');
            var deletebutton = $('<button class="btn btn-danger btn-sm" >删除</button>');
            addbutton.click(function () {
                clearInput();
                $("#op").val("add");
                $("#level").val(v.level+1);
                $("#functionno_parentno").val(v.no);
            })
            updatebutton.click(function () {
                clearInput();
                $("#functionno_seqno").attr("readonly", "readonly");

                $("#op").val("update");
                $("#level").val(v.level);
                if (parentNode) {
                    $("#functionno_parentno").val(parentNode.no);
                    $("#functionno_seqno").val(v.no.replaceAll(parentNode.no, ""));
                } else {
                    $("#functionno_seqno").val(v.no);
                }
               
                $("#functionname").val(v.text);
                $("#functionurl").val(v.url);
                $("#remark").val(v.remark);
            })
            deletebutton.click(function () {
                if ($$.Message.ShowConfirm({
                    msg: "确定删除该功能？（注意：删除该功能会导致其下所有子功能都被删除！）",
                    ok: {
                        text: "确定",
                        click: function (target) {
                            $$.Message.Close();
                            $$.Net.Ajax("/function/delete", "post", {
                                no: v.no
                            }).then(function (rtn) {
                                if (rtn.issuccess) {
                                    loadTree();
                                }
                                $$.Message.ShowMsg(rtn.msg);
                            })
                        }
                    }
                }));
            })
            divtools.append(addbutton);
            divtools.append(updatebutton);
            divtools.append(deletebutton);

            tr.append(tdName);
            tr.append(tdNo);
            tr.append(tdUrl);
            tr.append(tdtools);

            tree.append(tr);
            if (v.nodes) {
                builderTree(v.nodes, currentIndex, v);
            }

        })
    }

    function clearInput() {
        $("#level").val("");
        $("#functionno_parentno").val("");
        $("#functionno_seqno").val("");
        $("#functionname").val("");
        $("#functionurl").val("");
        $("#remark").val("");

        $("#functionno_seqno").removeAttr("readonly");
    }

    loadTree();
}(jQuery, effc))
