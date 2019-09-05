using EFFC.Frame.Net.Module.Extend.EConsole.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Module.Extend.EConsole.DataCollections;
using EFFC.Frame.Net.Module.Extend.EConsole.Parameters;
using EFFC.Frame.Net.Module.Business.Logic;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Constants;
using System.Reflection;
using System.Linq;
using EFFC.Frame.Net.Base.Exceptions;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Resource.Others;
using EFFC.Frame.Net.Module.HttpCall;

namespace EFFC.Frame.Net.Module.Extend.EBA
{
    public partial class ScheduleLogic
    {
        MailHelper _notice;
        SimpleRestCallHelper _restcall;
        /// <summary>
        /// mail通知工具
        /// </summary>
        public virtual MailHelper Mail
        {
            get
            {
                if (_notice == null)
                {
                    var server = ComFunc.nvl(Configs["Mail_ServerHost"]);
                    var isssl = ComFunc.nvl(Configs["Mail_IsSSL"]) == "" ? false : bool.Parse(ComFunc.nvl(Configs["Mail_IsSSL"]));
                    var loginid = ComFunc.nvl(Configs["Mail_Login_UserID"]);
                    var loginpass = ComFunc.nvl(Configs["Mail_Login_Password"]);
                    var port = IntStd.IsNotIntThen(Configs["Mail_ServerPort"], 0);
                    _notice = new MailHelper(server, loginid, loginpass, port, isssl);
                }
                return _notice;
            }
        }
        /// <summary>
        /// Rest呼叫工具
        /// </summary>
        public virtual SimpleRestCallHelper RestCall
        {
            get
            {
                if (_restcall == null) _restcall = new SimpleRestCallHelper();
                return _restcall;
            }
        }
    }
}
