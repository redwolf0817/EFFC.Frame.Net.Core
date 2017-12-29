
; (function ($, fns) {
    if (typeof fns.EventType == "undefined") {
        fns.EventType = {
            Click: 'click',
            Change: 'change',
            Blur: 'blur',
            Focus: 'focus',
            KeyDown: 'keydown',
            KeyUp: 'keyup',
            KeyPress: 'keypress',
            MouseDown: 'mousedown',
            MouseUp: 'mouseup',
            MouseOver: 'mouseover',
            MouseLeave: 'mouseleave'
        }

    }


    //原型定义
    FrameValidation = (function () {
        function ValidateStatusDto(obj, eventtype, category, isvalid, isprocess,checkfunc) {
            this.IsValid = isvalid;
            this.IsProcess = isprocess;
            this.obj = obj;
            this.eventtype = eventtype;
            this.category = category;
            var index = 0;
            this.checkfunc = new Array();
            this.checkfunc[index] = checkfunc;
        }

        function FrameValidation(options) {
            var defaults = {
                ShowMsg: null,//function (sender, msg) 
                RemoveMsg: null,//function (sender) 
                InProcessing: null,//function (sender)
                RemoveProcessing: null//function (sender)
            }
            this.opts = fns.extend(defaults, options);
            this.dics = new js.util.Dictionary();
            this.isvalidall = true;
        }
        FrameValidation.prototype.DefaultShowMsg = function (sender, msg) {
            var self = this;
            self.opts.ShowMsg(sender, msg);
        }
        FrameValidation.prototype.Register = function (reoptions) {
            var redefault = {
                //待检核控件id
                id: "",
                //所属类别
                category: "",
                //事件类型，click，change等
                event: fns.EventType.Click,
                //执行检核的自定义方法
                check: function (sender) { return true; },
                //检核成功后执行的方法
                success: null,//function();
                //检核失败或执行失败执行的方法
                fail: null,//function();
                //验证不通过时的提示信息
                msg: "",
                //信息显示方式，如果方法不为null则执行该方法，而忽略msg属性
                showmsg: null,//showmsg(sender, isvalid, msg);
                //移除显示信息
                removemsg: null//function(sender)
            }
            
            var opts = fns.extend(redefault, reoptions);
            if (opts.id != "" && $("#" + opts.id)) {
                var self = this;
                var validobj = $("#" + redefault.id);
               
                var checkfunc = function (e) {
                    if (self.CurrentState == "new")
                        self.dics.get(validobj.attr("id")).IsValid = true;

                    if (opts.removemsg) {
                        opts.removemsg(validobj);
                    } else {
                        self.opts.RemoveMsg(validobj);
                    }
                    if (!opts.check(validobj)) {
                        self.dics.get(validobj.attr("id")).IsValid = self.dics.get(validobj.attr("id")).IsValid & false;
                        self.isvalidall = self.isvalidall & false;
                        if (opts.showmsg != null) {
                            opts.showmsg(validobj, false, opts.msg);
                        } else {
                            self.DefaultShowMsg(validobj, opts.msg);
                        }

                        if (opts.fail) {
                            opts.fail();
                        }

                        if (e && e.target && e.target == this) {
                            //阻止后续事件的处理
                            e.stopImmediatePropagation();
                        }
                    } else {
                        if (opts.showmsg != null) {
                            opts.showmsg(validobj, true, opts.msg);
                        }
                        self.isvalidall = self.isvalidall & true;
                        self.dics.get(validobj.attr("id")).IsValid = self.dics.get(validobj.attr("id")).IsValid & true;

                        if (opts.success) {
                            opts.success();
                        }
                    }
                }

                
                switch (opts.event) {
                    case fns.EventType.Click:
                        validobj.click(checkfunc);
                        break;
                    case fns.EventType.Change:
                        validobj.change(checkfunc);
                        break;
                    case fns.EventType.Focus:
                        validobj.focus(checkfunc);
                        break;
                    case fns.EventType.Blur:
                        validobj.blur(checkfunc);
                        break;
                    case fns.EventType.MouseOver:
                        validobj.mouseover(checkfunc);
                        break;
                    case fns.EventType.MouseDown:
                        validobj.mouseup(checkfunc);
                        break;
                    case fns.EventType.MouseUp:
                        validobj.mouseup(checkfunc);
                        break;
                    case fns.EventType.MouseLeave:
                        validobj.mouseleave(checkfunc);
                        break;
                    case fns.EventType.KeyPress:
                        validobj.keypress(checkfunc);
                        break;
                    case fns.EventType.KeyUp:
                        validobj.keyup(checkfunc);
                        break;
                    case fns.EventType.KeyDown:
                        validobj.keydown(checkfunc);
                        break;
                    default:
                        break;
                }
                if (self.dics.containsKey(opts.id)) {
                    var index = self.dics.get(opts.id).checkfunc.length;
                    self.dics.get(opts.id).checkfunc[index] = checkfunc;
                } else {
                    self.dics.put(opts.id, new ValidateStatusDto(validobj, opts.event, opts.category, false, false, checkfunc));
                }
            }
        }

        FrameValidation.prototype.CurrentState = "new";

        FrameValidation.prototype.RegisterAjax = function (reoptions) {
            var redefault = {
                //待检核控件id
                id: "",
                //所属类别
                category: "",
                //事件类型，click，change等
                event: fns.EventType.Click,
                //执行检核的自定义方法
                check: function (sender, ajaxrtn) { return true; },
                //检核成功后执行的方法
                success: null,//function();
                //检核失败或执行失败执行的方法
                fail: null,//function();
                //提示信息
                msg: "",
                //信息显示方式，如果方法不为null则执行该方法，而忽略msg属性
                showmsg: null,//showmsg(sender, msg);
                //移除显示信息
                removemsg:null,//function(sender)
                //请求的url
                url: "",
                //回传的数据
                data: ""
            }
            var opts = fns.extend(redefault, reoptions);
            if (opts.id != "" && $("#" + opts.id)) {
                var self = this;

                var validobj = $("#" + opts.id);
                var inputid = opts.id;

                if (opts.removemsg) {
                    opts.removemsg(validobj);
                } else {
                    self.opts.RemoveMsg(validobj);
                }

                try {
                    var checkfunc = function () {
                        if (self.CurrentState == "new")
                            self.dics.get(inputid).IsValid = true;

                        if (opts.removemsg) {
                            opts.removemsg(validobj);
                        } else {
                            self.opts.RemoveMsg(validobj);
                        }
                        var postdata = {};
                        if (typeof opts.data === "function") {
                            postdata = opts.data();
                        } else {
                            postdata = opts.data;
                        }

                        fns.Ajax.Ajax({
                            url: opts.url,
                            postdata: postdata,
                            before:function(){
                                self.dics.get(inputid).IsProcess = true;
                                self.opts.InProcessing();
                            },
                            success: function (rtn) {
                                self.opts.RemoveProcessing();
                                self.dics.get(inputid).IsProcess = false;
                                if (opts.check == null) {
                                    fns.ExceptionProcess.ShowErrorMsg("The parameter \"check\" of AjaxValidator can't be null!");
                                    return;
                                }

                                if (!opts.check(validobj, rtn)) {
                                    self.dics.get(inputid).IsValid = self.dics.get(inputid).IsValid & false;
                                    if (opts.fail) {
                                        opts.fail();
                                    }
                                } else {
                                    self.dics.get(inputid).IsValid = self.dics.get(inputid).IsValid & true;
                                    if (opts.success) {
                                        opts.success();
                                    }
                                }

                                if (opts.showmsg != null) {
                                    opts.showmsg(validobj, self.dics.get(inputid).IsValid);
                                } else {
                                    if (!self.dics.get(inputid).IsValid) {
                                        self.opts.ShowMsg(validobj, opts.msg);
                                    }
                                }
                            },
                            complete: function () {
                                self.opts.RemoveProcessing();
                                self.dics.get(inputid).IsProcess = false;
                            },
                            error: function () {
                                self.opts.RemoveProcessing();
                                self.dics.get(inputid).IsProcess = false;
                                if (opts.fail) {
                                    opts.fail();
                                }
                            },
                            fail: function (errorcode, errormsg) {
                                if (opts.fail) {
                                    opts.fail();
                                }
                            }
                        });
                    }

                    switch (opts.event) {
                        case fns.EventType.Click:
                            validobj.click(checkfunc);
                            break;
                        case fns.EventType.Change:
                            validobj.change(checkfunc);
                            break;
                        case fns.EventType.Focus:
                            validobj.focus(checkfunc);
                            break;
                        case fns.EventType.Blur:
                            validobj.blur(checkfunc);
                            break;
                        case fns.EventType.MouseOver:
                            validobj.mouseover(checkfunc);
                            break;
                        case fns.EventType.MouseDown:
                            validobj.mouseup(checkfunc);
                            break;
                        case fns.EventType.MouseUp:
                            validobj.mouseup(checkfunc);
                            break;
                        case fns.EventType.MouseLeave:
                            validobj.mouseleave(checkfunc);
                            break;
                        case fns.EventType.KeyPress:
                            validobj.keypress(checkfunc);
                            break;
                        case fns.EventType.KeyUp:
                            validobj.keyup(checkfunc);
                            break;
                        case fns.EventType.KeyDown:
                            validobj.keydown(checkfunc);
                            break;
                        default:
                            break;
                    }
                    if (self.dics.containsKey(opts.id)) {
                        var index = self.dics.get(opts.id).checkfunc.length;
                        self.dics.get(opts.id).checkfunc[index] = checkfunc;
                    } else {
                        self.dics.put(opts.id, new ValidateStatusDto(validobj, opts.event, opts.category, false, false, checkfunc));
                    }
                } catch (error) {
                    fns.ExceptionProcess.Process(error, "AjaxValidator");
                } finally {

                }
            }
        }

        FrameValidation.prototype.IsValid = function (category) {
            var self = this;
            var rtn = true;
            for (var i = 0; i < self.dics.values().size() ; i++) {
                var key = self.dics.values().get(i);
                var item = self.dics.get(key);
                if (category == item.category || category == null || category == "") {
                    rtn = rtn & item.IsValid;
                }
                
            }
            return rtn;
        }

        FrameValidation.prototype.IsValidateProcess = function (category) {
            var self = this;
            var rtn = false;
            for (var i = 0; i < self.dics.values().size() ; i++) {
                var key = self.dics.values().get(i);
                var item = self.dics.get(key);
                if (category == item.category || category == null || category == "") {
                    if (item.IsProcess) {
                        rtn = true;
                        break;
                    }
                }
                
            }
            return rtn;
        }

        FrameValidation.prototype.ResetState = function () {
            var self = this;
            self.dics.iterate(function (k, v) {
                v.IsValid = true;
                v.IsProcess = false;
            });
        }

        FrameValidation.prototype.ValidAll = function (reoptions) {
            var self = this;
            self.CurrentState = "ischecking";
            var vadefaults = {
                category:"",
                //超时事件设定，单位为毫秒
                timeout: 200,
                //检核通过执行的事件
                success: function () { },
                //检核失败执行的事件
                fail: function () { },
                //检核超时执行的事件
                timeoutprocess: function () { }
            }
            var opts = fns.extend(vadefaults, reoptions);
            //先清除前一次的验证状态
            self.ResetState();

            self.dics.iterate(function (k, v) {
                if (opts.category == null || opts.category == "" || v.category == opts.category) {
                    for (var i=0; i < v.checkfunc.length; i++) {
                        v.checkfunc[i]();
                    }
                }
            });
            if (self.IsValidateProcess(opts.category)) {
                self.intervalid = setInterval(function () {
                    self.timeout = parseInt(opts.timeout / 100);
                    self.count = 0;
                    if (!self.IsValidateProcess()) {
                        if (self.IsValid(opts.category)) {
                            if (opts.success != null) {
                                opts.success();
                            }
                        } else {
                            if (opts.fail != null) {
                                opts.fail();
                            }
                        }
                        clearInterval(self.intervalid);
                        self.CurrentState = "new";
                    } else {
                        if (self.count >= self.timeout) {
                            clearInterval(self.intervalid);
                            if (opts.timeoutprocess != null) {
                                opts.timeoutprocess();
                            }
                        }
                    }
                }, opts.timeout);
            } else {
                if (self.IsValid(opts.category)) {
                    if (opts.success != null) {
                        opts.success();
                    }
                } else {
                    if (opts.fail != null) {
                        opts.fail();
                    }
                }
                self.CurrentState = "new";
            }

        }

        return FrameValidation;
    })();
    //封装
    _frame_valid_methods = {
        init: function (options) {
            var validation = new FrameValidation(options);
            $(this).data("validation", validation);
            return this;
        },
        Register: function (reoptions) {
            $(this).data("validation").Register(reoptions);
            return this;
        },
        RegisterAjax: function (reoptions) {
            $(this).data("validation").RegisterAjax(reoptions);
            return this;
        },
        //验证控件，默认为chang事件
        RegisterRequireInput: function (id, msg,category, event, showmsgfunc) {
            var options = {
                id: id,
                msg: msg,
                category:category,
                event: event == null ? fns.EventType.Change : event,
                check: function (obj) {
                    if ($(obj).val() == "") {
                        return false;
                    } else {
                        return true;
                    }
                },
                showmsg: showmsgfunc
            }
            this.Register(options);
            return this;
        },
        ValidAll: function (vaoptions) {
            $(this).data("validation").ValidAll(vaoptions);
            return this;
        }
    }

    fns.Validation = function (options) {
        
        var op = options;
        if (options == null)
            op = fns.Config.Validation;
        return _frame_valid_methods.init(op)
    };
}(jQuery, FrameNameSpace));