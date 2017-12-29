(function ($,fns) {
    String.prototype.replaceAll = function (s1, s2) {
        return this.replace(new RegExp(s1, "gm"), s2);
    }
    var methods, tree;
    Node = (function () {
        function Node(nid, pid, id) {
            //nodeid
            this.nid = nid;
            //parentid
            this.pid = pid;
            //控件id
            this.id = id;
            //控件
            this.row = null;
            if (id == null) {
                this.id = this.nid;
            }
            this.id = id;
            this.childNodes = [];
            this.childDics = {};
            this.parentNode = null;
            this.level = -1;
            //level
            this.Level = function () {
                return this.level;
            }
        }
        Node.prototype.AddChild = function (childnode) {
            var self = this;
            this.childNodes.push(childnode);
            this.childDics[childnode.nid] = childnode;
            childnode.parentNode = self;

            return this;
        }
        Node.prototype.ChildNodes = function () {
            return this.childNodes;
        }

        Node.prototype.HasChild = function () {
            return this.childNodes.length > 0;
        }

        Node.prototype.IsLeaf = function () {
            return !this.HasChild();
        }
        //通過nid獲得子節點
        Node.prototype.ChildNode = function (nid) {
            return this.childDics[nid];
        }
        //通過下標獲得子節點
        Node.prototype.ChildNodeAt = function (index) {
            return this.childNodes[index];
        }

        Node.prototype.EachCHild = function( callback, args ) {
            return $.each( this.childNodes, callback, args );
        }
        
        return Node;
    })();
    
    Tree = (function () {
        function Tree(table, settings) {
            this.table = table;
            this.settings = settings;

            this.rowTag = this.settings.rowTag;
            this.nodeidname = this.settings.nodeIdAttr;
            this.pidname = this.settings.parentIdAttr;
            this.rootid = this.settings.rootId;
            this.initialState = this.settings.initialState;
            this.subnodeOffset = this.settings.subnodeOffset;

            this.openTemplate = this.settings.openTemplate;
            this.closeTempate = this.settings.closeTempate;
            this.leafTemplate = this.settings.leafTemplate;

            this.onExpandState = this.settings.onExpandState;
            this.onCollapseState = this.settings.onCollapseState;
            this.onStateChange = this.settings.onStateChange;
            this.onLoad = this.settings.onLoad;
            this.onLoadComplete = this.settings.onLoadComplete;
            //node的Dictionary，id為key
            this.nodedics = {};
            //node的array
            this.arrnodes = [];
            //root node
            this.rootnode = null;
            //tree deep
            this.treedeep = 0;
            //deep constructure
            this.deepdics = [];
        };

        Tree.prototype._buildtree = function () {
            var self = this;
            var rootobj = $("#" + self.rootid);
            if (rootobj == null) return;

            self.rootnode = new Node(rootobj.attr(self.nodeidname), "", self.rootid);
            self.rootnode.level = 0;
            self.rootnode.row = $("#" + self.rootid);
            //遍歷找出所有的節點，并構建dics和array
            $(this.table).find(this.rowTag + "[" + self.nodeidname + "]").each(function (i, val) {
                var obj = $(this);
                //排除root
                if (obj.attr("id") == self.rootid) return;

                var nid = obj.attr(self.nodeidname);
                var pid = obj.attr(self.pidname);
                var id = new String(obj.attr("id")).replaceAll(" ", "_");
                if (id == null || typeof (id) == "undefined" || id == "undefined") {
                    id = nid.replaceAll(" ", "_");
                    obj.attr("id", id);
                }

                var node = new Node(nid, pid, id);
                node.row = obj;
                obj.remove();
                //添加節點dics
                self.nodedics[nid] = node;
                //添加節點array
                self.arrnodes[self.arrnodes.length] = node;
                //root 添加child
                if (pid == self.rootnode.nid) {
                    self.rootnode.AddChild(node);
                }
            });
            //遍歷array，構建tree關係
            for (var key in self.arrnodes) {
                var n = self.arrnodes[key];
                var pid = n.pid;
                var nid = n.nid;
                var id = n.id;
                
                if (pid != self.rootnode.nid) {
                    self.nodedics[pid].AddChild(n);
                }
            }
            //算出level,重構table
            self._levelc(self.rootnode);
            //算出deep
            for (var key in self.arrnodes) {
                var n = self.arrnodes[key];
                if (n.Level() > self.treedeep) {
                    self.treedeep = n.Level();
                }
            }
            

        }

        Tree.prototype._levelc = function (node) {
            var self = this;
            if (node.pid != "" && node.pid != null) {
                node.level = node.parentNode.level + 1;
            }
            var prenode = node;
            node.EachCHild(function (i, val) {
                this.row.insertAfter(prenode.row);
                prenode = this;
            });

            node.EachCHild(function (i, val) {
                self._levelc(this);
            });
        }

        Tree.prototype.Load = function () {
            var self = this;
            self._buildtree();
            if (self.arrnodes.length > 0) {
                if (self.initialState == "Expand") {
                    self.ExpandAll();
                } else {
                    self.CollapseAll();
                }
            }
            //onload
            if (self.onLoad != null) {
                //jsonobj{obj,parentobj,childobjs,nid,pid,isRoot,isLeaf}
                //root
                var jsonobj = {};
                jsonobj.obj = $("#" + self.rootid);
                jsonobj.parentobj = null;
                jsonobj.childobjs = [];
                self.rootnode.EachCHild(function (i, val) {
                    jsonobj.childobjs[i] = $(this.id);
                });
                jsonobj.nid = self.rootnode.nid;
                jsonobj.pid = self.rootnode.pid;
                jsonobj.isRoot = true;
                jsonobj.isLeaf = self.rootnode.IsLeaf();
                self.onLoad(jsonobj);
                //other nodes
                for (var key in self.arrnodes) {
                    var n = self.arrnodes[key];

                    jsonobj = {};
                    jsonobj.obj = $("#" + n.id);
                    jsonobj.parentobj = $("#" + n.parentNode.id);
                    jsonobj.childobjs = [];
                    self.n.EachCHild(function (i, val) {
                        jsonobj.childobjs[i] = $(this.id);
                    });
                    jsonobj.nid = n.nid;
                    jsonobj.pid = n.pid;
                    jsonobj.isRoot = false;
                    jsonobj.isLeaf = n.IsLeaf();
                    self.onLoadComplete(jsonobj);
                }
            }
            //onloadcomplete
            if (self.onLoadComplete != null) {
                self.onLoadComplete(self);
            }
        };

        Tree.prototype.Node = function (nid) {
            if (nid == this.rootnode.nid) {
                return this.rootnode;
            } else {
                return this.nodedics[nid];
            }
        }

        Tree.prototype.RenderNode = function (nid, includeChild) {
            if (!includeChild) includeChild = true;

            var self = this;
            var node = self.Node(nid);
            var obj = $("#" + node.id);
            var c = obj.find("[treeclick='true']");

            if (node.id == self.rootid) {
                c.css("margin-left", "0px");
            } else {
                var parent = $("#" + node.parentNode.id);
                var parentOffset = parent.find("[treeclick='true']").css("margin-left");
                var offsetint = (parentOffset == null || parentOffset == "") ? parseInt(self.subnodeOffset) : (parseInt(String.prototype.replace.apply(parentOffset, ["px", ""])) + parseInt(self.subnodeOffset));
                var offset = offsetint + "px";

                c.css("margin-left", offset);
            }
            if (includeChild) {
                if (node.HasChild()) {
                    var children = node.ChildNodes();
                    for (var key in children) {
                        var n = children[key];
                        self.RenderNode(n.nid);
                    }
                }
            }
        };
        Tree.prototype._ExpandNode = function(nid){
            var self = this;
            var node = self.Node(nid);
            var curobj = $("#"+node.id);
            curobj.attr("childExpanded", "true");
            var isLeaf = node.IsLeaf();
            //狀態變化時的處理
            if (self.onStateChange != null) {
                var c = self.onStateChange(curobj, true, isLeaf);
                self.RenderNode(nid, false);
                if (!isLeaf) {
                    $(c).click(function () {
                        self.Collapse(nid);
                    });
                }
            }
        };
        Tree.prototype.Expand = function (nid) {
            var self = this;
            self._ExpandNode(nid);
            //子節點處理
            self.ExpandChild(nid);
        };
        
        Tree.prototype.ExpandChild = function (nid) {
            var self = this;
            var node = self.Node(nid);
            //子節點處理
            node.EachCHild(function (i, val) {
                var obj = $("#" + this.id);
                var isLeaf = this.IsLeaf();
                //展開動作顯示
                if (self.onExpandState) {
                    self.onExpandState(obj);
                }
                //狀態處理
                if (self.onStateChange != null) {
                    var c;
                    var cnid = this.nid;
                    if (obj.attr("childExpanded") != "false") {
                        c = self.onStateChange(obj, true, isLeaf);
                        
                        $(c).click(function () {
                            if (!isLeaf) {
                                self.Collapse(cnid);
                            }
                        });
                    } else {
                        c = self.onStateChange(obj, false, isLeaf);
                        $(c).click(function () {
                            if (!isLeaf) {
                                self.Expand(cnid);
                            }
                        });
                    }
                    self.RenderNode(nid, false);
                }
                //展開其下子節點，如果之前是展開的
                if (!isLeaf) {
                    if (obj.attr("childExpanded") != "false") {
                        self.ExpandChild(this.nid);
                    } else {
                        self.CollapseChild(this.nid);
                    }
                }
            });
        };
        Tree.prototype.ExpandToLevel = function (level) {
            var self = this;
            if (level <= 0) {
                self.CollapseAll();
            } else {
                self.ExpandAll();
                for (var i = level; i < self.treedeep + 1; i++) {
                    for (var key in self.arrnodes) {
                        var n = self.arrnodes[key];

                        if (n.Level() == i) {
                            self.Collapse(n.nid);
                        }
                    }
                }
            }
        }
        Tree.prototype.ExpandAll = function () {
            var self = this;
            $("#" + this.rootnode.id).attr("childExpanded", "true");
            for (var key in this.arrnodes) {
                var n = this.arrnodes[key];
                if (n.HasChild()) {
                    $("#" + n.id).attr("childExpanded", "true");
                }
            }
            self.Expand(this.rootnode.nid);
        };
        Tree.prototype._CollapseNode = function (nid) {
            var self = this;
            var node = self.Node(nid);
            var isLeaf = node.IsLeaf();
            var curobj = $("#" + node.id);
            curobj.attr("childExpanded", "false");

            //狀態變化時的處理
            if (self.onStateChange != null) {
                var c = self.onStateChange(curobj, false, isLeaf);
                if (!isLeaf) {
                    $(c).click(function () {
                        self.Expand(nid);
                    });
                }
                self.RenderNode(nid, false);
            }
        };
        Tree.prototype.Collapse = function (nid) {
            var self = this;
            self._CollapseNode(nid);
            //子節點處理
            self.CollapseChild(nid);
        };
        Tree.prototype.CollapseChild = function (nid) {
            var self = this;
            var node = self.Node(nid);
            //子節點處理
            node.EachCHild(function (i, val) {
                var obj = $("#" + this.id);

                var isLeaf = this.IsLeaf();
                //閉合動作處理
                if (self.onCollapseState != null) {
                    self.onCollapseState(obj);
                }
                //狀態處理
                if (self.onStateChange != null) {
                    var c;
                    var cnid = this.nid;
                    if (obj.attr("childExpanded") != "false") {
                        c = self.onStateChange(obj, true, isLeaf);
                        
                        $(c).click(function () {
                            if (!isLeaf) {
                                self.Collapse(cnid);
                            }
                        });
                    } else {
                        c = self.onStateChange(obj, false, isLeaf);
                        $(c).click(function () {
                            if (!isLeaf) {
                                self.Expand(cnid);
                            }
                        });
                    }
                    
                    self.RenderNode(this.nid, false);
                }
                //閉合其下子節點，如果之前是展開的
                self.CollapseChild(this.nid);
            });
        }
        Tree.prototype.CollapseToLevel = function (level) {
            var self = this;
            for (var key in self.arrnodes) {
                var n = self.arrnodes[key];

                if (n.n.Level() == level) {
                    self.Collapse(n.nid);
                }

            }
        }
        Tree.prototype.CollapseAll = function () {
            var self = this;
            $("#" + this.rootnode.id).attr("childExpanded", "false");
            for (var key in this.arrnodes) {
                var n = this.arrnodes[key];
                if (n.HasChild()) {
                    $("#" + n.id).attr("childExpanded", "false");
                }
            }
            self.Collapse(this.rootnode.nid);
        };

        Tree.prototype.HasChildNode=function(nid){
            var self = this;

            return self.Node(nid).HasChild();
        }
        

        return Tree;
    })();
    

    methods = {
        init: function (table,options) {
            var settings, tree, table;
            this.table = table;

            settings = $.extend({
                openTemplate: "<a href='#'>&nbsp;-&nbsp;</a>",//展開狀態下的控件模板
                closeTempate: "<a href='#'>&nbsp;+&nbsp;</a>",//閉合狀態下的控件模板
                leafTemplate: "<span></span>",//葉子節點的控件模板
                rowTag: "tr",//行數據的標籤類型
                nodeIdAttr: "nid",//節點ID的屬性名稱
                parentIdAttr: "pid",//父節點ID的屬性名稱
                rootId: "root",//root節點的id
                initialState: "collapsed",//初始狀態
                subnodeOffset: "10",//字符節點直接的位移量
                //event
                //展現展開的狀態的方法
                onExpandState: function (obj) {
                    if (obj) {
                        $(obj).show();//.css("display", "table-row");
                    }
                },
                //展現閉合的狀態的方法
                onCollapseState: function (obj) {
                    if (obj) {
                        $(obj).hide();//.css("display", "none");
                    }
                },
                //狀態變化時的處理方法
                onStateChange: function (obj, isExpanded, isLeaf) {
                    var self = this;
                    if (obj) {
                        var c = $(obj).find("[treeclick='true']");
                        var rep = $(self.openTemplate).attr("treeclick", "true");
                        if (isLeaf) {
                            rep = $(self.leafTemplate).attr("treeclick", "true");
                        } else {
                            if (isExpanded) {
                                rep = $(self.openTemplate).attr("treeclick", "true");
                            } else {
                                rep = $(self.closeTempate).attr("treeclick", "true");
                            }
                        }
                        c.replaceWith(rep);
                        return rep;
                    }
                },
                //初始化時的收尾處理
                onLoad: null, //function(jsonobj{obj,parentobj,childobjs,nid,pid})
                onLoadComplete: null //function(jqueryobj)
            }, options);

            var self = this;
            tree = new Tree(self.table, settings);
            tree.Load();
            $(this).data("treetable", tree);
            return this;
            
        },
        Expand: function (nid) {
            $(this).data("treetable").Expand(nid);
            return this;
        },
        ExpandAll: function () {
            $(this).data("treetable").ExpandAll();
            return this;
        },
        ExpandToLevel:function(level){
            $(this).data("treetable").ExpandToLevel(level);
            return this;
        },
        Collapse: function (nid) {
            $(this).data("treetable").Collapse(nid);
            return this;
        },
        CollapseAll: function () {
            $(this).data("treetable").CollapseAll();
            return this;
        },
        CollapseToLevel: function (level) {
            $(this).data("treetable").CollapseToLevel(level);
            return this;
        },
        Destory: function () {

        }
    }

    $.fn.treetable = function (options) {
        return methods.init(this,options);
    }
})(jQuery, FrameNameSpace);