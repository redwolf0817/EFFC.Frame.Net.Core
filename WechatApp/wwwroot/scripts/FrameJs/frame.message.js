; (function ($, fns) {
    fns.Message = {}

    FrameMessage = (function(){
        function FrameMessage(options) {
            var defaults = {
                Dialog: null,//function (msg) 
                ConfirmMsg: null,//function (msg) 
                ShowMsg: null,//function(msg)
                Close:null//function()
            }
            this.opts = fns.extend(defaults, options);
        }
        FrameMessage.prototype.ShowMsg = function (msg) {
            var self = this;
            var opts = {
                msg:""
            }
            if (typeof msg === "string") {
                opts.msg = msg;
            } else {
                opts = fns.extend(opts, msg);
            }
            self.opts.ShowMsg(opts);
        }
        FrameMessage.prototype.ShowConfirm = function (msg) {
            var self = this;
            var opts = {
                msg: "",
                ok: {
                    text: "确定",
                    click: function (target) {
                        self.Close();
                    }
                },
                cancel: {
                    text: "取消",
                    click: function (target) {
                        self.Close();
                    }
                }
            }
            if (typeof msg === "string") {
                opts.msg = msg;
            } else {
                opts = fns.extend(opts, msg);
            }
            return self.opts.ConfirmMsg(opts);
        }
        FrameMessage.prototype.Dialog = function (reoptions) {
            var defaultoptions={
                msg:"",
                buttons: null//[{text:"",click:function()},{text:"",click:function()},...]
            }
            var opts = fns.extend(defaultoptions, reoptions);
            var self = this;
            return self.opts.Dialog(opts);
        }
        FrameMessage.prototype.Close = function () {
            var self = this;
            return self.opts.Close();
        }
        return FrameMessage;
    })();

    //封装
    _frame_msg_methods = {
        init: function (options) {
            var op = options;
            if (options == null)
                op = fns.Config.Message;

            var message = new FrameMessage(op);
            $(this).data("message", message);
            return this;
        },
        ShowMsg: function (msg) {
            $(this).data("message").ShowMsg(msg);
        },
        ShowConfirm: function (msg) {
            return $(this).data("message").ShowConfirm(msg);
        },
        Dialog: function (reoptions) {
            $(this).data("message").Dialog(reoptions);
        },
        Close: function () {
            $(this).data("message").Close();
        }
    }

    fns.Message = _frame_msg_methods.init();

    
}(jQuery, FrameNameSpace));

