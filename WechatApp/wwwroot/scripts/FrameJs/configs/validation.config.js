;(function($,fns){
    fns.Config.Validation =
        {
            ShowMsg: function (sender, msg) {
                this.RemoveMsg(sender);
                if (msg != "") {
                    var divid = "__validation_div_" + $(sender).attr("id") + "__";
                    var divobj = $("<span id='" + divid + "' for='validation' style=\"color:#0086c6;display:inline;\"></span>");
                    divobj.html("" + msg + "");
                    $(sender).after(divobj);
                }
            },
            RemoveMsg: function (sender) {
                var divid = "__validation_div_" + $(sender).attr("id") + "__";
                $("#" + divid).remove();
            },
            InProcessing: function (sender) {
                this.RemoveProcessing(sender);

                if (typeof fns.Control == "undefined") {
                    document.write("<script language=javascript src='/FrameJs/configs/jquery.control.js'></script>");
                }

                var validobj = $(sender);
                var inputid = validobj.attr("id");
                var divid = "__validation_div_" + $(sender).attr("id") + "__";
                var divobj = $("<span id='" + divid + "' for='validation'></span>");
                validobj.after(divobj);
                divobj.html(fns.Control.GenImage(inputid + "_checkbusy", "/Theme/images/busy.gif") + "Checking");
            },
            RemoveProcessing: function (sender) {
                var divid = "__validation_div_" + $(sender).attr("id") + "__";
                $("#" + divid).remove();
            }
        }
}(jQuery,FrameNameSpace));
