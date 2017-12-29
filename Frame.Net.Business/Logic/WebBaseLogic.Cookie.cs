using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Base.Unit;
using EFFC.Frame.Net.Data;
using EFFC.Frame.Net.Data.LogicData;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.UnitData;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;

namespace EFFC.Frame.Net.Business.Logic
{
    public partial class WebBaseLogic<P, D>
    {
        private CookieHelper _cookie;
        /// <summary>
        /// cache操作
        /// </summary>
        public virtual CookieHelper Cookie
        {
            get
            {
                if (_cookie == null)
                    _cookie = new CookieHelper(this);
                return _cookie;
            }
        }

        public class CookieHelper
        {
            WebBaseLogic<P, D> _logic;
            public CookieHelper(WebBaseLogic<P, D> logic)
            {
                _logic = logic;
            }
            /// <summary>
            /// 新增一个cookie
            /// </summary>
            /// <param name="name">名称</param>
            /// <param name="value">值</param>
            /// <param name="domain">cookie对应的域，如果不需要则填写null</param>
            /// <param name="expiretime">过期时间</param>
            public void SetCookie(string name, string value, string domain, DateTime expiretime)
            {
                if (_logic.CallContext_Parameter.ExtentionObj.cookie.add == null)
                {
                    _logic.CallContext_Parameter.ExtentionObj.cookie.add = FrameDLRObject.CreateInstance();
                }
                FrameDLRObject item = FrameDLRObject.CreateInstance();
                item.SetValue("name",name);
                item.SetValue("value",value);
                item.SetValue("domain",domain);
                item.SetValue("expire",expiretime);
                ((FrameDLRObject)_logic.CallContext_Parameter.ExtentionObj.cookie.add).SetValue(name, item);
            }
            /// <summary>
            /// 新增一个cookie，采用默认的域和时间
            /// </summary>
            /// <param name="name"></param>
            /// <param name="value"></param>
            public void SetCookie(string name, string value)
            {
                SetCookie(name, value, null, DateTime.MinValue);
            }
            /// <summary>
            /// 获取cookie的值，如果没有值则返回空串
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public string GetCookie(string name)
            {
                if (_logic.CallContext_Parameter.ExtentionObj.cookie != null
                    && ((FrameDLRObject)_logic.CallContext_Parameter.ExtentionObj.cookie).GetValue(name) != null)
                {
                    var item = (FrameDLRObject)((FrameDLRObject)_logic.CallContext_Parameter.ExtentionObj.cookie).GetValue(name);
                    return ComFunc.nvl(item.GetValue("value"));
                }
                else
                {
                    return "";
                }
            }
            /// <summary>
            /// 删除一个cookie
            /// </summary>
            /// <param name="name"></param>
            public void RemoveCookie(string name)
            {
                if (_logic.CallContext_Parameter.ExtentionObj.cookie.remove == null)
                {
                    _logic.CallContext_Parameter.ExtentionObj.cookie.remove = FrameDLRObject.CreateInstance();
                }
                FrameDLRObject item = FrameDLRObject.CreateInstance();
                item.SetValue("name", name);
                ((FrameDLRObject)_logic.CallContext_Parameter.ExtentionObj.cookie.remove).SetValue(name, item);
            }
        }
    }
}
