; (function ($,fns) {
    fns.Form = {
        submit: function (options) {
            var defaults = {
                url: "",
                postdata: {}
            }
            var opts = fns.extend(defaults, options);
            var f = $("<form method='post' id='__form_submit__' name='__form_submit__'></form>");
            f.attr("action", opts.url);
            f.attr('target', '_self');
            for (var v in opts.postdata) {
                var input = $("<input type='hidden' />").attr("name", v).val(opts.postdata[v]);
                f.append(input);
            }

            //IE fire event 
            if (fns.BrowserMatch.browser == "IE"
                || fns.BrowserMatch.browser == "firefox") {
                $("form").append(f);
                f[0].submit();
                //DOM2 fire event 
            } else if (document.createEvent) {
                f.submit();
            }
            return false;
        }
    }
}(jQuery, FrameNameSpace))