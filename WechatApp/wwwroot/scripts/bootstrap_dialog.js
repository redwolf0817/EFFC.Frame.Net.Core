(function ($) {

    window.Ewin = function () {
        var html = '<div id="[Id]" class="modal fade" role="dialog" aria-labelledby="modalLabel">' +
                   '<div class="modal-dialog modal-sm">' +
                     '<div class="modal-content">' +
                       '<div class="modal-header">' +
                         '<button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">×</span><span class="sr-only">Close</span></button>' +
                         '<h4 class="modal-title" id="modalLabel">[Title]</h4>' +
                       '</div>' +
                       '<div class="modal-body">' +
                       '<p>[Message]</p>' +
                       '</div>' +
                        '<div class="modal-footer">' +
                        '</div>' +
                     '</div>' +
                   '</div>' +
                 '</div>';


        var dialogdHtml = '<div id="[Id]" class="modal fade" role="dialog" aria-labelledby="modalLabel">' +
                   '<div class="modal-dialog">' +
                     '<div class="modal-content">' +
                       '<div class="modal-header">' +
                         '<button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">×</span><span class="sr-only">Close</span></button>' +
                         '<h4 class="modal-title" id="modalLabel">[Title]</h4>' +
                       '</div>' +
                       '<div class="modal-body">' +
                       '</div>' +
                     '</div>' +
                   '</div>' +
                 '</div>';
        var reg = new RegExp("\\[([^\\[\\]]*?)\\]", 'igm');
        var generateId = function () {
            var date = new Date();
            return 'mdl' + date.valueOf();
        }
        var init = function (options) {
            options = $.extend({}, {
                title: "操作提示",
                message: "提示内容",
                width: 200,
                auto: false,
                buttons:[]
            }, options || {});
            var modalId = generateId();
            var content = html.replace(reg, function (node, key) {
                return {
                    Id: modalId,
                    Title: options.title,
                    Message: options.message
                }[key];
            });
            $('body').append(content);
            
            $('#' + modalId).modal({
                width: options.width,
                backdrop: 'static'
            });
            $('#' + modalId).on('hide.bs.modal', function (e) {
                $('body').find('#' + modalId).remove();
            });
            return modalId;
        }

        return {
            currentId:"",
            dialog: function (options) {
                var self = this;
                if (typeof options == 'string') {
                    options = {
                        message: options
                    };
                }
                var id = init(options);
                self.currentId = id;
                var modal = $('#' + id);
                $(options.buttons).each(function (index,btn) {
                    var h = '<button type="button" class="btn btn-default" data-dismiss="modal">' + (btn.text ? btn.text : "") + '</button>';
                    if (["ok", "确定"].contains(btn.text)) {
                        h = '<button type="button" class="btn btn-primary ok" data-dismiss="modal">' + (btn.text ? btn.text : "") + '</button>';
                    }
                    var itemh = $(h);
                    itemh.click(function () {
                        if (btn.click) {
                            btn.click();
                        }
                    })
                    modal.find(".modal-footer").append(itemh);
                })
                //for (var i = 0; i < options.buttons.length; i++) {
                //    var btn = options.buttons[i];
                //    var h = '<button type="button" class="btn btn-default" data-dismiss="modal">' + (btn.text ? btn.text : "") + '</button>';
                //    if (["ok", "确定"].contains(btn.text)) {
                //        h = '<button type="button" class="btn btn-primary ok" data-dismiss="modal">' + (btn.text ? btn.text : "") + '</button>';
                //    }
                //    var itemh = $(h);
                //    itemh.click(function () {
                //        if (options.buttons[i].click) {
                //            options.buttons[i].click();
                //        }
                //    })
                //    modal.find(".modal-footer").append(itemh);
                //}

                return self;
            },
            alert: function (options) {
                var self = this;
                if (typeof options == 'string') {
                    options = {
                        message: options
                    };
                }
                var rtn = this.dialog($.extend({
                    title: "提示",
                    buttons: [
                        {
                            text: "确定",
                            click: function () {
                                self.close();
                            }
                        }
                    ]
                },options));

                return rtn;
            },
            confirm: function (options) {
                if (typeof options == 'string') {
                    options = {
                        message: options
                    };
                }
                var rtn = this.dialog($.extend({
                    title: "确认提示"
                }, options));

                return rtn;
            },
            modal: function (options) {
                options = $.extend({}, {
                    title: 'title',
                    url: '',
                    width: 800,
                    height: 550,
                    onReady: function () { },
                    onShown: function (e) { }
                }, options || {});
                var modalId = generateId();
                this.currentId = modalId;
                var content = dialogdHtml.replace(reg, function (node, key) {
                    return {
                        Id: modalId,
                        Title: options.title
                    }[key];
                });
                $('body').append(content);
                var target = $('#' + modalId);
                target.find('.modal-body').load(options.url);
                if (options.onReady())
                    options.onReady.call(target);
                target.modal();
                target.on('shown.bs.modal', function (e) {
                    if (options.onReady(e))
                        options.onReady.call(target, e);
                });
                target.on('hide.bs.modal', function (e) {
                    $('body').find(target).remove();
                });
            },
            close: function () {
                var self = this;
                var modal = $('#' + self.currentId);
                modal.modal('hide');
            }
        }
    }();
})(jQuery);