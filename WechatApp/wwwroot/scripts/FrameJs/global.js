///Frame的基础库
; (function () {
    var
    core_push = Array.prototype.push,
    core_slice = Array.prototype.slice,
    core_indexOf = Array.prototype.indexOf,
    core_toString = Object.prototype.toString,
    core_hasOwn = Object.prototype.hasOwnProperty,
    core_trim = String.prototype.trim,

    // Define EFFC
    EFFC = function (selector, context) {
        return new EFFC.fn.init(selector, context);
    },

    class2type = {};

    EFFC.fn = EFFC.prototype = {
        constructor: EFFC,
        selector: "",
        length: 0,
        size: function () {
            return this.length;
        },
        init: function (selector, context) {
            var args = arguments;
            if (args.length <= 0) {
                return this;
            } else {
                if (typeof selector === "string") {
                    this.selector = document.querySelectorAll(selector);
                    this.length = selector.length;
                } else {
                    this.selector = selector;
                    this.length = !this.selector.length ? 0 : this.selector.length;
                }

                return this;
            }
        }
    }
    EFFC.fn.init.prototype = EFFC.fn;

    EFFC.extend = EFFC.fn.extend = function () {
        var self = this;

        var options, name, src, copy, copyIsArray, clone,
            target = arguments[0] || {},
            i = 1,
            length = arguments.length,
            deep = false;

        // Handle a deep copy situation
        if (typeof target === "boolean") {
            deep = target;
            target = arguments[1] || {};
            // skip the boolean and the target
            i = 2;
        }

        // Handle case when target is a string or something (possible in deep copy)
        if (typeof target !== "object" && !self.isFunction(target)) {
            target = {};
        }

        // extend FrameJQ itself if only one argument is passed
        if (length === i) {
            target = this;
            --i;
        }

        for (; i < length; i++) {
            // Only deal with non-null/undefined values
            if ((options = arguments[i]) != null) {
                // Extend the base object
                for (name in options) {
                    src = target[name];
                    copy = options[name];

                    // Prevent never-ending loop
                    if (target === copy) {
                        continue;
                    }

                    // Recurse if we're merging plain objects or arrays
                    if (deep && copy && (self.isPlainObject(copy) || (copyIsArray = self.isArray(copy)))) {
                        if (copyIsArray) {
                            copyIsArray = false;
                            clone = src && self.isArray(src) ? src : [];

                        } else {
                            clone = src && self.isPlainObject(src) ? src : {};
                        }

                        // Never move original objects, clone them
                        target[name] = self.extend(deep, clone, copy);

                        // Don't bring in undefined values
                    } else if (copy !== undefined) {
                        target[name] = copy;
                    }
                }
            }
        }

        // Return the modified object
        return target;
    };

    EFFC.extend({
        isFunction: function (obj) {
            return this.type(obj) === "function";
        },

        isArray: Array.isArray || function (obj) {
            return this.type(obj) === "array";
        },

        isNumeric: function (obj) {
            return !isNaN(parseFloat(obj)) && isFinite(obj);
        },

        type: function (obj) {
            return obj == null ?
                String(obj) :
                class2type[core_toString.call(obj)] || "object";
        },

        isPlainObject: function (obj) {
            // Must be an Object.
            // Because of IE, we also have to check the presence of the constructor property.
            // Make sure that DOM nodes and window objects don't pass through, as well

            if (!obj || this.type(obj) !== "object" || obj.nodeType) {
                return false;
            }

            try {
                // Not own constructor property must be Object
                if (obj.constructor &&
                    !core_hasOwn.call(obj, "constructor") &&
                    !core_hasOwn.call(obj.constructor.prototype, "isPrototypeOf")) {
                    return false;
                }
            } catch (e) {
                // IE8,9 Will throw exceptions on certain host objects #9897
                return false;
            }

            // Own properties are enumerated firstly, so to speed up,
            // if last one is own, then all properties are own.

            var key;
            for (key in obj) { }

            return key === undefined || core_hasOwn.call(obj, key);
        },

        isEmptyObject: function (obj) {
            var name;
            for (name in obj) {
                return false;
            }
            return true;
        },
        bind: function (obj, eventtype, func) {
            if (!obj) return false;

            if (obj.addEventListener) {
                obj.addEventListener(eventtype, func);
                return true;
            } else if (obj.attachEvent) {
                var et = eventtype.indexOf("on") >= 0 ? eventtype : "on" + eventtype;
                obj.attachEvent(et, func);
                return true;
            } else {
                return false;
            }
        },
        isObjectValueEqual: function (a, b) {
            var re = a == b;
            if (re) return re;

            if (typeof a != "object" || typeof b != "object") return re;

            var aProps = Object.getOwnPropertyNames(a);
            var bProps = Object.getOwnPropertyNames(b);

            if (aProps.length != bProps.length) {
                return false;
            }

            for (var i = 0; i < aProps.length; i++) {
                var propName = aProps[i];
                if (Object.prototype.toString(a[propName]) == '[Object Object]' || Object.prototype.toString(b[propName]) == '[Object Object]') {
                    isObjectValueEqual(a[propName], b[propName])
                }
                if (a[propName] !== b[propName]) {
                    return false;
                }
            }
            return true;
        }
    })

    // Populate the class2type map
    var strarr = "Boolean Number String Function Array Date RegExp Object".split(" ");
    for (var i = 0; i < strarr.length;i++) {
        class2type["[object " + strarr[i] + "]"] = strarr[i].toLowerCase();
    }

    window.effc = EFFC;
    //提供对jQuery的扩展
    if (window.$) {
        $.Frame = EFFC;
    } 
}
)()

; (function () {
    effc.Config = {};
    FrameNameSpace = $$ = effc;
    FrameNameSpace.rootpath = "/scripts/FrameJs/";
    if (typeof js == "undefined") {
        document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "js.Util/js.lang.Class.min.js'></script>");
        document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "js.Util/js.events.EventDispatcher.min.js'></script>");
        document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "js.Util/js.util.ArrayList.min.js'></script>");
        document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "js.Util/js.util.Dictionary.min.js'></script>");
    }
    //if (typeof XML == "undefined") {
    //    document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "ObjTree.min.js'></script>");
    //}

    //if (typeof $.fn.ajaxSubmit == "undefined") {
    //    document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "jquery.form.min.js'></script>");
    //}

    if (typeof FrameNameSpace.ExceptionProcess == "undefined") {
        document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "frame.exception.min.js'></script>");
    }
    //if (typeof FrameNameSpace.Control == "undefined") {
    //    document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "frame.control.min.js'></script>");
    //}
    //if (typeof FrameNameSpace.Validation == "undefined") {
    //    document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "configs/validation.config.min.js'></script>");
    //    document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "frame.validation.min.js'></script>");
    //}
    //if (typeof FrameNameSpace.Ajax == "undefined") {
    //    document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "frame.ajax.js'></script>");
    //    document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "configs/ajax.config.js'></script>");
    //}
    if (typeof FrameNameSpace.Net == "undefined") {
        document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "configs/ajax.config.min.js'></script>");
        document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "frame.net.min.js'></script>");
    }

    if (typeof FrameNameSpace.Message == "undefined") {
        document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "configs/message.config.min.js'></script>");
        document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "frame.message.min.js'></script>");
    }

    //if (typeof FrameNameSpace.Form == "undefined") {
    //    document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "frame.form.min.js'></script>");
    //}
    if (typeof FrameNameSpace.Promise == "undefined") {
        document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "frame.promises.min.js'></script>");
    }
    if (typeof FrameNameSpace.ext == "undefined") {
        document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "frame.extention.min.js'></script>");
    }
    //FrameJQ ui
    //if (typeof $.ui == "undefined") {
    //    document.write("<script type='text/javascript' src='" + FrameNameSpace.rootpath + "jqueryui/jquery-ui-1.11.2.min.js'></script>");
    //    document.write("<link type='text/css' rel='stylesheet' href='" + FrameNameSpace.rootpath + "jqueryui/jquery-ui-1.11.2.min.css' />")
    //}
    //if (typeof $.blockUI == "undefined") {
    //    document.write("<script type='text/javascript' src='" + FrameNameSpace.rootpath + "jquery.blockUI.js'></script>");
    //}
    //dialog
    //if (typeof dialog == "undefined") {
    //    document.write("<script language=javascript src='/scripts/dialog.js'></script>");
    //}
    //bootstrap_dialog
    if (typeof EWin == "undefined") {
        document.write("<script language=javascript src='/scripts/bootstrap_dialog.min.js'></script>");
    }

    //if (typeof $.attrchange == "undefined") {
    //    document.write("<script language=javascript src='" + FrameNameSpace.rootpath + "attrchange.js'></script>");
    //}


    FrameNameSpace.ContentType = {
        Other: "application/x-www-form-urlencoded; charset=UTF-8",
        Json: "application/json; charset=UTF-8",
        Xml: "text/xml; charset=UTF-8",
        MultiPart: "multipart/form-data; charset=UTF-8"
    }

    FrameNameSpace.EventType = {
        Click: "click",
        DClick: "dblclick",
        Change: "change",
        MouseDown: "mousedown",
        MouseEnter: "mouseenter",
        MouseLeave: "mouseleave",
        MouseMove: "mousemove",
        MouseOver: "onmouseover",
        MouseOut: "mouseout",
        MouseUp: "mouseup"
    }

}());

String.format = function () {
    if (arguments.length == 0)
        return null;
    var str = arguments[0];
    for (var i = 1; i < arguments.length; i++) {
        var re = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
        str = str.replace(re, arguments[i]);
    }
    return str;
};

String.prototype.replaceAll = function (s1, s2) {
    return this.replace(new RegExp(s1, "gm"), s2);
}

String.prototype.startWith = function (str) {
    var reg = new RegExp("^" + str);
    return reg.test(this);
}

String.prototype.endWith = function (str) {
    var reg = new RegExp(str + "$");
    return reg.test(this);
}

JSON.TryParse = function (str) {
    try {
        return JSON.parse(str);
    } catch (ex) {
        return null;
    }
}

if (!Array.prototype.map) {
    ///数组的遍历方法
    Array.prototype.map = function (fun /*, thisp*/) {
        var len = this.length;
        if (typeof fun != "function")
            throw new TypeError();

        var res = new Array(len);
        var thisp = arguments[1];
        for (var i = 0; i < len; i++) {
            if (i in this)
                res[i] = fun.call(thisp, this[i], i, this);
        }

        return res;
    };
}
if (!Array.prototype.contains) {
    //数组的contains方法
    Array.prototype.contains = function (item) {
        //return RegExp("\\b" + item + "\\b").test(this);
        var i = this.length;
        while (i--) {
            if (this[i] === item) {
                return true;
            }
        }
        return false;
    };
}

if (!Array.prototype.indexOfEx) {
    Array.prototype.indexOfEx = function (val) {
        for (var i = 0; i < this.length; i++) {
            if (effc.isObjectValueEqual(this[i], val)) {
                return i;
            }
        }
        return -1;
    };
}
if (!Array.prototype.removeValue) {
    Array.prototype.removeValue = function (val) {
        var index = this.indexOfEx(val);
        if (index > -1) {
            this.splice(index, 1);
        }
    };
}
if (!Array.prototype.remove) {
    Array.prototype.remove =function remove(index) {
        return this.slice(0, index).concat(arr.slice(index + 1, arr.length))
    }
}
//数组的setValue方法，如果值存在则不处理，否则做插入操作
if (!Array.prototype.setValue) {
    Array.prototype.setValue = function remove(val) {
        if (this.indexOfEx(val) < 0) {
            this.push(val);
        }
    }
}

