

class QueryTableHeader extends React.Component {
    constructor(props) {
        super(props);

        this.handleAdd = this.handleAdd.bind(this);
    }

    handleAdd(e) {
        var mdata = {
            m_uid: "",
            m_loginid: "",
            m_loginname: "",
            m_loginpass: "",
            m_isactive: true,
            m_remark: "",
            m_op: "add"
        }
        this.props.onOpClick(mdata, e)
    }

    render() {
        return (
            <thead>
                <tr>
                    <th>登录账号</th>
                    <th>登录名称</th>
                    <th>是否启用</th>
                    <th>备注</th>
                    <th>创建时间</th>
                    <th>操作&nbsp;&nbsp;&nbsp;<button className="btn btn-primary btn-xs" data-toggle="modal" data-target="#myModal" onClick={this.handleAdd}>新增</button></th>
                </tr>
            </thead>
        )
    }
}
class QueryTableRows extends React.Component {
    constructor(props) {
        super(props);

        this.handleUpdate = this.handleUpdate.bind(this);
        this.handleDelete = this.handleDelete.bind(this);
        this.handleRoleMap = this.handleRoleMap.bind(this);
    }

    handleUpdate(data, e) {
        var mdata = {
            m_uid: data.uid,
            m_loginid: data.loginid,
            m_loginname: data.loginname,
            m_loginpass: data.loginpass,
            m_isactive: data.isactive == "true" ? true : false,
            m_remark: data.remark,
            m_op: "update"
        }
        this.props.onOpClick(mdata,e)
    }
    handleDelete(data, e) {
        this.props.onDeleteClick(data.uid, e)
    }
    handleRoleMap(data, e) {
        this.props.onLoadRoleList(data);
    }

    render() {
        var datas = this.props.data;
        let rows = [];
        if (!datas || datas.length <= 0) {
            rows.push(<tr className="warning"><td colSpan={"6"} style={{ "textAlign": "center" }}>{"查无资料"}</td></tr>)
        } else {
            var react = this;
            datas.forEach(function (data) {
                rows.push(<tr key={data.uid}>
                    <td>{data.loginid}</td>
                    <td>{data.loginname}</td>
                    <td>{data.isactive=="true"?"是":"否"}</td>
                    <td>{data.remark}</td>
                    <td>{data.createtime}</td>
                    <td>
                        <div className="btn-group">
                            <button className="btn btn-warning btn-sm" data-toggle="modal" data-target="#myModal" onClick={react.handleUpdate.bind(this, data)}>修改</button>
                            <button className="btn btn-danger btn-sm" onClick={react.handleDelete.bind(this, data)}>删除</button>
                            <button className="btn btn-primary btn-sm" data-toggle="modal" data-target="#myrolemap" onClick={react.handleRoleMap.bind(this, data)}>角色授权</button>
                        </div>
                    </td>
                </tr>);
            });
        }
        return (
            <tbody>
                    {rows}
            </tbody>
        )
    }
}

class QueryTable extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        var id = this.props.id;
        return (
            <div>
                <hr />
                <div className="row">
                    <div className="col-xs-12" id="result">
                        <table className="table table-striped table-hover" id={id}>
                            <QueryTableHeader onOpClick={this.props.onOpClick} />
                            <QueryTableRows data={this.props.data} onOpClick={this.props.onOpClick} onDeleteClick={this.props.onDeleteClick} onLoadRoleList={this.props.onLoadRoleList} />
                        </table>
                    </div>
                </div>
            </div>
        );
    }
}

class ModuleDiv extends React.Component {
    constructor(props) {
        super(props);

        this.handleSaveClick = this.handleSaveClick.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    handleSaveClick(e) {
        this.props.onSaveClick();
    }

    handleInputChange(colname, e) {
        if (colname == "m_isactive") {
            this.props.onInputChange(colname, e.target.checked);
        } else {
            this.props.onInputChange(colname, e.target.value);
        }
    }

    render() {
        var id = this.props.id;
        var react = this;
        
        return (
            <div className="modal fade" id={id} tabIndex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" >
                <input type="hidden" id="op" value={this.props.moduledata.m_op} />
                <div className="modal-dialog">
                    <div className="modal-content">
                        <div className="modal-header">
                            
                            <h4 className="modal-title" id="myModalLabel">
                                登录账户操作
                            </h4>
                        </div>
                        <div className="modal-body">
                            <div className="form-inline">
                                <div className="form-group">
                                    <label>登录账号：</label>
                                    <input type="text" className="form-control" maxLength="50" value={this.props.moduledata.m_loginid} onChange={react.handleInputChange.bind(this, "m_loginid")} />
                                </div>
                            </div>
                            <br />
                            <div className="form-inline">
                                <div className="form-group">
                                    <label>账号名称：</label>
                                    <input type="text" className="form-control" maxLength="50" value={this.props.moduledata.m_loginname} onChange={react.handleInputChange.bind(this, "m_loginname")}/>
                                </div>
                            </div>
                            <br />
                            <div className="form-inline">
                                <div className="form-group">
                                    <label>登录密码：</label>
                                    <input type="password" className="form-control" maxLength="128" value={this.props.moduledata.m_loginpass} onChange={react.handleInputChange.bind(this, "m_loginpass")}/>
                                </div>
                            </div>
                            <br />
                            
                            <div className="form-inline">
                                <div className="checkbox">
                                    <input type="checkbox" checked={this.props.moduledata.m_isactive} onChange={react.handleInputChange.bind(this, "m_isactive")}/> <label>是否激活</label>
                                </div>
                            </div>
                            <br />
                            <div className="form-inline">
                                <div className="form-group">
                                    <label>备&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;注：</label>
                                    <input type="text" className="form-control" maxLength="500" value={this.props.moduledata.m_remark} onChange={react.handleInputChange.bind(this, "m_remark")} />
                                </div>
                            </div>
                        </div>
                        <div className="modal-footer">
                            <button type="button" className="btn btn-default" data-dismiss="modal" id="close">
                                关闭
                            </button>
                            <button type="button" className="btn btn-primary" onClick={this.handleSaveClick}>
                                提交更改
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
class MapRoleDiv extends React.Component {
    constructor(props) {
        super(props);

        this.handleSaveClick = this.handleSaveClick.bind(this);
        this.handleSelected = this.handleSelected.bind(this);
        this.selectedvalues = [];
        
    }

    handleSaveClick(e) {
        this.props.onSaveRoleMapClick(this.selectedvalues);
    }
    handleSelected(e) {
        this.selectedvalues = $(e.target).val();
    }

    render() {
        var id = this.props.id;
        var roledata = this.props.rolelist;
        var react = this;
        var options = [];
        var seletedarr = [];
        roledata.forEach(function (item) {
            if (item.isselected == "1") {
                options.push(<option selected="true" value={item.roleuid}>{item.rolename}</option>)
                react.selectedvalues.push(item.roleuid);
            } else {
                options.push(<option value={item.roleuid}>{item.rolename}</option>)
            }
        })
        
        return (
            <div className="modal fade" id={id} tabIndex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" >
                <div className="modal-dialog">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h4 className="modal-title" id="myModalLabel">
                                角色授权操作
                            </h4>
                        </div>
                        <div className="modal-body">
                            <div className="form-inline">
                                <div className="form-group">
                                    <label>登录账号：</label>
                                    <input type="text" className="form-control" value={this.props.moduledata.loginname} readOnly="readonly" />
                                </div>
                            </div>
                            <br />
                            <div className="form-inline">
                                <div className="form-group">
                                    <label>角&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;色：</label>
                                    <select multiple className="form-control" onChange={this.handleSelected}>
                                        {options}
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div className="modal-footer">
                            <button type="button" className="btn btn-default" data-dismiss="modal" id="close2">
                                关闭
                            </button>
                            <button type="button" className="btn btn-primary" onClick={this.handleSaveClick}>
                                提交更改
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

class SearchBar extends React.Component {
    constructor(props) {
        super(props);

        this.handleSearch = this.handleSearch.bind(this);
        this.handleFilterChange = this.handleFilterChange.bind(this);
    }

    componentDidMount() {
        this.handleSearch();
    }

    handleFilterChange(e) {
        this.props.onFilterTextChange(e.target.value);
    }

    handleSearch(e) {
        this.props.onSearchClick();
    }

    render() {
        return (
            <div className="row">
                <div className="col-xs-12">
                    <div className="form-inline">
                        <div className="form-group">
                            <input type="text" className="form-control" id="functionno_parentno" placeholder="输入登录账号或名称" value={this.props.filterText} onChange={this.handleFilterChange} />
                            &nbsp;<button className="btn btn-primary btn-sm" id="search" onClick={this.handleSearch}>查询</button>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

class PageNavigation extends React.Component {
    constructor(props) {
        super(props);

        this.handleToPage = this.handleToPage.bind(this);
    }

    handleToPage(topage, e) {
        this.props.onToPage(topage, this.props.countPerPage);
    }

    render() {
        var react = this;
        var liarr = [];
        if (this.props.currentPage > 1) {
            liarr.push(<li><a href="#" onClick={react.handleToPage.bind(this, this.props.currentPage - 1)}> 上一页</a></li>);
        }
        if (this.props.totalPage <= 16) {
            for (let i = 1; i <= this.props.totalPage; i++) {
                if (i == react.props.currentPage) {
                    liarr.push(<li className="active"><a href="#">{i}</a></li>);
                } else {
                    liarr.push(<li><a href="#" onClick={react.handleToPage.bind(this, i)}>{i}</a></li>);
                }
            }
        } else {
            //页数1总会显示
            if (1 == react.props.currentPage) {
                liarr.push(<li className="active"><a href="#">1</a></li>);
            } else {
                liarr.push(<li><a href="#" onClick={react.handleToPage.bind(this, 1)}>1</a></li>);
            }

            if (react.props.currentPage > 4) {
                liarr.push(<li><a href="#">...</a></li>);
            }

            for (let i = react.props.currentPage - 2; i <= react.props.currentPage + 2; i++) {
                if (i == react.props.currentPage) {
                    liarr.push(<li className="active"><a href="#">{i}</a></li>);
                } else {
                    liarr.push(<li><a href="#" onClick={react.handleToPage.bind(this, i)}>{i}</a></li>);
                }
            }

            if (opts.current_page < opts.total_page - 2) {
                liarr.push(<li><a href="#">...</a></li>);
            }

            //最后一页总显示
            if (react.props.totalPage == react.props.currentPage) {
                liarr.push(<li className="active"><a href="#">{react.props.totalPage}</a></li>);
            } else {
                liarr.push(<li><a href="#" onClick={react.handleToPage.bind(this, react.props.totalPage)}>{react.props.totalPage}</a></li>);
            }
        }

        if (react.props.totalPage > react.props.currentPage) {
            liarr.push(<li><a href="#" onClick={react.handleToPage.bind(this, react.props.currentPage + 1)}>下一页</a></li>);
        }

        

        return (
            <div className="container" id="pagenav">
                {react.props.totalPage > 1 &&
                    <ul className="pagination">
                        {liarr}
                    </ul>
                }
            </div>
        )
    }
}

class App extends React.Component {
    constructor(props) {
        super(props);
        this.handleSearch = this.handleSearch.bind(this);
        this.handlePop = this.handlePop.bind(this);
        this.handleModelInputChange = this.handleModelInputChange.bind(this);
        this.handleFilterTextChange = this.handleFilterTextChange.bind(this);
        this.handleSave = this.handleSave.bind(this);
        this.handleDelete = this.handleDelete.bind(this);
        this.handleToPage = this.handleToPage.bind(this);
        this.handleSaveRoleMap = this.handleSaveRoleMap.bind(this);
        this.loadRoleList = this.loadRoleList.bind(this);

        this.state = {
            data:[],
            filterText: "",
            m_data: {
                m_uid: "",
                m_loginid: "",
                m_loginname: "",
                m_loginpass: "",
                m_isactive: false,
                m_remark: "",
                m_op: ""
            },
            toPage: 1,
            countPerPage: 5,
            totalPage: 0,
            totalRows: 0,
            currentPage: 1,
            rolemap:[]
        }
    }

    handleFilterTextChange(filterText, e) {
        this.setState({
            filterText: filterText
        })
    }

    handleSearch(e) {
        var react = this;
        $$.Net.Ajax("/account/list", "post", {
            filter: this.state.filterText,
            toPage: this.state.toPage,
            Count_per_Page: this.state.countPerPage
        }).then(function (rtn) {
                if (rtn.issuccess) {
                    react.setState({
                        data: rtn.data,
                        toPage: rtn.to_page,
                        countPerPage: rtn.count_per_page,
                        totalPage: rtn.total_page,
                        totalRows: rtn.total_row,
                        currentPage: rtn.current_page
                    });
            } else {
                $$.Message.ShowMsg(rtn.msg);
            }
        })
    }

    handlePop(m_data, e) {
        this.setState({
            m_data: m_data
        })
    }
    handleModelInputChange(colname, val, e) {
        var m_data = this.state.m_data;
        m_data[colname] = val;
        this.setState({
            m_data: m_data
        })
    }
    handleSave(e) {
        let url = "/account/" + this.state.m_data.m_op;
        let react = this;
        $$.Net.Ajax(url, "post", {
            uid: this.state.m_data.m_uid,
            loginid: this.state.m_data.m_loginid,
            loginname: this.state.m_data.m_loginid,
            pass: this.state.m_data.m_loginpass,
            isactive: this.state.m_data.m_isactive,
            remark: this.state.m_data.m_remark,
        }).then(function (rtn) {
            $$.Message.ShowMsg(rtn.msg);
            if (rtn.issuccess) {
                document.getElementById("close").click();
                react.handleSearch();
            }
        })
    }

    handleDelete(uid,e) {
        let react = this;
        if ($$.Message.ShowConfirm({
            msg: "确定删除该账号？",
            ok: {
                text: "确定",
                click: function (target) {
                    $$.Message.Close();
                    $$.Net.Ajax("/account/delete", "post", {
                        uid: uid
                    }).then(function (rtn) {
                        if (rtn.issuccess) {
                            react.handleSearch();
                        }
                        $$.Message.ShowMsg(rtn.msg);
                    })
                }
            }
        }));
    }

    handleToPage(toPage, countPerPage, e) {
        //setstate是异步方法，不能及时生效，直接赋值的方式可以及时生效，但无法同步所有节点的值，但在本例子中是适用的
        //因为翻页查询完成后会用setstate方法再同步所有节点
        this.state.toPage = toPage;
        this.state.countPerPage = countPerPage;
        //this.setState({
        //    toPage: toPage,
        //    countPerPage: countPerPage
        //})
        this.handleSearch();
    }

    handleSaveRoleMap(selectedarr, e) {
        var self = this;
        $$.Net.Ajax("/account/saverolemap", "post", {
            uid: self.state.m_data.uid,
            roles: selectedarr
        }).then(function (rtn) {
            $$.Message.ShowMsg(rtn.msg);
            if (rtn.issuccess) {
                document.getElementById("close2").click();
            }
        })
    }
    loadRoleList(data, e) {
        let react = this;
        $$.Net.Ajax("/account/rolemap", "post", {
            uid: data.uid
        }).then(function (rtn) {
            if (rtn.issuccess) {
                react.setState({
                    m_data: data,
                    rolemap:rtn.data
                })
            } else {
                $$.Message.ShowMsg(rtn.msg);
                e.preventDefault();
            }
        })
    }
    render() {
        return (
            <div>
                <SearchBar onSearchClick={this.handleSearch} onFilterTextChange={this.handleFilterTextChange} filterText={this.state.filterText} />
                <QueryTable id={"queryresult"} data={this.state.data} onOpClick={this.handlePop} onDeleteClick={this.handleDelete} onLoadRoleList={this.loadRoleList}/>
                <PageNavigation onToPage={this.handleToPage} currentPage={this.state.currentPage} totalPage={this.state.totalPage} countPerPage={this.state.countPerPage} />
                <ModuleDiv id={"myModal"} moduledata={this.state.m_data} onInputChange={this.handleModelInputChange} onSaveClick={this.handleSave} />
                <MapRoleDiv id={"myrolemap"} rolelist={this.state.rolemap} moduledata={this.state.m_data} onSaveRoleMapClick={this.handleSaveRoleMap} />
            </div>
        );
    }
}
ReactDOM.render(
    <App />,
    document.getElementById('page')
);

