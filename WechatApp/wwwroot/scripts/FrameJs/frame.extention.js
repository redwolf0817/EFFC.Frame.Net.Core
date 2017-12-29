(function (fns) {
    var _sleeptimer;
    fns.ext = {}
    fns.sleep = function (ms2sleep, callback) {
        fns.sleep._sleeptimer = ms2sleep;
        fns.sleep._cback = callback;
        fns.sleep.timer = setTimeout('effc.sleep.count()', ms2sleep);//setInterval('$.sleep.count()', 1000);
    }
    fns.extend(fns.sleep, {
        current_i: 1,
        _sleeptimer: 0,
        _cback: null,
        timer: null,
        count: function () {
            //if ($.sleep.current_i >= $.sleep._sleeptimer) {
            //    clearInterval($.sleep.timer);
            //    $.sleep._cback.call(this);
            //}
            //$.sleep.current_i++;

            clearInterval(fns.sleep.timer);
            fns.sleep._cback.call(this);
        }
    });

    fns.url = {
        encode: function (str) {
            return encodeURIComponent(str);
        },
        decode: function (str) {
            return decodeURIComponent(str);
        }
    }

    fns.url.p = getQueryString();
    fns.QueryString = fns.url.p;
    //获取QueryString的数组
    function getQueryString() {
        var result = location.search.match(new RegExp("[\?\&][^\?\&]+=[^\?\&]+", "g"));
        var rtn = {};
        if (result == null) {
            return rtn;
        }

        for (var i = 0; i < result.length; i++) {
            var arr = result[i].substring(1).split("=");
            rtn[arr[0]] = arr[1];
        }
        return rtn;
    }

    fns.BrowserMatch = {
        init: function () {
            this.browser = this.getBrowser().browser || "An Unknown Browser";
            this.version = this.getBrowser().version || "An Unknown Version";
            this.OS = this.getOS() || "An Unknown OS";
        },
        getOS: function () {
            if (navigator.platform.indexOf("Win") != -1) return "Windows";
            if (navigator.platform.indexOf("Mac") != -1) return "Mac";
            if (navigator.platform.indexOf("Linux") != -1) return "Linux";
            if (navigator.userAgent.indexOf("iPhone") != -1) return "iPhone/iPod";
        },
        getBrowser: function () {
            var rMsie = /(msie\s|trident\/7)([\w\.]+)/;
            var rTrident = /(trident)\/([\w.]+)/;
            var rFirefox = /(firefox)\/([\w.]+)/;
            var rOpera = /(opera).+version\/([\w.]+)/;
            var rNewOpera = /(opr)\/(.+)/;
            var rChrome = /(chrome)\/([\w.]+)/;
            var rSafari = /version\/([\w.]+).*(safari)/;
            var ua = navigator.userAgent.toLowerCase();
            var matchBS, matchBS2;
            matchBS = rMsie.exec(ua);
            if (matchBS != null) {
                matchBS2 = rTrident.exec(ua);
                if (matchBS2 != null) {
                    switch (matchBS2[2]) {
                        case "4.0": return { browser: "IE", version: "8" }; break;
                        case "5.0": return { browser: "IE", version: "9" }; break;
                        case "6.0": return { browser: "IE", version: "10" }; break;
                        case "7.0": return { browser: "IE", version: "11" }; break;
                        default: return { browser: "IE", version: "Undefined" };
                    }
                } else {
                    return { browser: "IE", version: matchBS[2] || "0" };
                }
            }
            matchBS = rFirefox.exec(ua);
            if ((matchBS != null) && (!(window.attachEvent)) && (!(window.chrome)) && (!(window.opera))) {
                return { browser: matchBS[1] || "", version: matchBS[2] || "0" };
            }
            matchBS = rOpera.exec(ua);
            if ((matchBS != null) && (!(window.attachEvent))) {
                return { browser: matchBS[1] || "", version: matchBS[2] || "0" };
            }
            matchBS = rChrome.exec(ua);
            if ((matchBS != null) && (!!(window.chrome)) && (!(window.attachEvent))) {
                matchBS2 = rNewOpera.exec(ua);
                if (matchBS2 == null) {
                    return { browser: matchBS[1] || "", version: matchBS[2] || "0" };
                } else {
                    return { browser: "Opera", version: matchBS2[2] || "0" };
                }
            }
            matchBS = rSafari.exec(ua);
            if ((matchBS != null) && (!(window.attachEvent)) && (!(window.chrome)) && (!(window.opera))) {
                return { browser: matchBS[2] || "", version: matchBS[1] || "0" };
            }
        }
    };
    fns.BrowserMatch.init();
    fns.b = fns.BrowserMatch;

    /*
    base64
    */
    fns.base64 = function (options) {
        var defaults = {
            data: "",
            type: 0,
            utf16: true
        };
        var opts = fns.extend(defaults, options);
        var keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        //将Ansi编码的字符串进行Base64编码
        encode64 = function (input) {
            var output = "";
            var chr1, chr2, chr3 = "";
            var enc1, enc2, enc3, enc4 = "";
            var i = 0;
            do {
                chr1 = input.charCodeAt(i++);
                chr2 = input.charCodeAt(i++);
                chr3 = input.charCodeAt(i++);
                enc1 = chr1 >> 2;
                enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
                enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
                enc4 = chr3 & 63;
                if (isNaN(chr2)) {
                    enc3 = enc4 = 64;
                } else if (isNaN(chr3)) {
                    enc4 = 64;
                }
                output = output + keyStr.charAt(enc1) + keyStr.charAt(enc2)
                + keyStr.charAt(enc3) + keyStr.charAt(enc4);
                chr1 = chr2 = chr3 = "";
                enc1 = enc2 = enc3 = enc4 = "";
            } while (i < input.length);
            return output;
        }
        //将Base64编码字符串转换成Ansi编码的字符串
        decode64 = function (input) {
            var output = "";
            var chr1, chr2, chr3 = "";
            var enc1, enc2, enc3, enc4 = "";
            var i = 0;
            if (input.length % 4 != 0) {
                return "";
            }
            var base64test = /[^A-Za-z0-9\+\/\=]/g;
            if (base64test.exec(input)) {
                return "";
            }
            do {
                enc1 = keyStr.indexOf(input.charAt(i++));
                enc2 = keyStr.indexOf(input.charAt(i++));
                enc3 = keyStr.indexOf(input.charAt(i++));
                enc4 = keyStr.indexOf(input.charAt(i++));
                chr1 = (enc1 << 2) | (enc2 >> 4);
                chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                chr3 = ((enc3 & 3) << 6) | enc4;
                output = output + String.fromCharCode(chr1);
                if (enc3 != 64) {
                    output += String.fromCharCode(chr2);
                }
                if (enc4 != 64) {
                    output += String.fromCharCode(chr3);
                }
                chr1 = chr2 = chr3 = "";
                enc1 = enc2 = enc3 = enc4 = "";
            } while (i < input.length);
            return output;
        }
        utf16to8 = function (str) {
            var out, i, len, c;
            out = "";
            len = str.length;
            for (i = 0; i < len; i++) {
                c = str.charCodeAt(i);
                if ((c >= 0x0001) && (c <= 0x007F)) {
                    out += str.charAt(i);
                } else if (c > 0x07FF) {
                    out += String.fromCharCode(0xE0 | ((c >> 12) & 0x0F));
                    out += String.fromCharCode(0x80 | ((c >> 6) & 0x3F));
                    out += String.fromCharCode(0x80 | ((c >> 0) & 0x3F));
                } else {
                    out += String.fromCharCode(0xC0 | ((c >> 6) & 0x1F));
                    out += String.fromCharCode(0x80 | ((c >> 0) & 0x3F));
                }
            }
            return out;
        }
        utf8to16 = function (str) {
            var out, i, len, c;
            var char2, char3;
            out = "";
            len = str.length;
            i = 0;
            while (i < len) {
                c = str.charCodeAt(i++);
                switch (c >> 4) {
                    case 0: case 1: case 2: case 3: case 4: case 5: case 6: case 7:
                        // 0xxxxxxx
                        out += str.charAt(i - 1);
                        break;
                    case 12: case 13:
                        // 110x xxxx 10xx xxxx
                        char2 = str.charCodeAt(i++);
                        out += String.fromCharCode(((c & 0x1F) << 6) | (char2 & 0x3F));
                        break;
                    case 14:
                        // 1110 xxxx 10xx xxxx 10xx xxxx
                        char2 = str.charCodeAt(i++);
                        char3 = str.charCodeAt(i++);
                        out += String.fromCharCode(((c & 0x0F) << 12) |
                        ((char2 & 0x3F) << 6) |
                        ((char3 & 0x3F) << 0));
                        break;
                }
            }
            return out;
        }

        if (opts.data == "") {
            return "";
        } else {
            if (opts.type == 0) {
                if (opts.utf16 == true) {
                    return encode64(utf16to8(opts.data));
                } else {
                    return encode64(opts.data);
                }
            } else {
                if (opts.utf16 == true) {
                    return utf8to16(decode64(opts.data));
                } else {
                    return decode64(opts.data);
                }
            }
        }


    }

    fns.base64Encode = function (input) {
        return fns.base64({
            data: input,
            type: 0
        })
    }
    fns.base64Decode = function (input) {
        return fns.base64({
            data: input.replaceAll(" ", "+"),
            type: 1
        })
    }

    fns.IsMobileFormat=function(str){
        var re = /^1\d{10}$/
        if (re.test(str)) {
            return true;
        } else {
            return false;
        }
    }

    fns.IsPhoneFormat=function(str){
        var
            re = /^0\d{2,3}-?\d{7,8}$/;
        if (re.test(str)) {
            return true;
        } else {
            return false;
        }
    }

    fns.IsEmailFormat = function (str) {
        var
            re = /^(\w-*\.*)+@(\w-?)+(\.\w{2,})+$/;
        if (re.test(str)) {
            return true;
        } else {
            return false;
        }
    }
})(FrameNameSpace);