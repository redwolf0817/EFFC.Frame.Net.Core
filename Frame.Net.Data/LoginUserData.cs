using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;

namespace EFFC.Frame.Net.Data
{
    [Serializable]
    public class LoginUserData : DataCollection, IJSONable, ISerializable
    {
        public enum SexType
        {
            Male,
            Female
        }

        public LoginUserData()
        {
            SetValue("UserID", "");
            SetValue("UserName", "");
            SetValue("RoleID", "");
            SetValue("RoleName", "");
            SetValue("GroupID", "");
            SetValue("GroupName", "");
            SetValue("TeamID", "");
            SetValue("TeamName", "");
            SetValue("AgentID", "");
            SetValue("AgentName", "");
            SetValue("Actions", new ActionCollection());
            SetValue("LoginTime", DateTime.MinValue);
            SetValue("ForceLogoutTimeOut", 30);
            SetValue("LoginIPAddress", "");
            SetValue("UserSex", SexType.Male);
        }
        public override object Clone()
        {
            return this.Clone<LoginUserData>();
        }
        /// <summary>
        /// 登录者的ID
        /// </summary>
        public string UserID
        {
            get { return ComFunc.nvl(GetValue("UserID")); }
            set { SetValue("UserID", value); }
        }
        /// <summary>
        /// 登陆账号
        /// </summary>
        public string AccountID
        {
            get { return ComFunc.nvl(GetValue("AccountID")); }
            set { SetValue("AccountID", value); }
        }
        /// <summary>
        /// SourceType
        /// </summary>
        public string SourceType
        {
            get { return ComFunc.nvl(GetValue("SourceType")); }
            set { SetValue("SourceType", value); }
        }
        /// <summary>
        /// 登录者的名称
        /// </summary>
        public string UserName
        {
            get { return ComFunc.nvl(GetValue("UserName")); }
            set { SetValue("UserName", value); }
        }
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleID
        {
            get { return ComFunc.nvl(GetValue("RoleID")); }
            set { SetValue("RoleID", value); }
        }
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleName
        {
            get { return ComFunc.nvl(GetValue("RoleName")); }
            set { SetValue("RoleName", value); }
        }
        /// <summary>
        /// 群组ID
        /// </summary>
        public string GroupID
        {
            get { return ComFunc.nvl(GetValue("GroupID")); }
            set { SetValue("GroupID", value); }
        }
        /// <summary>
        /// 群组名稱
        /// </summary>
        public string GroupName
        {
            get { return ComFunc.nvl(GetValue("GroupName")); }
            set { SetValue("GroupName", value); }
        }

        /// <summary>
        /// 組ID
        /// </summary>
        public string TeamID
        {
            get { return ComFunc.nvl(GetValue("TeamID")); }
            set { SetValue("TeamID", value); }
        }

        /// <summary>
        /// 組Name
        /// </summary>
        public string TeamName
        {
            get { return ComFunc.nvl(GetValue("TeamName")); }
            set { SetValue("TeamName", value); }
        }

        /// <summary>
        /// 被代理人的ID
        /// </summary>
        public string AgentID
        {
            get { return ComFunc.nvl(GetValue("AgentID")); }
            set { SetValue("AgentID", value); }
        }
        /// <summary>
        /// 被代理人的名称
        /// </summary>
        public string AgentName
        {
            get { return ComFunc.nvl(GetValue("AgentName")); }
            set { SetValue("AgentName", value); }
        }

        /// <summary>
        /// 上次登陆时间
        /// </summary>
        public DateTime LastLoginDatetime
        {
            get { return DateTimeStd.ParseStd(GetValue("LastLoginDatetime")).Value; }
            set { SetValue("LastLoginDatetime", value); }
        }
        /// <summary>
        /// 登陸時間
        /// </summary>
        public DateTime LoginTime
        {
            get { return DateTimeStd.ParseStd(GetValue("LoginTime")); }
            set { SetValue("LoginTime", value); }
        }
        /// <summary>
        /// 強制登出超時設定
        /// </summary>
        public int ForceLogoutTimeOut
        {
            get { return IntStd.ParseStd(GetValue("ForceLogoutTimeOut")); }
            set { SetValue("ForceLogoutTimeOut", value); }
        }
        /// <summary>
        /// 登陸者的IP地址
        /// </summary>
        public string LoginIPAddress
        {
            get { return ComFunc.nvl(GetValue("LoginIPAddress")); }
            set { SetValue("LoginIPAddress", value); }
        }
        /// <summary>
        /// 登陆者的性别
        /// </summary>
        public SexType UserSex
        {
            get { return (SexType)GetValue("UserSex"); }
            set { SetValue("UserSex", value); }
        }

        public ActionCollection Actions
        {
            get { return (ActionCollection)GetValue("Actions"); }
        }
        private LoginUserData(SerializationInfo info, StreamingContext context)
        {
            _info = info;
            _context = context;
            DeSerialization(SerializableType.Binary);
        }

        public override void DeSerialization(SerializableType st)
        {
            if (st == SerializableType.Binary)
            {
                string keys = _info.GetString("DatatdKeys#");
                string[] keyarray = keys.Split(';');
                foreach (string key in keyarray)
                {
                    string typename = _info.GetString(key + "_Type#");
                    if (!string.IsNullOrEmpty(typename))
                    {
                        Type t = Type.GetType(_info.GetString(key + "_Type#"));
                        SetValue(key, _info.GetValue(key, t));
                    }
                    else
                    {
                        SetValue(key, null);
                    }
                }
            }
            else
            {

            }
        }

        public dynamic ToJSONObject()
        {
            var rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            rtn.UserID = this.UserID;
            rtn.UserName = this.UserName;
            rtn.RoleID = this.RoleID;
            rtn.RoleName = this.RoleName;
            rtn.GroupID = this.GroupID;
            rtn.GroupName = this.GroupName;
            rtn.TeamID = this.TeamID;
            rtn.TeamName = TeamName;
            rtn.AgentID = AgentID;
            rtn.AgentName = AgentName;
            rtn.Actions = Actions;
            rtn.LoginTime = LoginTime;
            rtn.ForceLogoutTimeOut = ForceLogoutTimeOut;
            rtn.LoginIPAddress = LoginIPAddress;
            rtn.UserSex = UserSex;
            return rtn;
        }

        public string ToJSONString()
        {
            var f = (FrameDLRObject)ToJSONObject();
            return f.ToJSONString();
        }
    }

    /// <summary>
    /// 本权限模型基于FunctionID：页面=1：多。
    /// 每个FunctionID会针对权限进行ActionID的设定，FunctionID：ActionID=1：多
    /// </summary>
    [Serializable]
    public sealed class ActionCollection : IJSONable, ISerializable, IConvertible, IEnumerable<KeyValuePair<string, ActionEntity>>, ICloneable
    {
        /// <summary>
        /// functionid-action映射表
        /// </summary>
        Dictionary<string, ActionEntity> _d = new Dictionary<string, ActionEntity>();
        /// <summary>
        /// url-action映射表
        /// </summary>
        Dictionary<string, ActionEntity> _dbyurl = new Dictionary<string, ActionEntity>();
        /// <summary>
        /// 通配符映射表
        /// </summary>
        Dictionary<string, string> _wildcardsmap = new Dictionary<string, string>();
        public ActionCollection()
        {
        }

        public object Clone()
        {
            var rtn = new ActionCollection();
            foreach (var item in this._d)
            {
                rtn._d.Add(item.Key, (ActionEntity)item.Value.Clone());
            }
            foreach (var item in this._dbyurl)
            {
                rtn._dbyurl.Add(item.Key, (ActionEntity)item.Value.Clone());
            }
            foreach (var item in this._wildcardsmap)
            {
                rtn._wildcardsmap.Add(item.Key, item.Value);
            }

            return rtn;
        }

        private ActionCollection(SerializationInfo info, StreamingContext context)
        {

            string pre = "ActionCollection_";
            this._d = (Dictionary<string, ActionEntity>)info.GetValue(pre + "content", typeof(Dictionary<string, ActionEntity>));
            this._dbyurl = (Dictionary<string, ActionEntity>)info.GetValue(pre + "content_by_url", typeof(Dictionary<string, ActionEntity>));
            this._wildcardsmap = (Dictionary<string, string>)info.GetValue(pre + "content_wildcardsmap", typeof(Dictionary<string, string>));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            string pre = "ActionCollection_";
            info.AddValue(pre + "content", _d);
            info.AddValue(pre + "content_by_url", _dbyurl);
            info.AddValue(pre + "content_wildcardsmap", _wildcardsmap);
        }
        /// <summary>
        /// 采用通配符搜索action
        /// </summary>
        /// <param name="pageUrl"></param>
        /// <returns></returns>
        private ActionEntity GetActionByPageUrlUsingWideCards(string pageUrl)
        {
            if (_wildcardsmap.ContainsKey(pageUrl))
            {
                return _dbyurl[_wildcardsmap[pageUrl]];
            }
            else
            {
                //采用通配符进行搜索
                var ss = pageUrl.Split('.');
                var logic = "";
                var action = "";
                var requesttype = "";
                if (ss.Length == 2)
                {
                    logic = ss[0];
                    requesttype = ss[1];
                }
                else if (ss.Length > 2)
                {
                    logic = ss[0];
                    action = ss[1];
                    requesttype = ss[2];
                }

                var newurlkey = logic + ".*." + requesttype;
                var newurlkey2 = logic + ".*.*";
                if (_dbyurl.ContainsKey(newurlkey))
                {
                    _wildcardsmap.Add(pageUrl, newurlkey);
                    return _dbyurl[newurlkey];
                }
                else if (_dbyurl.ContainsKey(newurlkey2))
                {
                    _wildcardsmap.Add(pageUrl, newurlkey2);
                    return _dbyurl[newurlkey2];
                }
                else
                {
                    return new ActionEntity();
                }
            }
        }
        /// <summary>
        /// 通过PageUrl获取对应的FunctionID
        /// </summary>
        /// <param name="pageUrl">页面的URL</param>
        /// <returns></returns>
        public string GetFunctionIDByPageUrl(string pageUrl)
        {
            if (_dbyurl.ContainsKey(pageUrl))
            {
                return _dbyurl[pageUrl].FunctionID;
            }
            else
            {
                return ComFunc.nvl(GetActionByPageUrlUsingWideCards(pageUrl).FunctionID);
            }
        }

        /// <summary>
        /// 通过PageUrl获取对应的ParentFunctionID
        /// </summary>
        /// <param name="pageUrl">页面的URL</param>
        /// <returns></returns>
        public string GetParentFunctionIDByPageUrl(string pageUrl)
        {
            if (_dbyurl.ContainsKey(pageUrl))
            {
                return _dbyurl[pageUrl].ParentFunctionID;
            }
            else
            {
                return ComFunc.nvl(GetActionByPageUrlUsingWideCards(pageUrl).ParentFunctionID);
            }
        }

        /// <summary>
        /// 通过PageUrl获取对应的被授权的ActionID
        /// </summary>
        /// <param name="pageUrl">页面的URL</param>
        /// <returns>多个ActionID会以逗号连接</returns>
        public string GetActionIDsByPageUrl(string pageUrl)
        {
            string rtn = "";
            if (_dbyurl.ContainsKey(pageUrl))
            {
                foreach (string s in _dbyurl[pageUrl].Actions)
                {
                    rtn += rtn == "" ? s : "," + s;
                }
            }
            else
            {
                var action = GetActionByPageUrlUsingWideCards(pageUrl);
                foreach (string s in action.Actions)
                {
                    rtn += rtn == "" ? s : "," + s;
                }
            }

            return rtn;
        }
        /// <summary>
        /// 获取Action
        /// </summary>
        /// <param name="pageUrl"></param>
        /// <returns></returns>
        public List<string> GetActionsByPageUrl(string pageUrl)
        {
            List<string> rtn = new List<string>();
            if (_dbyurl.ContainsKey(pageUrl))
            {
                rtn = _dbyurl[pageUrl].Actions;
            }
            else
            {
                var action = GetActionByPageUrlUsingWideCards(pageUrl);
                foreach (string s in action.Actions)
                {
                    rtn = action.Actions;
                }
            }

            return rtn;
        }
        /// <summary>
        /// 根据FunctionID获取对应的ActionID
        /// </summary>
        /// <param name="functionID">功能编号</param>
        /// <returns>多个ActionID会以逗号连接</returns>
        public string GetActionIDsByFunctionID(string functionID)
        {
            string rtn = "";
            if (_d.ContainsKey(functionID))
            {
                foreach (string s in _dbyurl[functionID].Actions)
                {
                    rtn += rtn == "" ? s : "," + s;
                }
            }

            return rtn;
        }

        /// <summary>
        /// 根据FunctionID获取对应的ActionID
        /// </summary>
        /// <param name="functionID">功能编号</param>
        /// <returns>多个ActionID会以逗号连接</returns>
        public List<string> GetActionsByFunctionID(string functionID)
        {
            List<string> rtn = new List<string>();
            if (_d.ContainsKey(functionID))
            {
                rtn = _d[functionID].Actions;
            }

            return rtn;
        }
        public string GetFiledsByPageUrl(string pageUrl)
        {
            if (this._dbyurl.ContainsKey(pageUrl))
            {
                return this._dbyurl[pageUrl].Fileds;
            }
            return ComFunc.nvl(this.GetActionByPageUrlUsingWideCards(pageUrl).Fileds);
        }



        /// <summary>
        /// 新增一个Action设定
        /// </summary>
        /// <param name="functionID">FunctionID</param>
        /// <param name="parentFunctionId">parentFunctionId</param>
        /// <param name="pageurl">页面Url,如果对应多个url，则每个url以逗号分隔</param>
        /// <param name="actionID">ActionID</param>
        public void AddAction(string functionID, string parentFunctionId, string pageurl, string actionID)
        {
            if (!_d.ContainsKey(functionID))
            {
                ActionEntity ae = new ActionEntity();
                ae.FunctionID = functionID;
                ae.PageUrl = pageurl;
                var actionstr = actionID.Split(',');
                foreach (var s in actionstr)
                {
                    ae.AddAction(s);
                }
                ae.ParentFunctionID = parentFunctionId;

                _d.Add(ae.FunctionID, ae);
                var spliturl = ae.PageUrl.Split(',');
                //url可能沒有維護
                foreach (var url in spliturl)
                {
                    if (!_dbyurl.ContainsKey(url))
                    {
                        _dbyurl.Add(url, ae);
                    }
                }

            }
            else
            {
                _d[functionID].PageUrl = pageurl;
                var actionstr = actionID.Split(',');
                foreach (var s in actionstr)
                {
                    _d[functionID].AddAction(s);
                }
            }
        }
        /// <summary>
        /// 新增一个Action设定
        /// </summary>
        /// <param name="functionID">FunctionID</param>
        /// <param name="parentFunctionId">parentFunctionId</param>
        /// <param name="pageurl">页面Url,如果对应多个url，则每个url以逗号分隔</param>
        /// <param name="actionID">ActionID</param>
        /// <param name="Fileds"></param>
        public void AddAction(string functionID, string parentFunctionId, string pageurl, string actionID, string Fileds)
        {
            string[] strArray;
            if (!this._d.ContainsKey(functionID))
            {
                ActionEntity entity = new ActionEntity
                {
                    FunctionID = functionID,
                    PageUrl = pageurl,
                    Fileds = Fileds
                };
                strArray = actionID.Split(new char[] { ',' });
                foreach (string str in strArray)
                {
                    entity.AddAction(str);
                }
                entity.ParentFunctionID = parentFunctionId;
                this._d.Add(entity.FunctionID, entity);
                string[] strArray2 = entity.PageUrl.Split(new char[] { ',' });
                foreach (string str2 in strArray2)
                {
                    if (!this._dbyurl.ContainsKey(str2))
                    {
                        this._dbyurl.Add(str2, entity);
                    }
                }
            }
            else
            {
                this._d[functionID].PageUrl = pageurl;
                strArray = actionID.Split(new char[] { ',' });
                foreach (string str in strArray)
                {
                    this._d[functionID].AddAction(str);
                }
            }
        }


        /// <summary>
        /// 根据FunctionID写入新的actionID
        /// </summary>
        /// <param name="functionID">FunctionID</param>
        /// <param name="actionID">待写入的actionID</param>
        public void SetActionByFunctionID(string functionID, string actionID)
        {
            if (_d.ContainsKey(functionID))
            {
                var actionstr = actionID.Split(',');
                foreach (var s in actionstr)
                {
                    _d[functionID].AddAction(s);
                }
            }
        }
        /// <summary>
        /// 根页面写入新的actionID
        /// </summary>
        /// <param name="pageUrl">页面Url</param>
        /// <param name="actionID">待写入的ActionID</param>
        public void SetActionByPageUrl(string pageUrl, string actionID)
        {
            if (_dbyurl.ContainsKey(pageUrl))
            {
                var actionstr = actionID.Split(',');
                foreach (var s in actionstr)
                {
                    _dbyurl[pageUrl].AddAction(s);
                }
            }
            else
            {
                var action = GetActionByPageUrlUsingWideCards(pageUrl);
                var actionstr = actionID.Split(',');
                foreach (var s in actionstr)
                {
                    action.AddAction(s);
                }
            }
        }
        /// <summary>
        /// 判断是否含有权限设定
        /// </summary>
        public bool HasActions
        {
            get
            {
                if (_d.Values.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }



        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return false;
        }

        public byte ToByte(IFormatProvider provider)
        {
            return 0;
        }

        public char ToChar(IFormatProvider provider)
        {
            return new char();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return DateTime.Now;
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return 0;
        }

        public double ToDouble(IFormatProvider provider)
        {
            return 0;
        }

        public short ToInt16(IFormatProvider provider)
        {
            return 0;
        }

        public int ToInt32(IFormatProvider provider)
        {
            return 0;
        }

        public long ToInt64(IFormatProvider provider)
        {
            return 0;
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return 0;
        }

        public float ToSingle(IFormatProvider provider)
        {
            return 0;
        }

        public string ToString(IFormatProvider provider)
        {
            return this.GetType().FullName;
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType.FullName == this.GetType().FullName)
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return 0;
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return 0;
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return 0;
        }

        public IEnumerator<KeyValuePair<string, ActionEntity>> GetEnumerator()
        {
            return _d.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _d.GetEnumerator();
        }

        public dynamic ToJSONObject()
        {
            var rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            rtn.content = _d;
            rtn.content_by_url = _dbyurl;
            rtn.wild_cards_map = _wildcardsmap;
            return rtn;
        }

        public string ToJSONString()
        {
            return ((FrameDLRObject)ToJSONObject()).ToJSONString();
        }
    }

    [Serializable]
    public sealed class ActionEntity : ISerializable, IConvertible, ICloneable, IJSONable
    {
        private string _functionid = "";
        private string _pageurl = "";
        private string _parentfunctionid = "";
        private string _fileds = "";
        private List<string> _actions = new List<string>();

        public object Clone()
        {
            var rtn = new ActionEntity();
            rtn.FunctionID = this.FunctionID;
            rtn.PageUrl = this.PageUrl;
            rtn.ParentFunctionID = this.ParentFunctionID;
            rtn.Fileds = this.Fileds;
            foreach (var item in this.Actions)
            {
                rtn.Actions.Add(item);
            }

            return rtn;
        }

        public string FunctionID
        {
            get { return _functionid; }
            set { _functionid = value; }
        }

        public string ParentFunctionID
        {
            get { return _parentfunctionid; }
            set { _parentfunctionid = value; }
        }
        public string PageUrl
        {
            get { return _pageurl; }
            set { _pageurl = value; }
        }
        public string Fileds
        {
            get { return _fileds; }
            set { _fileds = value; }
        }
        public List<string> Actions
        {
            get { return _actions; }
        }
        /// <summary>
        /// 新增单个ActionID
        /// </summary>
        /// <param name="actionid"></param>
        public void AddAction(string actionid)
        {
            if (!_actions.Contains(actionid))
            {
                _actions.Add(actionid);
            }
        }
        /// <summary>
        /// 新增多个ActionID
        /// </summary>
        /// <param name="actionids"></param>
        public void AddActions(string actionids)
        {
            string[] ss = actionids.Split(',');
            foreach (string s in ss)
            {
                if (!_actions.Contains(s) && !string.IsNullOrEmpty(s))
                {
                    _actions.Add(s);
                }
            }
        }

        private ActionEntity(SerializationInfo info, StreamingContext context)
        {
            string pre = "ActionEntity_";
            this.FunctionID = ComFunc.nvl(info.GetValue(pre + "#functionid", typeof(string)));
            this.PageUrl = ComFunc.nvl(info.GetValue(pre + "#PageUrl", typeof(string)));
            this.ParentFunctionID = ComFunc.nvl(info.GetValue(pre + "#ParentFunctionID", typeof(string)));
            this._actions = (List<string>)info.GetValue(pre + "#functionid", typeof(List<string>));
        }

        public ActionEntity()
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            string pre = "ActionEntity_";
            info.AddValue(pre + "#functionid", this.FunctionID);
            info.AddValue(pre + "#PageUrl", this.PageUrl);
            info.AddValue(pre + "#ParentFunctionID", this.ParentFunctionID);
            info.AddValue(pre + "#Actions", this.Actions);
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return false;
        }

        public byte ToByte(IFormatProvider provider)
        {
            return 0;
        }

        public char ToChar(IFormatProvider provider)
        {
            return new char();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return DateTime.Now;
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return 0;
        }

        public double ToDouble(IFormatProvider provider)
        {
            return 0;
        }

        public short ToInt16(IFormatProvider provider)
        {
            return 0;
        }

        public int ToInt32(IFormatProvider provider)
        {
            return 0;
        }

        public long ToInt64(IFormatProvider provider)
        {
            return 0;
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return 0;
        }

        public float ToSingle(IFormatProvider provider)
        {
            return 0;
        }

        public string ToString(IFormatProvider provider)
        {
            return this.GetType().FullName;
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType.FullName == this.GetType().FullName)
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return 0;
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return 0;
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return 0;
        }

        public dynamic ToJSONObject()
        {
            var rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            rtn.actions = this._actions;
            rtn.fields = _fileds;
            rtn.functionid = _functionid;
            rtn.pageurl = _pageurl;
            rtn.parentid = _parentfunctionid;
            return rtn;
        }

        public string ToJSONString()
        {
            return ((FrameDLRObject)ToJSONObject()).ToJSONString();
        }
    }

}
