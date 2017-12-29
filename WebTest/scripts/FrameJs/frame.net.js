/*
HttpRequest请求，提供Pormise写法
*/
define(["effc"], function (fns) {
    var asap = function (fn) {
        setTimeout(fn, 0);
    };
    fns.Net = {
        //参数任意，如果为一个参数则为json对象,
        //如果为多个参数，则顺序如下：arg0=url,arg1=method,arg2=data,arg3=return-type,arg4=request-headerdata,arg5=async
        Ajax: function () {
            var me = this;
            var args = arguments;
            if (!args || args.length <= 0) {
                fns.ExceptionProcess.Process("no arguments!", "Net.Ajax");
                return;
            }
            var options = {};

            if (args.length == 1) {
                if (typeof args[0] == "object") {
                    options = args[0];
                } else if (typeof args[0] == "string") {
                    options.url = args[0];
                } else {
                    fns.ExceptionProcess.Process("Invalid arguments!", "Net.Ajax");
                }

            } else {
                options.url = args[0];
                if (args.length > 1) options.method = args[1];
                if (args.length > 2) options.postdata = args[2];
                if (args.length > 3) options.returntype = args[3];
                if (args.length > 4) options.headerdata = args[4];
                if (args.length > 5) options.async = args[5];
            }

            var defaults = {
                url: "",
                method: "post",
                returntype: "json",
                postdata: {
                },
                headerdata: {
                    "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
                    "POWERED-BY-EFFC": "Approve",
                    "x-requested-with": "XMLHttpRequest"
                },
                async: true,
                progress: null //文件上传过程中的进度条的处理，function(index,total)
            }
            var opts = fns.extend(true, defaults, options);
            if (opts.async) {
                opts.headerdata["x-request-async"] = "true";
            }

            var request = this.xhr();
            return new Promise(function (resolve, reject) {
                try {
                    request.onreadystatechange = function () {
                        if (request.readyState === 4) {
                            if (request.status === 200) {
                                var repdata = FormatResponseData(request);

                                me.ResponseDataPreProcess(opts, repdata, resolve, reject);
                            } else {
                                reject({
                                    errorCode: request.status,
                                    errorMsg: ""
                                });
                            }
                        }
                    };

                    request.upload.addEventListener("progress", function (evt) {
                        if (opts.progress) {
                            if (evt.lengthComputable) {
                                opts.progress(evt.loaded, evt.total);
                            }
                        }
                    }, false);

                    request.open(opts.method, opts.url, opts.async);
                    //safari浏览器不支持xmlhttprequest level 2
                    if (fns.BrowserMatch.browser != "safari") {
                        request.responseType = opts.returntype;
                    }
                    if (opts.headerdata) {
                        for (var k in opts.headerdata) {
                            request.setRequestHeader(k, opts.headerdata[k]);
                        }
                    }

                    if (FrameNameSpace.Config.Ajax && FrameNameSpace.Config.Ajax.beforeAjax) {
                        if (FrameNameSpace.Config.Ajax.beforeAjax(opts)) {
                            request.send(GenPostData(opts.postdata, opts.method, opts.headerdata["Content-Type"]));
                        }
                    } else {
                        request.send(GenPostData(opts.postdata, opts.method, opts.headerdata["Content-Type"]));
                    }

                } catch (ex) {
                    fns.ExceptionProcess.Process(ex, "frame.Net.Ajax");
                    reject({
                        errorCode: ex.name,
                        errorMsg: ex.stack
                    })
                }
            });
        },
        xhr: function () {
            xmlhttp = null;
            if (window.XMLHttpRequest) {// code for all new browsers
                xmlhttp = new XMLHttpRequest();
            }
            else if (window.ActiveXObject) {// code for IE5 and IE6
                xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
            }

            return xmlhttp;
        },
        //参数任意，如果为一个参数则为json对象,
        //如果为多个参数，则顺序如下：arg0=url,arg1=onopen,arg2=onmessage,arg3=onerror,arg4=onclose
        WS: function () {
            var me = this;
            var args = arguments;
            if (!args || args.length <= 0) {
                fns.ExceptionProcess.Process("no arguments!", "Net.WS");
            }
            if (!WebSocket) {
                fns.ExceptionProcess.Process("Your browser not support WebSocket", "Net.WS");
            }

            var options = {};

            if (args.length == 1) {
                if (typeof args[0] == "object") {
                    options = args[0];
                } else if (typeof args[0] == "string") {
                    options.url = args[0];
                } else {
                    fns.ExceptionProcess.Process("Invalid arguments!", "Net.WS");
                }

            } else {
                options.url = args[0];
                if (args.length > 1) options.onopen = args[1];
                if (args.length > 2) options.onmessage = args[2];
                if (args.length > 3) options.onerror = args[3];
                if (args.length > 4) options.onclose = args[4];
            }

            var defaults = {
                url: "",
                onopen: null,//function(ws)
                onmessage: null,//function(ws,data)
                onerror: null,//function(ws,status,msg)
                onclose: null//function(ws)
            }
            var opts = fns.extend(true, defaults, options);



            var ws = null;
            if (opts.url.startWith("http://")) {
                ws = new WebSocket(opts.url.replaceAll("http", "ws"));
            } else if (opts.url.startWith("ws://")) {
                ws = new WebSocket(opts.url);
            } else {
                ws = new WebSocket('ws://' + window.location.hostname + (window.location.port == "" ? "" : ':' + window.location.port) + '/' + opts.url);
            }
            this.WebSocket = ws;

            //发送方法
            this.Send = function (obj) {
                var ws = this.WebSocket;
                if (ws) {
                    ws.send(JSON.stringify(obj), 20971520);
                }
            }
            this.Close = function () {
                var ws = this.WebSocket;
                if (ws) {
                    ws.close();
                }
            }
            this.IsClose = function () {
                var ws = this.WebSocket;
                if (ws) {
                    return ws.readyState == ws.CLOSED;
                }
            }
            //设定异步接受方法，event有四个，"open", "message", "close", "error"
            this.On = function (event, func) {
                if (typeof func != "function") throw TypeError("On need a function");

                if (event) {
                    var eindex = event.toLowerCase().startWith("on") ? event.toLowerCase() : "on" + event;
                    opts[eindex] = func;
                }

                return me;
            }

            asap(function () {
                try {
                    ws.onopen = function () {
                        if (opts.onopen) {
                            opts.onopen(me);
                        }
                    }
                    ws.onmessage = function (evt) {
                        if (Object.prototype.toString.call(evt.data) === "[object Blob]") {
                            if (opts.onmessage) {
                                opts.onmessage(me, evt.data);
                            }
                        } else {
                            eval("var jobj=" + evt.data);
                            if (jobj) {
                                if (jobj.ErrorCode == "") {
                                    if (opts.onmessage) {
                                        opts.onmessage(me, jobj.Content);
                                    }
                                } else {
                                    fns.ExceptionProcess.Process(jobj.ErrorCode + "\n" + jobj.ErrorMessage, "Net.WS");
                                    if (opts.onerror) {
                                        opts.onerror(me, jobj.ErrorCode, jobj.ErrorMessage);
                                    }
                                }
                            } else {
                                if (opts.onmessage) {
                                    opts.onmessage(me, evt.data);
                                }
                            }
                        }
                    }
                    ws.onerror = function (evt) {
                        fns.ExceptionProcess.Process(evt.message, "Net.WS");
                        if (opts.onerror) {
                            opts.onerror(me, "-1", evt.message);
                        }
                    }
                    ws.onclose = function () {
                        if (opts.onclose) {
                            opts.onclose();
                        }
                    }
                } catch (ex) {
                    fns.ExceptionProcess.Process(ex, "frame.Net.WS");
                    if (opts.onerror) {
                        opts.onerror(me, ex.name, ex.stack);
                    }
                }
            })


            return me;
        },
        //response回的数据进行预处理，可以重写
        //参数说明：repdata-返回的数据对象，resolve-promise的resolve(value)方法，reject-promise的reject(value)方法
        ResponseDataPreProcess: function (options, repdata, resolve, reject) {
            var obj = JSON.TryParse(repdata);
            if (!obj) obj = repdata;

            if (obj && obj.ErrorCode != null) {
                if (obj.ErrorCode == "") {
                    if (fns.Config.Ajax && fns.Config.Ajax.successAction) {
                        if (fns.Config.Ajax.successAction(options, obj)) {
                            resolve(obj.Content);
                        }
                    } else {
                        resolve(obj.Content);
                    }

                } else {
                    if (fns.Config.Ajax && fns.Config.Ajax.failAction) {
                        if (fns.Config.Ajax.failAction(options, obj.ErrorCode, obj.ErrorMessage)) {
                            reject({
                                errorCode: obj.ErrorCode,
                                errorMsg: obj.ErrorMessage
                            });
                        }
                    } else {
                        fns.ExceptionProcess.Process(obj.ErrorMessage, "Frame.Net.Ajax");
                        reject({
                            errorCode: obj.ErrorCode,
                            errorMsg: obj.ErrorMessage
                        });
                    }

                }
            } else {
                resolve(obj);
            }
        }
    }



    //将数据对象进行转换，提供给request发送出去
    function GenPostData(data, method, contenttype) {
        if (method.toLowerCase() == "get") {
            return null
        }

        if (contenttype.toLowerCase().indexOf("json") >= 0) {
            return JSON.stringify(data);
        } else if (contenttype.toLowerCase().indexOf("xml") >= 0) {
            return data;
        } else if (contenttype.toLowerCase().indexOf("multipart/form-data") >= 0) {
            var fd = new FormData();
            if (typeof data == "object") {
                for (var k in data) {
                    fd.append(k, data[k]);
                }
            } else {
                fd.append("data", data);
            }
            return fd;
        } else {
            var str = "";
            if (typeof data == "object") {
                for (var k in data) {
                    str += "&" + k + "=" + data[k];
                }
                str = str != "" ? str.substr(1) : "";
            } else {
                str = data;
            }

            return str;
        }
    }
    //将response的数据转化成数据对象
    function FormatResponseData(xhr) {
        if (xhr.responseType.toLowerCase() == "blob") {
            return xhr.response;
        } else {
            return xhr.response
        }
    }

    return fns.Net;
})