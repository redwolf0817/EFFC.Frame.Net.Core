/*
根據返回的frame json串進行判定
successfunc(contentobj):contentobj返回的json對象，FrameJson中的Content內容；返回值為bool
failedfunc(errorCode, errorMsg):errorCode錯誤代碼，errorMsg錯誤信息
*/
; (function ($,fns) {
    fns.Ajax = {
        Ajax: function (options) {
            var dtd = $.Deferred();
            var defaults = {
                url: "",
                returntype: "json",
                postdata: "",
                before: null, //function(XMLHttpRequest)
                success: null,//function(rtn)
                complete: null,//function (XMLHttpRequest, textStatus)
                fail: null,//function(errorcode,errormsg)
                async: true,
                xhr: null,//function() custom xhr
                contenttype: "application/x-www-form-urlencoded",
                processdata: true,
                crossDomain: true,
                preprocessresult: true //是否需要对结果做框架预处理
            }
            var opts = $.extend(defaults, options);
            if (fns.Config.Ajax) {
                if (fns.Config.Ajax.beforeAjax) {
                    fns.Config.Ajax.beforeAjax(opts);
                }
            }
            //json格式需要单独处理
            var datatype = opts.returntype == "json" ? "text" : opts.returntype;
            if (opts.url != "") {
                var p = {
                    type: "Post",
                    url: opts.url,
                    dataType: datatype,
                    cache: false,
                    data: opts.postdata,
                    async: opts.async,
                    processData: opts.processdata,
                    contentType: opts.contenttype,
                    beforeSend: function (XMLHttpRequest) {
                        if (opts.before != null) {
                            opts.before(XMLHttpRequest);
                        }
                    },
                    success: function (rtn) {
                        var jobj;
                        if (opts.preprocessresult) {
                            if (rtn != null) {
                                //判斷jsonstr是否就是json對象
                                //jobj = $.parseJSON(rtn);
                                if (jobj == null) {
                                    jobj = rtn;
                                }
                                if (opts.returntype.toLowerCase() == "json") {
                                    var bobj = eval("(" + jobj + ")");
                                    if (bobj.ErrorCode == "") {

                                        if (opts.success != null) {
                                            opts.success(bobj.Content);
                                        } else {
                                            dtd.resolve(bobj.Content);
                                        }
                                    } else {
                                        fns.ExceptionProcess.ShowErrorMsg(bobj.ErrorCode + "\n" + bobj.ErrorMessage);
                                        if (opts.fail) {
                                            opts.fail(bobj.ErrorCode, bobj.ErrorMessage);
                                        } else {
                                            dtd.reject(bobj.ErrorCode, bobj.ErrorMessage);
                                        }
                                    }
                                } else {
                                    if (opts.success != null) {
                                        opts.success(jobj);
                                    } else {
                                        dtd.resolve(jobj);
                                    }
                                }
                            }
                        } else {
                            if (opts.success != null) {
                                opts.success(jobj);
                            } else {
                                dtd.resolve(jobj);
                            }
                        }
                    },
                    complete: function (XMLHttpRequest, textStatus) {
                        completehandler(XMLHttpRequest, textStatus, opts);
                    },
                    error: function (jqXHR, errormsg, errorThrown) {
                        errorhandler(jqXHR, errormsg, errorThrown, opts);
                        dtd.reject(errormsg);
                    }
                }
                if (opts.xhr != null) {
                    p.xhr = function () {
                        if (opts.xhr != null) {
                            return opts.xhr();
                        } else {
                            return new XMLHttpRequest();
                        }
                    }
                }
                $.ajax(p);
            } else {
                fns.ExceptionProcess.ShowErrorMsg("需要提供url信息！");
            }
            return dtd.promise();
        },
        AjaxForm: function (options) {
            var dtd = $.Deferred();
            var defaults = {
                url: "",
                formid:"",
                returntype: "html",
                before: null,//bool function(XMLHttpRequest)，if false abort
                success: null,//function(jsonrtn)
                complete: null,//function(XMLHttpRequest, textStatus)
                fail: null//function(errorcode,errormsg)
            }
            var opts = $.extend(defaults, options);
            if (opts.formid == "") {
                fns.ExceptionProcess.ShowErrorMsg("需要提供formid信息！");
                return false;
            }
            if (opts.url == "") {
                fns.ExceptionProcess.ShowErrorMsg("需要提供url信息！");
                return false;
            }
            var __f = $("#" + opts.formid);
            //取消已经绑定的事件，防止事件重复性注册，并导致重复提交
            __f.unbind();

            var dt = opts.returntype == "" ? "html" : opts.returntype;
            var __op = {
                url: opts.url, type: "post", cache: false, datatype: dt,
                beforeSubmit: function (dataarray,obj) {
                    if (opts.befersend != null) {
                        opts.befersend(dataarray, obj);
                    }
                },
                success: function (rtn) {
                    if (opts.returntype.toLowerCase() == "json") {
                        if (rtn != null) {
                            var jobj = $.parseJSON(rtn);
                            if (jobj == null) {
                                jobj = rtn;
                            }
                            if (jobj.ErrorCode == "") {
                                if (jobj.Content.__isneedlogin__) {
                                    fns.Message.DialogLogin("请先登录！", jobj.Content.__loginurl__);
                                } else {
                                    if (opts.success != null) {
                                        opts.success(jobj.Content);
                                    } else {
                                        dtd.resolve(bobj.Content);
                                    }
                                }
                            } else {
                                fns.ExceptionProcess.ShowErrorMsg(jobj.ErrorCode + "\n" + jobj.ErrorMessage);
                                if (opts.fail) {
                                    opts.fail(jobj.ErrorCode, jobj.ErrorMessage);
                                } else {
                                    dtd.reject(bobj.ErrorCode, bobj.ErrorMessage);
                                }
                            }
                        }
                    } else {
                        if (opts.success != null) {
                            opts.success(rtn);
                        } else {
                            dtd.resolve(rtn);
                        }
                    }
                    
                },
                complete: function (XMLHttpRequest, textStatus) {
                    completehandler(XMLHttpRequest, textStatus,opts);
                },
                error: function (jqXHR, errormsg, errorThrown) {
                    errorhandler(jqXHR, errormsg, errorThrown, opts);
                    dtd.reject(errormsg);
                }
            };

            __f.submit(function () {
                $(this).ajaxSubmit(__op);
                return false;
            });

            __f.submit();
            return dtd.promise();
        },
        //ajax提交，应对含有文件上传的情况
        AjaxUpload: function (options) {
            var defaults = {
                url: "",
                returntype: "json",
                postdata: "",
                before: null, //function(XMLHttpRequest)
                success: null,//function(rtn)
                complete: null,//function (XMLHttpRequest, textStatus)
                fail: null,//function(errorcode,errormsg)
                xhr: null//function() custom xhr
            }
            var opts = $.extend(defaults, options);

            var fd = new FormData();
            if (opts.postdata != null) {
                for (var s in opts.postdata) {
                    fd.append(s, opts.postdata[s]);
                }
            }

            opts.processdata = false;
            opts.contenttype = false;
            opts.postdata = fd;

            return this.Ajax(opts);
        }
    }


    function successhandler(rtn, opts) {
        var jobj;
        if (opts.preprocessresult) {
            if (rtn != null) {
                //判斷jsonstr是否就是json對象
                //jobj = $.parseJSON(rtn);
                if (jobj == null) {
                    jobj = rtn;
                }
                if (opts.returntype.toLowerCase() == "json") {
                    var bobj = eval("(" + jobj + ")");
                    if (bobj.ErrorCode == "") {
                        if (typeof (bobj.Content.__isvalid__) != "undefined" && !bobj.Content.__isvalid__) {
                            fns.Message.DialogToUrl(bobj.Content.__msg__, bobj.Content.__tonurl__);
                        } else {
                            if (opts.success != null) {
                                opts.success(bobj.Content);
                            }
                        }
                    } else {
                        fns.ExceptionProcess.ShowErrorMsg(bobj.ErrorCode + "\n" + bobj.ErrorMessage);
                        if (opts.fail) {
                            opts.fail(bobj.ErrorCode, bobj.ErrorMessage);
                        }
                    }
                } else {
                    if (opts.success != null) {
                        opts.success(jobj);
                    }
                }
            }
        } else {
            if (opts.success != null) {
                opts.success(jobj);
            }
        }
    }

    function completehandler(XMLHttpRequest, textStatus, opts) {
        if (opts.complete != null) {
            opts.complete(XMLHttpRequest, textStatus);
        }

    }

    function errorhandler(jqXHR, errormsg, errorThrown, opts) {
        if (opts.fail) {
            opts.fail(errormsg, errormsg);
        }
    }
}(jQuery, FrameNameSpace))

