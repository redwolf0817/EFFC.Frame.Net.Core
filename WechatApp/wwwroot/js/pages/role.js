; (function ($, $$) {
    //准备一个空的实例对象
    var Event = new Vue();

    Vue.component('item', {
        template: '#item-template',
        props: {
            model: {
                type: Array,
                default: function () {
                    return [];
                }
            }
        },
        data: function () {
            return {
                checkedarr:[],
                checkall:false
            }
        },
        watch: {//深度 watcher
            model: {
                handler: function (val, oldVal) {
                    this.checkedarr = [];
                    for (var i = 0; i < val.length; i++) {
                        if (val[i].ischecked) {
                            this.checkedarr.push(val[i].no);
                        }
                    }
                }
            },
            checkall: {
                handler: function (val, oldVal) {
                    this.checkedarr = [];
                    if (val) {
                        for (var i = 0; i < this.model.length; i++) {
                            this.checkedarr.push(this.model[i].no);
                        }
                    }
                }
            },
            checkedarr: {
                handler: function (val, oldVal) {
                    Event.$emit("checkarr", val);
                }
            }
        },
        updated: function () {
            $(".tree").treegrid({
                expanderExpandedClass: 'glyphicon glyphicon-minus',
                expanderCollapsedClass: 'glyphicon glyphicon-plus'
            });
        },
        computed: {
        },
        methods: {
            classObject: function (currentIndex, parentIndex) {
                return "treegrid-" + currentIndex + (parentIndex == 0 ? "" : " treegrid-parent-" + parentIndex);
            },
            checkme: function (item) {
                var ischecked = event.target.checked;
                //处理子节点
                var childstartindex = item.index;
                for (var i = childstartindex; i < this.model.length; i++) {
                    if (this.model[i].parentno == item.no) {
                        if (ischecked) {
                            this.checkedarr.setValue(this.model[i].no);
                        } else {
                            this.checkedarr.removeValue(this.model[i].no);
                        }
                        this.checkme(this.model[i]);
                    }
                }
                //处理父节点
                this.checkparent(item);
            },
            checkparent: function (item) {
                if (item.parentindex == 0) return;
                var ischecked = event.target.checked;
                var parent = this.model[item.parentindex - 1];
                if (ischecked) {
                    this.checkedarr.setValue(parent.no);
                    this.checkparent(parent);
                } else {
                    //遍历子节点
                    for (var i = parent.index; i < this.model.length; i++) {
                        if (parent.no == this.model[i].parentno && this.checkedarr.indexOfEx(this.model[i].no) > -1) {
                            return;
                        }
                    }

                    this.checkedarr.removeValue(parent.no);
                    this.checkparent(parent);
                }
            }
        }
    })

    var vm = new Vue({
        el: '#container',
        data: {
            filterText: "",
            rows: [],
            isShowPN: false,
            to_page: 1,
            count_per_page: 5,
            current_page: 1,
            total_page: 0,
            total_rows: 0,
            op: "",
            roleName: "",
            isactive: true,
            remark: "",
            roleuid: "",
            tree: [],
            currentIndex: 1,
            parentIndex: 0,
            checkarr: []
        },
        mounted: function () {
            Event.$on("checkarr", function (arr) {
                this.checkarr = arr;
            }.bind(this));
        },
        computed: {
            middlePN: function () {
                var arr = [];
                for (var i = this.current_page - 2; i < this.current_page + 2; i++) {
                    arr.push(i);
                }
            }
        },
        methods: {
            search: function (e) {
                var self = this;
                $$.Net.Ajax("/role/list", "post", {
                    filter: self.filterText,
                    toPage: self.to_page,
                    Count_per_Page: self.count_per_page
                }).then(function (rtn) {
                    if (rtn.issuccess) {
                        self.rows = rtn.data;
                        self.to_page = rtn.to_page;
                        self.count_per_page = rtn.count_per_page;
                        self.total_page = rtn.total_page;
                        self.total_rows = rtn.total_row;
                        self.current_page = rtn.current_page;

                        if (self.total_page > 1) {
                            self.isShowPN = true;
                        }
                    } else {
                        $$.Message.ShowMsg(rtn.msg);
                    }
                })
            },
            add: function (e) {
                this.roleName = "";
                this.isactive = true;
                this.remark = "";
                this.roleuid = "";
                this.op = "add";
            },
            update: function (item) {
                if (item) {
                    this.roleName = item.rolename;
                    this.isactive = item.isactive == 1 ? true : false;
                    this.remark = item.remark;
                    this.roleuid = item.roleuid;
                    this.op = "update";
                }
            },
            save: function (e) {
                if (this.roleName == "") {
                    $$.Message.ShowMsg("请输入角色名称");
                    return false;
                }
                var self = this;
                var url = "/role/" + this.op;
                $$.Net.Ajax(url, "post", {
                    rolename: self.roleName,
                    isactive: self.isactive,
                    remark: self.remark,
                    roleuid: self.roleuid
                }).then(function (rtn) {
                    if (rtn.issuccess) {
                        self.search();
                    }
                    $$.Message.ShowMsg(rtn.msg);
                    $("#close").click();
                })
            },
            doDelete: function (roleuid) {
                var self = this;
                $$.Message.ShowConfirm({
                    msg: "确定删除该角色？",
                    ok: {
                        text: "确定",
                        click: function (target) {
                            $$.Message.Close();
                            $$.Net.Ajax("/role/delete", "post", {
                                roleuid: roleuid
                            }).then(function (rtn) {
                                if (rtn.issuccess) {
                                    self.search();
                                }
                                $$.Message.ShowMsg(rtn.msg);
                            })
                        }
                    }
                })
            },
            toPage: function (topage) {
                if (this.to_page == topage) return;
                if (topage < 1) return;
                if (topage > this.total_page) return;
                this.to_page = topage;
                this.search();
            },
            loadtree: function (roleuid) {
                var self = this;
                self.roleuid = roleuid;
                $$.Net.Ajax("/role/tree", "post", {
                    roleuid: roleuid
                }).then(function (rtn) {
                    if (rtn.issuccess) {
                        self.tree = rtn.tree;
                       
                    } else {
                        $$.Message.ShowMsg(rtn.msg);
                        event.preventDefault();
                    }
                })
            },
            saveMap: function () {
                var self = this;
                $$.Net.Ajax("/role/map", "post", {
                    roleuid: self.roleuid,
                    functions: this.checkarr
                }).then(function (rtn) {
                    $$.Message.ShowMsg(rtn.msg);
                    if (rtn.issuccess) {
                        $("#close2").click();
                    }
                })
            }
        }
    })

    vm.search();
   
})(jQuery,effc)