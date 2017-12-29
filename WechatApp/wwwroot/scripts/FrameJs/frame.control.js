; (function ($,fns) {
    fns.Control = {
        GenImage : function (id, src) {
            var rtn = "<img id='" + id + "' name='" + id + "' src='" + src + "' />";
            return rtn;
        },
        GenValidationLabel : function (id, text) {
            var rtn = "<label id='" + id + "' name='" + id + "' style='color:red'>" + text + "</label>";
            return rtn;
        },
        GenOption : function (value, text) {
            rtn = "<option value='" + value + "'>" + text + "</option>";
            return rtn;
        },
        ///智能绑定对象事件，动态监听父层控件状态，并逐层搜索目标控件并绑定事件
        ///selector支持以下格式：a b c这样的层级搜索，a，b，c的格式：#表示根据id选择，.表示根据class进行选择，名字则表示根据标签来选择
        ///b可以为a的子孙节点，比如，a为div，a的子节点为ul，b为ul下的li，则可以直接用“div li”来做选择路径
        ///eventtype为事件名称,为effc.EventType类型
        ///func(target,index)为操作事件，参数为target-目标对象,index-为对象所在数组下标
        Smartbind: function (selector,eventtype, func) {
            var target = null;
            var tstr = "";
            //
            if (!doExce(selector, eventtype, func)) return false;

            var MutationObserver = window.MutationObserver ||
                        window.WebKitMutationObserver ||
                        window.MozMutationObserver;

            var strarr = selector.split(" ");
            var article = document.querySelector(strarr[0]);
            if (!article) article = document.querySelector("html");
            if (MutationObserver) {
                var options = {
                    'childList': true,
                    "subtree": true
                };
                var observer = new MutationObserver(function (records) {
                    doExce(selector, eventtype, func);
                })

                observer.observe(article, options);
            } else {
                //IE的DOM效能极差，不能使用
                if (fns.BrowserMatch.browser === "IE") {
                    fns.ExceptionProcess.Process("Your browser not support SmartBind!", "Frame.Control.Smartbind");
                    return;
                }
                fns.bind(article, "DOMNodeInserted", function (e) {
                    doExce(selector, eventtype, func);
                })
            }
        }
    }

    function doExce(selector, eventtype, func) {
        if (typeof selector != "string") return false;
        if (func) {
            var target = document.querySelectorAll(selector);
            if (target && target.length > 0) {
                for (var i = 0; i < target.length;i++) {
                    fns.bind(target[i],eventtype,function () {
                        func(this, i);
                    });
                }
            }
        }
        return true;
    }
}(jQuery, FrameNameSpace));
