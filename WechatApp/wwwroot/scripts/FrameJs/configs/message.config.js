; (function (fns) {
    fns.Config.Message =
        {
            Dialog: function (options) {
                //使用bootstrap
                Ewin.confirm({
                    message: options.msg,
                    buttons:options.buttons,
                    title: "提示框"
                })
                //使用jquery.easyui
                //if (options) {
                //    Alert(options.msg, options.buttons);
                //}
            },
            ConfirmMsg: function (options) {
                if (options && options.ok && options.cancel) {
                    //使用bootstrap
                    Ewin.confirm({
                        message: options.msg,
                        title: "提示框",
                        buttons: [
                            {
                                text: options.cancel.text,
                                click: options.cancel.click
                            },
                            {
                                text: options.ok.text,
                                click:options.ok.click
                            }
                        ]
                    })

                    //使用jquery.easyui
                    //var op = {
                    //    msg: options.msg,
                    //    buttons: [
                    //        {
                    //            text: options.ok.text,
                    //            click: options.ok.click
                    //        },
                    //        {
                    //            text: options.cancel.text,
                    //            click: options.cancel.click
                    //        }
                    //    ]
                    //}
                    //dialog(op.msg, "确认", op.buttons);
                }
            },
            ShowMsg: function (options) {
                if (options) {
                    //使用bootstrap
                    Ewin.alert(options.msg);

                    //使用jquery.easyui
                    //Alert(options.msg, options.buttons);
                }
            },
            Close: function () {
                Ewin.close();
            }
        }
        
}(FrameNameSpace));
