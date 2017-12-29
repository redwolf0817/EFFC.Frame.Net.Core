

; (function ($, $$) {
    var ultop = $("#topmenu");
    ultop.empty();
    var ulleft = $("#leftmenu");
    ulleft.empty();
    BuilderTopMenu();
    renderNaviBar();

    $("#navbar .navbar-right a").first().click(function () {
        sessionStorage.clear();
        location.href = "/admin/logout";
    })

    function BuilderTopMenu() {
        var currentTopMenuClick = sessionStorage.getItem("currentTopMenuClick");
        currentTopMenuClick = currentTopMenuClick ? JSON.parse(currentTopMenuClick) : null;
        var menu = JSON.parse(sessionStorage.getItem("menu"));
        if (menu) {
            $(menu).each(function (i, v) {
                if (i == 0 && currentTopMenuClick==null) {
                    currentTopMenuClick = {
                        text: v.text,
                        no: v.no
                    }
                }
                var url = menu[i].url == "" ? "#" : menu[i].url;
                var li = $('<li><a href="' + url + '">' + menu[i].text + '</a></li>');
                if (url == "#") {
                    var index = i;
                    li.click(function () {
                        ulleft.empty();
                        BuilderLeftMenu(menu[index]);
                        sessionStorage.setItem("currentTopMenuClick", JSON.stringify({ text: v.text, no:v.no}));
                    })

                    if (v.no == currentTopMenuClick.no)
                        li.click();
                }
                ultop.append(li);
            })
            
        }
    }

    function BuilderLeftMenu(parentNode, topcontrol) {
        var currentLeftMenuClick = sessionStorage.getItem("currentLeftMenuClick");
        currentLeftMenuClick = currentLeftMenuClick ? JSON.parse(currentLeftMenuClick) : null;
        var nodes = parentNode.nodes;
        if (nodes) {
            $(nodes).each(function (i, v) {
                if (v.level == 1) {
                    var li = null;
                    if (v.url == "") {
                        li = $('<li class="nav-header">' + v.text + '</li>');
                    } else {
                        li = $('<li class="nav-header"></li>');
                        var a = $('<a href="#">' + v.text + '</a>');
                        li.append(a);
                        a.click(function () {
                            var currentLeftMenuClick = [];
                            if (parentNode.level > 0) {
                                currentLeftMenuClick[currentLeftMenuClick.length] = {
                                    text: parentNode.text,
                                    no: parentNode.no,
                                    url: parentNode.url
                                }
                            }
                            currentLeftMenuClick[currentLeftMenuClick.length] = {
                                text: v.text,
                                no: v.no,
                                url: v.url
                            }
                            sessionStorage.setItem("currentLeftMenuClick", JSON.stringify(currentLeftMenuClick));
                            location.href = v.url;
                        })
                    }
                    ulleft.append(li);
                    if (v.nodes.length > 0) {
                        BuilderLeftMenu(v, li);
                    }
                } else {
                    var li = null;
                    if (v.url == "") {
                        li = $('<a href="#">' + v.text + '</a>');
                    } else {
                        li = $('<a href="#">' + v.text + '</a>');
                        li.click(function () {
                            var currentLeftMenuClick = [];
                            if (parentNode.level > 0) {
                                currentLeftMenuClick[currentLeftMenuClick.length] = {
                                    text: parentNode.text,
                                    no: parentNode.no,
                                    url: parentNode.url
                                }
                            }
                            currentLeftMenuClick[currentLeftMenuClick.length] = {
                                text: v.text,
                                no: v.no,
                                url: v.url
                            }
                            sessionStorage.setItem("currentLeftMenuClick", JSON.stringify(currentLeftMenuClick));
                            location.href = v.url;
                        })
                    }
                    topcontrol.append(li);
                    if (v.nodes.length > 0) {
                        BuilderLeftMenu(v, li);
                    }
                }
            })
        }
    }

    function renderNaviBar() {
        var topmenu = sessionStorage.getItem("currentTopMenuClick");
        var leftmenu = sessionStorage.getItem("currentLeftMenuClick");

        var breadcrumb = $(".breadcrumb");
        breadcrumb.empty();
        if (!topmenu) {
            var li = $('<li class="active">首页</li>');
            breadcrumb.append(li);
            return;
        }
        var top = JSON.parse(topmenu);
        var left = JSON.parse(leftmenu);

        var mainli = $('<li>首页</li>');
        breadcrumb.append(mainli);
        var topli = $('<li>' + top.text + '</li>');
        breadcrumb.append(topli);
        $(left).each(function (i, v) {
            var li = $('<li></li>');
            if (v.url == "") {
                li.text(v.text);
            } else {
                if (i < left.length - 1) {
                    var a = $('<a href="' + v.url + '">' + v.text + '</a>');
                    li.append(a);
                } else {
                    li.text(v.text);
                    li.addClass("active");
                }
                
            }
            breadcrumb.append(li);
        });
        //var lastitem = left[left.length - 1];
        //if (!location.href.endsWith(lastitem.url)) {
        //    location.href = lastitem.url;
        //}

    }
    

    $$.extend({
        loadPageNavigation: function (options) {
            var defaults = {
                current_page: 1,
                count_per_page: 10,
                total_page: 0,
                total_rows: 0,
                queryfunction:null
            }
            var opts = $$.extend(true, defaults, options);

            var div = $('<div class="container"></div>');
            var h_topage = $('<input type="hidden" id="toPage" />');
            var h_count_per_page = $('<input type="hidden" id="Count_per_Page" />');
            h_count_per_page.val(opts.count_per_page);

            div.append(h_topage);
            div.append(h_count_per_page);

            if (opts.total_page > 1) {
                var ul = $('<ul class="pagination"></ul>');
                div.append(ul);

                if (opts.current_page > 1) {
                    var li = $('<li></li >');
                    var a = $('<a href= "#"> 上一页</a >');
                    li.append(a);
                    if (opts.queryfunction) {
                        a.click(function () {
                            h_topage.val(opts.current_page - 1);
                            opts.queryfunction();
                        })
                    }

                    ul.append(li);
                }
                
                var liarr = [];
                if (opts.total_page <= 16) {
                    for (var i = 1; i <= opts.total_page; i++) {
                        var li = $('<li></li >');
                        if (i == opts.current_page) li.addClass("active");
                        var a = $('<a href= "#">' + i + '</a >');
                        li.append(a);

                        liarr[liarr.length] = li;
                        ul.append(li);
                    }
                } else {
                    //页数1总会显示
                    var li = $('<li></li >');
                    if (i == opts.current_page) li.addClass("active");
                    var a = $('<a href= "#">1</a >');
                    li.append(a);

                    liarr[liarr.length] = li;
                    ul.append(li);

                    if (opts.current_page > 4) {
                         var li = $('<li><a href= "#">...</a ></li >');
                         ul.append(li);
                    }

                    for (var i = opts.current_page-2; i <= opts.current_page+2; i++) {
                        var li = $('<li></li >');
                        if (i == opts.current_page) li.addClass("active");
                        var a = $('<a href= "#">' + i + '</a >');
                        li.append(a);

                        liarr[liarr.length] = li;
                        ul.append(li);
                    }

                    if (opts.current_page < opts.total_page-2) {
                        var li = $('<li><a href= "#">...</a ></li >');
                        ul.append(li);
                    }
                    //最后一页总显示
                    var li = $('<li></li >');
                    if (i == opts.current_page) li.addClass("active");
                    var a = $('<a href= "#">' + opts.total_page + '</a >');
                    li.append(a);

                    liarr[liarr.length] = li;
                    ul.append(li);
                }
                //添加翻页事件
                $(liarr).each(function (i, v) {
                    $(v).click(function () {
                        h_topage.val(parseInt($(v).text()));
                        opts.queryfunction();
                    })
                })

                if (opts.current_page < opts.total_page) {
                    var li = $('<li></li >');
                    var a = $('<a href= "#"> 下一页</a >');
                    li.append(a);
                    if (opts.queryfunction) {
                        a.click(function () {
                            h_topage.val(opts.current_page + 1);
                            opts.queryfunction();
                        })
                    }

                    ul.append(li);
                }
            }
            return div;
        }
    })
}(jQuery, effc))