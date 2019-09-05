using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Module.Extend.EWRA.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Extends.LinqDLR2SQL;
using System.IO;
using EFFC.Frame.Net.Unit.DB.Parameters;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Module.Extend.WeixinWeb.Logic;
using System.Linq;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Resource.SQLServer;
using EFFC.Frame.Net.Resource.Sqlite;

namespace RestAPISample.Business
{
    class MyAuth: WeixinAuthorizationLogic
    {
        MyAuthHelper _db = null;

        public new MyAuthHelper DB
        {
            get
            {
                if (_db == null) _db = new MyAuthHelper(this);
                return _db;
            }
        }

        static object lockobj = new object();
        public override TimeSpan Expire => TimeSpan.FromHours(24);
        protected override bool DoLogin(string id)
        {
            var type = ComFunc.nvl(PostDataD.type);
            return AdminLogin(id);
        }
        private bool AdminLogin(string id)
        {
            var up = DB.NewDBUnitParameter();
            var pw = PostDataD.pw;
            //登录模式，以下几种：Password-密码登录，AuthCode-验证码登录，OpenID-微信的OpenID方式登录；MP-微信小程序方式登录； 默认Password
            string login_mode = ComFunc.nvl(PostDataD.login_mode);
            if (login_mode == "") login_mode = "Password";
            if (!new string[] { "Password", "AuthCode", "OpenID", "MP" }.Contains(login_mode))
            {
                return false;
            }

            if (string.IsNullOrEmpty(id)) id = ComFunc.UrlDecode(ComFunc.nvl(PostDataD.id).ToLower());
            //小程序登录需要通过id(即jscode)换取openid和sessionkey,然后写入db
            var weixin_union_id = "";
            var weixinmp_session_key = "";
            if (login_mode == "MP")
            {
                var jscode = id;
                dynamic result = WeixinMP.GetSessionByCode(jscode);
                if (result != null && (ComFunc.nvl(result.errcode) == "" || result.errcode == 0))
                {
                    id = result.openid;
                    weixin_union_id = result.unionid;
                    weixinmp_session_key = result.session_key;
                }
                else
                {
                    id = "";
                }
            }

            var s = from t in DB.LamdaTable(up, "user_info", "a")
                    join t2 in DB.LamdaTable(up, "Auth_Code", "b").LeftJoin() on t.userid equals t2.AuthKey
                    where t.userid == id || t.WeixinID == id || t.PlatformID == id || t.Mobile == id || t.QQ == id || t.WeixinMPID == id || t.WeixinPlatUnionID == id
                    select new
                    {
                        t.UserID,
                        t.LoginPass,
                        t.UserName,
                        t.UserSex,
                        t.WeixinID,
                        t.WeixinMPID,
                        t.WeixinPlatUnionID,
                        t.HeadImgUrl,
                        t.PlatformID,
                        t2.AuthCode,
                        t2.ValidSeconds,
                        t2.StartTime,
                        t2.IsUsed
                    };

            BeginTrans();
            lock (lockobj)
            {
                var list = s.GetQueryList(up);
                if (login_mode == "MP")
                {
                    if (list.Count <= 0 && id != "")
                    {
                        var new_userid = NewUserID(up); ;

                        DB.QuickInsert(up, "user_info", new
                        {
                            userid = new_userid,
                            WeixinMPID = id,
                            WeixinPlatUnionID = weixin_union_id,
                            add_id = new_userid,
                            add_ip = ClientInfo.IP,
                            add_name = "",
                            add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            last_id = new_userid,
                            last_ip = ClientInfo.IP,
                            last_name = "",
                            last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        });
                       

                        list = s.GetQueryList(up);
                    }
                }

                if (list.Count != 1) return false;
                dynamic logininfo = list.First();

                var is_valid = false;
                if (login_mode == "Password" && logininfo.loginpass == pw)
                {
                    is_valid = true;
                }
                if (login_mode == "AuthCode")
                {
                    DateTime start_time = DateTimeStd.ParseStd(logininfo.StartTime).Value;
                    int valid_seconds = IntStd.IsNotIntThen(logininfo.ValidSeconds, 30);
                    if (start_time.AddSeconds(valid_seconds).CompareTo(DateTime.Now) >= 0 && pw == ComFunc.nvl(logininfo.AuthCode))
                    {
                        is_valid = true;
                    }
                }
                if (login_mode == "OpenID" && logininfo.WeixinID == id)
                {
                    is_valid = true;
                }
                if (login_mode == "MP" && logininfo.WeixinMPID == id)
                {
                    DB.QuickDelete(up, "WeixinMP_SessionKey", new
                    {
                        UserID = logininfo.UserID
                    });
                    DB.QuickInsert(up, "WeixinMP_SessionKey", new
                    {
                        UserID = logininfo.UserID,
                        SessionKey = weixinmp_session_key,
                        add_id = logininfo.UserID,
                        add_ip = ClientInfo.IP,
                        add_name = "",
                        add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        last_id = logininfo.UserID,
                        last_ip = ClientInfo.IP,
                        last_name = "",
                        last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    });

                    is_valid = true;
                }

                if (!is_valid)
                {
                    if (login_mode != "MP")
                    {
                        DB.QuickUpdate(up, "user_info", new
                        {
                            ErrorTime = IntStd.IsNotIntThen(logininfo.ErrorTime) + 1,
                            LastLoginDate = DateTime.Now,
                            LastLoginIP = ClientInfo.IP
                        }, new { UserID = logininfo.UserID });
                    }

                    return false;
                }

                DB.QuickUpdate(up, "user_info", new
                {
                    ErrorTime = 0,
                    LastLoginDate = DateTime.Now,
                    LastLoginIP = ClientInfo.IP
                }, new { UserID = logininfo.UserID });
                var private_info = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                private_info.weixin_id = logininfo.weixinid;
                private_info.weixinmp_id = logininfo.WeixinMPID;
                private_info.weixin_union_id = logininfo.WeixinPlatUnionID;
                private_info.platform_id = logininfo.PlatformID;
                //登录者的唯一编码
                SetClaimSaveParameter("user_id", logininfo.userid);
                SetClaimSaveParameter("sex", logininfo.UserSex);
                SetClaimSaveParameter("username", ComFunc.UrlEncode(logininfo.UserName));
                SetClaimSaveParameter("p_info", EncryptByPublicKey(((FrameDLRObject)private_info).ToJSONString(true)));
            }
            CommitTrans();
            return true;
        }


        protected override bool IsValid(string token, ref string msg)
        {
            var result = true;//base.IsValid(token, ref msg);

            return result;
        }
        /// <summary>
        /// 做自定义payload解析
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected override FrameDLRObject GetPayLoadInfoFromToken(string token)
        {
            var rtn = base.GetPayLoadInfoFromToken(token);
            if (ComFunc.nvl(rtn.GetValue("username")) != "")
            {
                rtn.SetValue("username", ComFunc.UrlDecode(ComFunc.nvl(rtn.GetValue("username"))));
            }
            var privateinfo_rsa = ComFunc.nvl(rtn.GetValue("p_info"));
            if (privateinfo_rsa != "")
            {
                var privateinfo_json = DecryptByPublicKey(privateinfo_rsa);
                FrameDLRObject dinfo = FrameDLRObject.CreateInstance(privateinfo_json);
                foreach (var item in dinfo.Items)
                {
                    rtn.SetValue(item.Key, item.Value);
                }
            }
            return rtn;
        }
        protected override bool IsValid_Level2(string token, FrameDLRObject payloadinfo, ref string msg)
        {
            return true;
        }
        public override string Issuer => "Your_Issuer";
        public override string Audience => "Your_Audience";
        public override string PublicKeySavePath => "~/token/public.json";
        public override string PrivateKeySavePath => "~/token/private.json";
        /// <summary>
        /// 通过公钥做key进行加密,AES算法
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string EncryptByPublicKey(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            var content = File.ReadAllText(PublicKeySavePath.Replace("~", CallContext_Parameter.ServerRootPath));
            var key = ComFunc.getMD5_String(ComFunc.Base64Code(content));


            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(text);

            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7
            };

            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        /// 通过公钥做key进行解密,AES算法
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string DecryptByPublicKey(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            var content = File.ReadAllText(PublicKeySavePath.Replace("~", CallContext_Parameter.ServerRootPath));
            var key = ComFunc.getMD5_String(ComFunc.Base64Code(content));

            Byte[] toEncryptArray = Convert.FromBase64String(text);

            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7
            };

            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
        private string NewUserID(UnitParameter up)
        {
            var s = from t in DB.LamdaTable(up, "user_info", "a")
                    where t.userid.startwith(DateTime.Now.ToString("yyyyMMdd"))
                    select t;
            var maxid = Int64Std.IsNotInt64Then(s.Max(up, "userid"), Int64Std.ParseStd(DateTime.Now.ToString("yyyyMMdd") + "00000").Value);
            return (maxid + 1).ToString();

        }

        public class MyAuthHelper : MyDBHelper
        {
            MyAuth _logic = null;

            public MyAuthHelper(MyAuth logic)
                : base(logic)
            {
                _logic = logic;
            }
            /// <summary>
            /// 创建一个默认的DB访问参数
            /// </summary>
            /// <returns></returns>
            public override UnitParameter NewDBUnitParameter()
            {
                var dbtype = ComFunc.nvl(_logic.Configs["DB_Type"]);
                UnitParameter rtn = null;
                if (dbtype.ToLower() == "sqlserver")
                {
                    rtn = base.NewDBUnitParameter<SQLServerAccess>();
                }
                else if (dbtype.ToLower() == "mysql")
                {
                    rtn = base.NewDBUnitParameter<MySQLAccess>();
                }
                else if (dbtype.ToLower() == "oracle")
                {
                    rtn = base.NewDBUnitParameter<OracleAccess>();
                }
                else if (dbtype.ToLower() == "sqlite")
                {
                    rtn = base.NewDBUnitParameter<SqliteAccess>();
                }
                rtn.Dao.Open(_logic.CallContext_Parameter.DBConnectionString);
                return rtn;
            }
        }
    }
}
