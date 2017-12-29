using EFFC.Frame.Net.Base.AttributeDefine;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Base.ResouceManage.JsEngine;
using EFFC.Frame.Net.Business.Logic;
using EFFC.Frame.Net.Business.Unit.HostUnit;
using EFFC.Frame.Net.Data;
using EFFC.Frame.Net.Data.LogicData;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Business.Engine
{
    #region Server
    /// <summary>
    /// 服务器端对象
    /// </summary>
    public class ServerObject : BaseHostJsObject
    {
        WebBaseLogic<WebParameter, GoData> _logic = null;
        public ServerObject(WebBaseLogic<WebParameter, GoData> logic)
        {
            _logic = logic;
        }
        [Desc("记录infolog")]
        public void LogInfo(string msg)
        {
            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, msg);
        }
        [Desc("记录debuglog")]
        public void LogDebug(string msg)
        {
            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.DEBUG, msg);
        }
        [Desc("记录errorlog")]
        public void LogError(string msg)
        {
            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.ERROR, msg);
        }
        [Desc("服务端架构信息")]
        public Dictionary<string, object> Info
        {
            get
            {
                var rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                rtn.EFFCVersion = "EFFC 3.0";
                rtn.HostCoreVersion = Environment.Version.ToString();
                return rtn;
            }
        }
        [Desc("判定本次请求是否为ajax异步调用")]
        public bool IsAjaxAsync
        {
            get
            {
                return _logic.IsAjaxAsync;
            }
        }
        [Desc("判定本次请求是否为websocket请求")]
        public bool IsWebSocket
        {
            get
            {
                return _logic.IsWebSocket;
            }
        }
        [Desc("Server的IP地址")]
        public string IP
        {
            get
            {
                return _logic.ServerInfo.IP;
            }
        }
        [Desc("站点的域名")]
        public string Domain
        {
            get
            {
                return _logic.ServerInfo.Domain;
            }
        }
        [Desc("当前系统时间")]
        public DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }
        public override string Description
        {
            get { return "服务器端对象"; }
        }

        public override string Name
        {
            get { return "Server"; }
        }
    }
    #endregion
    #region Clinet
    /// <summary>
    /// 服务器端对象
    /// </summary>
    public class ClientObject : BaseHostJsObject
    {
        WebBaseLogic<WebParameter, GoData> _logic = null;
        public ClientObject(WebBaseLogic<WebParameter, GoData> logic)
        {
            _logic = logic;
        }
        [Desc("Client的IP地址")]
        public string IP
        {
            get
            {
                return _logic.ClientInfo.IP;
            }
        }
        [Desc("Client的操作平台名称")]
        public string Platform
        {
            get
            {
                return _logic.ClientInfo.Platform;
            }
        }
        [Desc("Client的浏览器版本号")]
        public string BrowserVersion
        {
            get
            {
                return _logic.ClientInfo.BrowserVersion;
            }
        }
        [Desc("Client的机器名称")]
        public string UserHostName
        {
            get
            {
                return _logic.ClientInfo.UserHostName;
            }
        }
        public override string Description
        {
            get { return "客户端对象"; }
        }

        public override string Name
        {
            get { return "Client"; }
        }
    }
    #endregion
    #region Session
    public class SessionObject : BaseHostJsObject
    {
        WebBaseLogic<WebParameter, GoData> _logic = null;
        public SessionObject(WebBaseLogic<WebParameter, GoData> logic)
        {
            _logic = logic;
        }
        [Desc("获取Session值")]
        public object GetValue(string key)
        {
            var obj = _logic.Sessions.GetSessionValue(key);
            if (obj is FrameDLRObject)
            {
                return ((FrameDLRObject)obj).ToDictionary();
            }
            else
            {
                return obj;
            }
        }
        [Desc("设置Session值")]
        public void SetValue(string key, string value)
        {
            _logic.Sessions.AddSessionValue(key, value);
        }
        /// <summary>
        /// 清除当前session
        /// </summary>
        [Desc("清除当前session")]
        public void Abandon()
        {
            _logic.Sessions.SessionAbandon();
        }
        [Desc("删除session值")]
        public void Remove(string key)
        {
            _logic.Sessions.RemoveSessionValue(key);
        }
        public override string Description
        {
            get { return "session对象"; }
        }

        public override string Name
        {
            get { return "Session"; }
        }
    }
    #endregion
    #region LoginInfo
    /// <summary>
    /// 登录信息
    /// </summary>
    public class LoginInfoObject : BaseHostJsObject
    {
        WebBaseLogic<WebParameter, GoData> _logic = null;
        public LoginInfoObject(WebBaseLogic<WebParameter, GoData> logic)
        {
            _logic = logic;
        }
        /// <summary>
        /// 是否有登录信息
        /// </summary>
        public bool HasLoginInfo
        {
            get
            {
                return _logic.LoginInfo != null;
            }
        }
        /// <summary>
        /// 用户登录账号
        /// </summary>
        [Desc("用户登录账号")]
        public string UserID
        {
            get
            {
                if (_logic.LoginInfo != null)
                {
                    return _logic.LoginInfo.UserID;
                }
                else
                {
                    return "";
                }

            }
        }
        /// <summary>
        /// 用户名称
        /// </summary>
        [Desc("用户名称")]
        public string UserName
        {
            get
            {
                if (_logic.LoginInfo != null)
                {
                    return _logic.LoginInfo.UserName;
                }
                else
                {
                    return "";
                }
            }
        }
        [Desc("用户角色ID")]
        public string RoleID
        {
            get
            {
                if (_logic.LoginInfo != null)
                {
                    return _logic.LoginInfo.RoleID;
                }
                else
                {
                    return "";
                }
            }
        }
        [Desc("用户角色姓名")]
        public string RoleName
        {
            get
            {
                if (_logic.LoginInfo != null)
                {
                    return _logic.LoginInfo.RoleName;
                }
                else
                {
                    return "";
                }
            }
        }
        [Desc("用户群组ID")]
        public string GroupID
        {
            get
            {
                if (_logic.LoginInfo != null)
                {
                    return _logic.LoginInfo.GroupID;
                }
                else
                {
                    return "";
                }
            }
        }
        [Desc("用户群组名称")]
        public string GroupName
        {
            get
            {
                if (_logic.LoginInfo != null)
                {
                    return _logic.LoginInfo.GroupName;
                }
                else
                {
                    return "";
                }
            }
        }
        [Desc("代理人ID")]
        public string AgentID
        {
            get
            {
                if (_logic.LoginInfo != null)
                {
                    return _logic.LoginInfo.AgentID;
                }
                else
                {
                    return "";
                }
            }
        }
        [Desc("代理人姓名")]
        public string AgentName
        {
            get
            {
                if (_logic.LoginInfo != null)
                {
                    return _logic.LoginInfo.AgentName;
                }
                else
                {
                    return "";
                }
            }
        }
        [Desc("用户登录IP地址")]
        public string LoginIPAddress
        {
            get
            {
                if (_logic.LoginInfo != null)
                {
                    return _logic.LoginInfo.LoginIPAddress;
                }
                else
                {
                    return "";
                }
            }
        }
        [Desc("用户登录时间")]
        public DateTime LoginTime
        {
            get
            {
                if (_logic.LoginInfo != null)
                {
                    return _logic.LoginInfo.LoginTime;
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }
        [Desc("根据url获取对应的action权限值")]
        public string GetActionsByPageUrl(string url)
        {
            return _logic.LoginInfo.Actions.GetActionIDsByPageUrl(url);
        }
        /// <summary>
        /// 获取其他登录信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetValue(string key)
        {
            if (_logic.LoginInfo != null)
            {
                return _logic.LoginInfo.GetValue(key);
            }
            else
            {
                return "";
            }
        }
        
        [Desc("更新Logininfo数据")]
        public void UpdateLoginInfo(Dictionary<string, object> obj)
        {
            var lud = new Data.LoginUserData();
            foreach (var item in obj)
            {
                if (item.Key.ToLower() == "userid")
                {
                    lud.UserID = ComFunc.nvl(item.Value);
                }
                else if (item.Key.ToLower() == "username")
                {
                    lud.UserName = ComFunc.nvl(item.Value);
                }
                else if (item.Key.ToLower() == "roleid")
                {
                    lud.RoleID = ComFunc.nvl(item.Value);
                }
                else if (item.Key.ToLower() == "rolename")
                {
                    lud.RoleName = ComFunc.nvl(item.Value);
                }
                else if (item.Key.ToLower() == "groupid")
                {
                    lud.GroupID = ComFunc.nvl(item.Value);
                }
                else if (item.Key.ToLower() == "groupname")
                {
                    lud.GroupName = ComFunc.nvl(item.Value);
                }
                else if (item.Key.ToLower() == "agentid")
                {
                    lud.AgentID = ComFunc.nvl(item.Value);
                }
                else if (item.Key.ToLower() == "agentname")
                {
                    lud.AgentName = ComFunc.nvl(item.Value);
                }
                else if (item.Key.ToLower() == "loginipaddress")
                {
                    lud.LoginIPAddress = ComFunc.nvl(item.Value);
                }
                else if (item.Key.ToLower() == "logintime")
                {
                    lud.LoginTime =  (item.Value != null & item.Value is DateTime?(DateTime)item.Value:DateTime.MinValue);
                }
                else
                {
                    lud.SetValue(item.Key, item.Value);
                }
            }
            _logic.UpdateLoginInfo(lud);
        }

        public override string Description
        {
            get { return "登录者的信息"; }
        }

        public override string Name
        {
            get { return "LoginInfo"; }
        }
    }
    #endregion
    #region Cookie
    /// <summary>
    /// Cookie对象
    /// </summary>
    public class CookieObject : BaseHostJsObject
    {
        WebBaseLogic<WebParameter, GoData> _logic = null;
        public CookieObject(WebBaseLogic<WebParameter, GoData> logic)
        {
            _logic = logic;
        }
        [Desc("获取Cookie值")]
        public string GetValue(string key)
        {
            return _logic.Cookie.GetCookie(key);
        }
        [Desc("设置Cookie值")]
        public void SetValue(string key, string value)
        {
            _logic.Cookie.SetCookie(key, value);
        }
        /// <summary>
        /// 删除一个cookie数据
        /// </summary>
        /// <param name="key"></param>
        [Desc("删除一个cookie数据")]
        public void Remove(string key)
        {
            _logic.Cookie.RemoveCookie(key);
        }
        public override string Description
        {
            get { return "服务器端cookie访问"; }
        }

        public override string Name
        {
            get { return "Cookie"; }
        }
    }
    #endregion
    #region Config
    public class ConfigObject : BaseHostJsObject
    {
        WebBaseLogic<WebParameter, GoData> _logic = null;
        public ConfigObject(WebBaseLogic<WebParameter, GoData> logic)
        {
            _logic = logic;
        }
        [Desc("获取参数值")]
        public object GetValue(string key)
        {
            return _logic.Configs[key];
        }
        public override string Description
        {
            get { return "Config对象"; }
        }

        public override string Name
        {
            get { return "Config"; }
        }
    }
    #endregion
    #region DB
    public class DBObject : BaseHostJsObject
    {
        WebBaseLogic<WebParameter, GoData> _logic = null;
        UnitParameter _up = null;
        public DBObject(WebBaseLogic<WebParameter, GoData> logic)
        {
            _logic = logic;
        }
        /// <summary>
        /// 开启一个DB连接
        /// </summary>
        /// <param name="dbtype"></param>
        /// <param name="connstr"></param>
        [Desc(@"开启一个DB连接，dbtype为：Oracle，SqlServer,SqlServer2000,mongo,默认为sqlserver;connstr为连接串")]
        public void Open(string dbtype, string connstr)
        {
            Close();
            switch (dbtype.ToLower())
            {
                case "oracle":
                    _up = _logic.DB.NewDBUnitParameter<OracleAccess>();
                    break;
                case "sqlserver2000":
                    _up = _logic.DB.NewDBUnitParameter<SQLServerAccess2000>();
                    break;
                case "mongo":
                    _up = _logic.DB.NewDBUnitParameter<MongoAccess26>();
                    break;
                default:
                    _up = _logic.DB.NewDBUnitParameter<SQLServerAccess>();
                    break;
            }
            _up.Dao.Open(connstr);
        }
        [Desc(@"开启一个系统默认的数据库连接,数据库类型为SqlServer")]
        public void OpenDefault()
        {
            Open("default", ComFunc.nvl(_logic.Configs[ParameterKey.DBCONNECT_STRING]));
        }
        [Desc(@"数据库执行操作，condition为json对象(符合EFFC3.0中DBExpress的表述结构);返回值为{data}")]
        public object Excute(Dictionary<string, object> condition)
        {
            var udc = _logic.DB.Excute(_up, (FrameDLRObject)FrameDLRObject.CreateInstance(condition));
            var list = udc.QueryData<FrameDLRObject>();
            list = list == null ? new List<FrameDLRObject>() : list;
            var rtn = FrameDLRObject.CreateInstance();
            rtn.data = list;
            return ((FrameDLRObject)rtn).ToDictionary();
        }
         [Desc(@"数据库查询操作，sql为sql脚本，data为数据;返回值为{data}")]
        public virtual object Query(string sql, Dictionary<string, object> data)
        {
            _up.SetValue("___host_sql___", sql);
            if (data != null)
            {
                foreach (var item in data)
                {
                    _up.SetValue(item.Key, item.Value);
                }
            }
            var udc = _logic.DB.Query<HostQueryUnit>(_up, "");
            var list = udc.QueryData<FrameDLRObject>();
            list = list == null ? new List<FrameDLRObject>() : list;
            var rtn = FrameDLRObject.CreateInstance();
            rtn.data = list;
            return ((FrameDLRObject)rtn).ToDictionary();
        }
         [Desc(@"数据库非查询操作，sql为sql脚本，data为数据")]
         public virtual void NonQuery(string sql, Dictionary<string, object> data)
         {
             _up.SetValue("___host_sql___", sql);
             if (data != null)
             {
                 foreach (var item in data)
                 {
                     _up.SetValue(item.Key, item.Value);
                 }
             }
             _logic.DB.NonQuery<HostNonQueryUnit>(_up, "");
         }
        [Desc(@"数据库非查询操作，sql为{sql,orderby,presql,aftersql}，data为参数数据，格式为{key1:value1,key2:value2......}。返回值为{data,TotalPage,TotalRow,Count_Of_OnePage,CurrentPage}")]
         public virtual object QueryByPage(Dictionary<string,object> sqlobj, int topage, int count_per_page, Dictionary<string, object> data)
         {
             if (sqlobj == null) sqlobj = new Dictionary<string, object>();
             var dsql = FrameDLRObject.CreateInstance(sqlobj);


             _up.SetValue("___host_sql___", ComFunc.nvl(dsql.sql));
             _up.SetValue("___host_orderby___", ComFunc.nvl(dsql.orderby));
             _up.SetValue("___host_pre___", ComFunc.nvl(dsql.presql));
             _up.SetValue("___host_after___", ComFunc.nvl(dsql.aftersql));
             if (data != null)
             {
                 foreach (var item in data)
                 {
                     _up.SetValue(item.Key, item.Value);
                 }
             }
             _up.Count_Of_OnePage = count_per_page;
             _up.ToPage = topage;
             var udc = _logic.DB.QueryByPage<HostQueryByPageUnit>(_up, "");
             var list = udc.QueryData<FrameDLRObject>();
             list = list == null ? new List<FrameDLRObject>() : list;
             var rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
             rtn.data = list;
             rtn.TotalPage = udc.TotalPage;
             rtn.TotalRow = udc.TotalRow;
             rtn.Count_Of_OnePage = udc.Count_Of_OnePage;
             rtn.CurrentPage = udc.CurrentPage;
             return ((FrameDLRObject)rtn).ToDictionary();
         }
        [Desc(@"关闭连接")]
        public void Close()
        {
            if (_up != null && _up.Dao != null)
            {
                _up.Dao.Close();
            }
        }
        public override string Description
        {
            get { return "DB对象"; }
        }

        public override string Name
        {
            get { return "DB"; }
        }
    }
    #endregion
    #region Input
    public class InputObject : BaseHostJsObject
    {
        WebBaseLogic<WebParameter, GoData> _logic = null;
        LogicData _ld = null;
        public InputObject(WebBaseLogic<WebParameter, GoData> logic)
        {
            _logic = logic;
            _ld = new LogicData();
            if (_logic.CallContext_Parameter != null)
            {
                //添加querystring
                foreach (var s in _logic.CallContext_Parameter.Domain(DomainKey.QUERY_STRING))
                {
                    _ld.SetValue(s.Key, s.Value);
                }
                //添加postback数据
                foreach (var s in _logic.CallContext_Parameter.Domain(DomainKey.POST_DATA))
                {
                    _ld.SetValue(s.Key, s.Value);
                }
            }
        }
        [Desc("获取参数值")]
        public object GetValue(string key)
        {
            return _ld.GetValue(key);
        }
        public override string Description
        {
            get { return "输入参数对象"; }
        }

        public override string Name
        {
            get { return "Input"; }
        }
    }
    #endregion
    #region Logic
    public class LogicObject : BaseHostJsObject
    {
        WebBaseLogic<WebParameter, GoData> _logic = null;
        public LogicObject(WebBaseLogic<WebParameter, GoData> logic)
        {
            _logic = logic;
        }
        [Desc("设定ContentType,ct值为：Json（默认）,HTML,String,Pic_Jpg,Pic_Bmp,Pic_Gif,Pic_png,Excel,Word,PDF,HostView")]
        public void SetContentType(string ct)
        {
            _logic.CallContext_DataCollection.ContentType = ComFunc.EnumParse<GoResponseDataType>(ct);
        }
        [Desc("设定hostview的路径，~为根路径")]
        public void SetHostViewPath(string path)
        {
            _logic.CallContext_DataCollection.ExtentionObj.hostviewpath = path;
        }
        [Desc("开启事务")]
        public void BeginTrans()
        {
            _logic.BeginTrans();
        }
        [Desc("提交事务")]
        public void CommitTrans()
        {
            _logic.CommitTrans();
        }
        [Desc("回滚事务")]
        public void RollBackTrans()
        {
            _logic.RollBack();
        }
        public override string Description
        {
            get { return "Logic相关参数设定"; }
        }

        public override string Name
        {
            get { return "Logic"; }
        }
    }
    #endregion
    #region ComFunc
    public class ComFuncObject : BaseHostJsObject
    {
        public ComFuncObject()
        {
        }
        [Desc("将json对象转化成string")]
        public string ToJSONString(Dictionary<string, object> obj)
        {
            FrameDLRObject dobj = FrameDLRObject.CreateInstance(obj, FrameDLRFlags.SensitiveCase);
            return dobj.ToJSONString();
        }
        [Desc("获取一个新的GUID")]
        public string NewGUID()
        {
            return Guid.NewGuid().ToString();
        }
        [Desc("对日期进行格式化")]
        public string FormatDateTime(DateTime dt, string format)
        {
            return dt.ToString(format);
        }
        [Desc("对object执行nvl操作，将对象转化成string，如果为null，则返回空串")]
        public string nvl(object obj)
        {
            return ComFunc.nvl(obj);
        }
        [Desc("对字符串进行url encode处理")]
        public string UrlEncode(string str)
        {
            return System.Web.HttpUtility.UrlEncode(str, Encoding.UTF8);
        }
        [Desc("对字符串进行url decode处理")]
        public string UrlDecode(string str)
        {
            return System.Web.HttpUtility.UrlDecode(str, Encoding.UTF8);
        }
        [Desc("对字符串进行base64 encode处理")]
        public string Base64Encode(string str)
        {
            return ComFunc.Base64Code(str);
        }
        [Desc("对字符串进行base64 decode处理")]
        public string Base64Decode(string str)
        {
            return ComFunc.Base64DeCode(str);
        }
        public override string Description
        {
            get { return "Common Function"; }
        }

        public override string Name
        {
            get { return "ComFunc"; }
        }
    }
    #endregion
    #region FrameCache
    public class FrameCacheObject : BaseHostJsObject
    {
        WebBaseLogic<WebParameter, GoData> _logic = null;
        public FrameCacheObject(WebBaseLogic<WebParameter, GoData> logic)
        {
            _logic = logic;
        }
        [Desc("获取缓存值")]
        public object GetCache(string key)
        {
            var obj = _logic.CacheHelper.GetCache(key);
            if (obj is FrameDLRObject)
            {
                return ((FrameDLRObject)obj).ToDictionary();
            }
            else
            {
                return obj;
            }
        }
        [Desc("写入缓存值")]
        public void SetCache(string key, object value, DateTime expiration)
        {
            _logic.CacheHelper.SetCache(key, value, expiration);
        }
        [Desc("移除缓存值")]
        public void RemoveCache(string key)
        {
            _logic.CacheHelper.RemoveCache(key);
        }
        public override string Name
        {
            get { return "Cache"; }
        }

        public override string Description
        {
            get { return "框架缓存器"; }
        }
    }
    #endregion
    #region ThreadLock
    public class LockObject : BaseHostJsObject
    {
        WebBaseLogic<WebParameter, GoData> _logic = null;
        HostJsLockObejctEntity _locks = null;
        public LockObject(WebBaseLogic<WebParameter, GoData> logic)
        {
            _logic = logic;
            var workarea = (_logic.RequestResources != null && _logic.RequestResources.Length > 0) ? _logic.RequestResources[0] : _logic.Name;

            _locks = new HostJsLockObejctEntity(workarea);
            _logic.CallContext_ResourceManage.AddEntity(_logic.CallContext_CurrentToken, _locks);
        }
        [Desc("锁定一个对象，lockname为锁名称，大小写敏感")]
        public void Lock(string lockname)
        {
            _locks.Lock(lockname);
        }
        [Desc("释放一个锁对象，lockname为锁名称，大小写敏感")]
        public void Exit(string lockname)
        {
            _locks.Free(lockname);
        }
        public override string Name
        {
            get { return "Lock"; }
        }

        public override string Description
        {
            get { return "代码锁对象，用于对一段代码进行同步化处理，该锁对象作用于本Logic"; }
        }
    }
    #endregion
    #region File
    public class FileObject : BaseHostJsObject
    {
        WebBaseLogic<WebParameter, GoData> _logic = null;
        public FileObject(WebBaseLogic<WebParameter, GoData> logic)
        {
            _logic = logic;
        }
        [Desc("读取一个文件,path可以为服务器所在的物理路径，也可以为http的远程路径")]
        public byte[] ReadFileByte(string path)
        {
            var rpath = path.Replace("~", GlobalCommon.HostCommon.RootPath);
            //以http的方式读取文件
            if (rpath.ToLower().StartsWith("http"))
            {
                WebClient client = new WebClient();
                client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                client.Headers.Add("UserAgent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                return client.DownloadData("https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=gQES8DoAAAAAAAAAASxodHRwOi8vd2VpeGluLnFxLmNvbS9xL0ZFejJzcDNseTkyeWFkRUZaMkp1AAIEYyPgVQMECAcAAA==");
            }
            else
            {
                return File.ReadAllBytes(rpath);
            }
        }
        [Desc("以Uft8的方式读取一个文本文件的内容")]
        public string ReadFileText(string path)
        {
            var rpath = path.Replace("~", GlobalCommon.HostCommon.RootPath);
            return File.ReadAllText(rpath, Encoding.UTF8);
        }
        [Desc("以覆盖的方式写入一个文本文件,采用UTF8的编码方式")]
        public void WriteFileText(string path, string content)
        {
            var rpath = path.Replace("~", GlobalCommon.HostCommon.RootPath);
            File.WriteAllText(rpath, content, Encoding.UTF8);
        }
        [Desc("以覆盖的方式写入一个文本文件")]
        public void WriteFileByte(string path, byte[] content)
        {
            var rpath = path.Replace("~", GlobalCommon.HostCommon.RootPath);
            File.WriteAllBytes(rpath, content);
        }
        public override string Name
        {
            get { return "Cache"; }
        }

        public override string Description
        {
            get { return "框架缓存器"; }
        }
    }
    #endregion
}
