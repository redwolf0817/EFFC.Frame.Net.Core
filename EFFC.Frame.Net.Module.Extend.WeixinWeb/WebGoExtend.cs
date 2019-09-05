using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Module.Web.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.WeixinWeb
{
    public class WebGoExtend : WebGo.WebGo
    {
        protected override void InvokeAction(WebParameter p, GoData d)
        {
            if (IsValid4Invoke(p, d))
                base.InvokeAction(p, d);
            else
            {
                throw new Exception("无效访问，可能原因，缺少访问授权信息或密钥不正确");
            }

        }
        /// <summary>
        /// 业务逻辑执行之前的检核判定，如果为true，则执行业务逻辑模块，否则不执行
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        protected virtual bool IsValid4Invoke(WebParameter p, GoData d)
        {
            return true;
        }

        protected override void LoadConfig(WebParameter p, GoData d)
        {
            base.LoadConfig(p, d);
            bool bvalue = true;
            foreach (var item in MyConfig.GetConfigurationList("Weixin"))
            {
                if (bool.TryParse(ComFunc.nvl(item.Value), out bvalue))
                {
                    p[DomainKey.CONFIG, item.Key] = bool.Parse(ComFunc.nvl(item.Value));
                }
                else if (DateTimeStd.IsDateTime(item.Value))
                {
                    p[DomainKey.CONFIG, item.Key] = DateTimeStd.ParseStd(item.Value).Value;
                }
                else
                {
                    p[DomainKey.CONFIG, item.Key] = ComFunc.nvl(item.Value);
                }
            }
            foreach (var item in MyConfig.GetConfigurationList("WeixinMP"))
            {
                if (bool.TryParse(ComFunc.nvl(item.Value), out bvalue))
                {
                    p[DomainKey.CONFIG, item.Key] = bool.Parse(ComFunc.nvl(item.Value));
                }
                else if (DateTimeStd.IsDateTime(item.Value))
                {
                    p[DomainKey.CONFIG, item.Key] = DateTimeStd.ParseStd(item.Value).Value;
                }
                else
                {
                    p[DomainKey.CONFIG, item.Key] = ComFunc.nvl(item.Value);
                }
            }
        }

        protected override void ProcessRequestInfo(WebParameter p, GoData d)
        {
            base.ProcessRequestInfo(p, d);
            //微信相关信息
            p.ExtentionObj.weixin = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            p.ExtentionObj.weixin.signature = ComFunc.nvl(p[DomainKey.QUERY_STRING, "signature"]);
            p.ExtentionObj.weixin.timestamp = ComFunc.nvl(p[DomainKey.QUERY_STRING, "timestamp"]);
            p.ExtentionObj.weixin.nonce = ComFunc.nvl(p[DomainKey.QUERY_STRING, "nonce"]);
            p.ExtentionObj.weixin.token = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_token"]);
            p.ExtentionObj.weixin.encrypt_type = ComFunc.nvl(p[DomainKey.QUERY_STRING, "encrypt_type"]);
            p.ExtentionObj.weixin.encrypt_key = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_encry_key"]);
            p.ExtentionObj.weixin.appid = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_Appid"]);
            p.ExtentionObj.weixin.appsecret = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_Appsecret"]);
            p.ExtentionObj.weixin.weixin_mch_ssl_path = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_Mch_SSL_Path"]);
            p.ExtentionObj.weixin.weixin_mch_ssl_pass = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_Mch_SSL_Pass"]);
            p.ExtentionObj.weixin.agentid = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_AgentId"]);

            p.ExtentionObj.weixinmp = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            p.ExtentionObj.weixinmp.appid = ComFunc.nvl(p[DomainKey.CONFIG, "weixinmp_Appid"]);
            p.ExtentionObj.weixinmp.appsecret = ComFunc.nvl(p[DomainKey.CONFIG, "weixinmp_Appsecret"]);
            p.ExtentionObj.weixinmp.weixinmp_mch_ssl_path = ComFunc.nvl(p[DomainKey.CONFIG, "weixinmp_Mch_SSL_Path"]);
            p.ExtentionObj.weixinmp.weixinmp_mch_ssl_pass = ComFunc.nvl(p[DomainKey.CONFIG, "weixinmp_Mch_SSL_Pass"]);
        }
    }
}
