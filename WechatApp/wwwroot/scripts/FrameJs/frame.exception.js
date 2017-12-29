; (function (fns) {
    fns.ExceptionProcess = {
        Process : function (error, sourcename) {
            if (error != null) {
                console.error("An error occured in \"" + sourcename + "\"!\n" + (error.message ? error.message : error));
                //fns.Message.Dialog({
                //    msg: "An error occured in \"" + sourcename + "\"!\n" + error.message
                //});
            }
        },
        ShowErrorMsg: function (msg) {
            if (msg != null) {
                fns.Message.Dialog({
                    msg: "This is a system error：\n" + msg
                });
            }
        }
    };
}(FrameNameSpace));

