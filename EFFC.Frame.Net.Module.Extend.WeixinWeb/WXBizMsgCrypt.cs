using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace EFFC.Frame.Net.Module.Extend.WeixinWeb
{
    internal class WXBizMsgCrypt
    {
        string m_sToken;
        string m_sEncodingAESKey;
        string m_sAppID;
        public enum WXBizMsgCryptErrorCode
        {
            WXBizMsgCrypt_OK = 0,
            WXBizMsgCrypt_ValidateSignature_Error = -40001,
            WXBizMsgCrypt_ParseXml_Error = -40002,
            WXBizMsgCrypt_ComputeSignature_Error = -40003,
            WXBizMsgCrypt_IllegalAesKey = -40004,
            WXBizMsgCrypt_ValidateAppid_Error = -40005,
            WXBizMsgCrypt_EncryptAES_Error = -40006,
            WXBizMsgCrypt_DecryptAES_Error = -40007,
            WXBizMsgCrypt_IllegalBuffer = -40008,
            WXBizMsgCrypt_EncodeBase64_Error = -40009,
            WXBizMsgCrypt_DecodeBase64_Error = -40010
        };

        //构造函数
        // @param sToken: 公众平台上，开发者设置的Token
        // @param sEncodingAESKey: 公众平台上，开发者设置的EncodingAESKey
        // @param sAppID: 公众帐号的appid
        public WXBizMsgCrypt(string sToken, string sEncodingAESKey, string sAppID)
        {
            m_sToken = sToken;
            m_sAppID = sAppID;
            m_sEncodingAESKey = sEncodingAESKey;
        }


        // 检验消息的真实性，并且获取解密后的明文
        // @param sMsgSignature: 签名串，对应URL参数的msg_signature
        // @param sTimeStamp: 时间戳，对应URL参数的timestamp
        // @param sNonce: 随机串，对应URL参数的nonce
        // @param sPostData: 密文，对应POST请求的数据
        // @param sMsg: 解密后的原文，当return返回0时有效
        // @return: 成功0，失败返回对应的错误码
        public WXBizMsgCryptErrorCode DecryptMsg(string sMsgSignature, string sTimeStamp, string sNonce, string sPostData, ref string sMsg)
        {
            if (m_sEncodingAESKey.Length != 43)
            {
                return WXBizMsgCryptErrorCode.WXBizMsgCrypt_IllegalAesKey;
            }
            XmlDocument doc = new XmlDocument();
            XmlNode root;
            string sEncryptMsg;
            try
            {
                doc.LoadXml(sPostData);
                root = doc.FirstChild;
                sEncryptMsg = root["Encrypt"].InnerText;
            }
            catch (Exception)
            {
                return WXBizMsgCryptErrorCode.WXBizMsgCrypt_ParseXml_Error;
            }
            //verify signature
            WXBizMsgCryptErrorCode ret = 0;
            ret = VerifySignature(m_sToken, sTimeStamp, sNonce, sEncryptMsg, sMsgSignature);
            if (ret != 0)
                return ret;
            //decrypt
            string cpid = "";
            try
            {
                sMsg = Cryptography.AES_decrypt(sEncryptMsg, m_sEncodingAESKey, ref cpid);
            }
            catch (FormatException)
            {
                return WXBizMsgCryptErrorCode.WXBizMsgCrypt_DecodeBase64_Error;
            }
            catch (Exception)
            {
                return WXBizMsgCryptErrorCode.WXBizMsgCrypt_DecryptAES_Error;
            }
            if (cpid != m_sAppID)
                return WXBizMsgCryptErrorCode.WXBizMsgCrypt_ValidateAppid_Error;
            return 0;
        }

        //将企业号回复用户的消息加密打包
        // @param sReplyMsg: 企业号待回复用户的消息，xml格式的字符串
        // @param sTimeStamp: 时间戳，可以自己生成，也可以用URL参数的timestamp
        // @param sNonce: 随机串，可以自己生成，也可以用URL参数的nonce
        // @param sEncryptMsg: 加密后的可以直接回复用户的密文，包括msg_signature, timestamp, nonce, encrypt的xml格式的字符串,
        //						当return返回0时有效
        // return：成功0，失败返回对应的错误码
        public WXBizMsgCryptErrorCode EncryptMsg(string sReplyMsg, string sTimeStamp, string sNonce, ref string sEncryptMsg)
        {
            if (m_sEncodingAESKey.Length != 43)
            {
                return WXBizMsgCryptErrorCode.WXBizMsgCrypt_IllegalAesKey;
            }
            string raw = "";
            try
            {
                raw = Cryptography.AES_encrypt(sReplyMsg, m_sEncodingAESKey, m_sAppID);
            }
            catch (Exception)
            {
                return WXBizMsgCryptErrorCode.WXBizMsgCrypt_EncryptAES_Error;
            }
            string MsgSigature = "";
            var ret = WXBizMsgCryptErrorCode.WXBizMsgCrypt_OK;
            ret = GenarateSinature(m_sToken, sTimeStamp, sNonce, raw, ref MsgSigature);
            if (WXBizMsgCryptErrorCode.WXBizMsgCrypt_OK != ret)
                return ret;
            sEncryptMsg = "";

            string EncryptLabelHead = "<Encrypt><![CDATA[";
            string EncryptLabelTail = "]]></Encrypt>";
            string MsgSigLabelHead = "<MsgSignature><![CDATA[";
            string MsgSigLabelTail = "]]></MsgSignature>";
            string TimeStampLabelHead = "<TimeStamp><![CDATA[";
            string TimeStampLabelTail = "]]></TimeStamp>";
            string NonceLabelHead = "<Nonce><![CDATA[";
            string NonceLabelTail = "]]></Nonce>";
            sEncryptMsg = sEncryptMsg + "<xml>" + EncryptLabelHead + raw + EncryptLabelTail;
            sEncryptMsg = sEncryptMsg + MsgSigLabelHead + MsgSigature + MsgSigLabelTail;
            sEncryptMsg = sEncryptMsg + TimeStampLabelHead + sTimeStamp + TimeStampLabelTail;
            sEncryptMsg = sEncryptMsg + NonceLabelHead + sNonce + NonceLabelTail;
            sEncryptMsg += "</xml>";
            return 0;
        }



        public class DictionarySort : System.Collections.IComparer
        {
            public int Compare(object oLeft, object oRight)
            {
                string sLeft = oLeft as string;
                string sRight = oRight as string;
                int iLeftLength = sLeft.Length;
                int iRightLength = sRight.Length;
                int index = 0;
                while (index < iLeftLength && index < iRightLength)
                {
                    if (sLeft[index] < sRight[index])
                        return -1;
                    else if (sLeft[index] > sRight[index])
                        return 1;
                    else
                        index++;
                }
                return iLeftLength - iRightLength;

            }
        }
        //Verify Signature
        private static WXBizMsgCryptErrorCode VerifySignature(string sToken, string sTimeStamp, string sNonce, string sMsgEncrypt, string sSigture)
        {
            string hash = "";
            var ret = WXBizMsgCryptErrorCode.WXBizMsgCrypt_OK;
            ret = GenarateSinature(sToken, sTimeStamp, sNonce, sMsgEncrypt, ref hash);
            if (ret != 0)
                return ret;
            //System.Console.WriteLine(hash);
            if (hash == sSigture)
                return 0;
            else
            {
                return WXBizMsgCryptErrorCode.WXBizMsgCrypt_ValidateSignature_Error;
            }
        }

        public static WXBizMsgCryptErrorCode GenarateSinature(string sToken, string sTimeStamp, string sNonce, string sMsgEncrypt, ref string sMsgSignature)
        {
            ArrayList AL = new ArrayList();
            AL.Add(sToken);
            AL.Add(sTimeStamp);
            AL.Add(sNonce);
            AL.Add(sMsgEncrypt);
            AL.Sort(new DictionarySort());
            string raw = "";
            for (int i = 0; i < AL.Count; ++i)
            {
                raw += AL[i];
            }

            SHA1 sha;
            ASCIIEncoding enc;
            string hash = "";
            try
            {
                sha = SHA1.Create();
                enc = new ASCIIEncoding();
                byte[] dataToHash = enc.GetBytes(raw);
                byte[] dataHashed = sha.ComputeHash(dataToHash);
                hash = BitConverter.ToString(dataHashed).Replace("-", "");
                hash = hash.ToLower();
            }
            catch (Exception)
            {
                return WXBizMsgCryptErrorCode.WXBizMsgCrypt_ComputeSignature_Error;
            }
            sMsgSignature = hash;
            return WXBizMsgCryptErrorCode.WXBizMsgCrypt_OK;
        }
        #region added by chuan.yin in 2015/9/24
        /// <summary>
        /// 生成微信服务访问用的签名，访问微信服务器或访问.wx的服务需要用到的签证
        /// </summary>
        /// <param name="token"></param>
        /// <param name="timestamp"></param>
        /// <param name="noncestr"></param>
        /// <returns></returns>
        public static string GenWeixinVisitSign(string token, string timestamp, string noncestr)
        {
            string[] infos = new string[] { token, timestamp, noncestr };
            Array.Sort<string>(infos);
            string info = string.Format("{0}{1}{2}", infos[0], infos[1], infos[2]);
            return WXBizMsgCrypt.GenSHASignKey(info, Encoding.ASCII);
        }
        /// <summary>
        /// 返回SHA1哈希字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GenSHASignKey(string input, Encoding encoding)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(input) && encoding != null)
            {
                byte[] bytes = encoding.GetBytes(input);
                SHA1 sha1 = SHA1.Create();
                byte[] result = sha1.ComputeHash(bytes);
                foreach (byte b in result)
                    sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 根据参数采用MD5算法生成数字签名
        /// </summary>
        /// <param name="obj">参数集，该参数集不可包含数字签名的栏位</param>
        /// <param name="mch_key">微信商户支付秘钥</param>
        /// <returns></returns>
        public static string GenMD5SignString(FrameDLRObject obj, string mch_key)
        {
            SortedDictionary<string, object> p = new SortedDictionary<string, object>();
            foreach (var k in obj.Keys)
            {
                p.Add(k, obj.GetValue(k));
            }
            string buff = "";
            foreach (KeyValuePair<string, object> pair in p)
            {
                if (pair.Key != "sign" && ComFunc.nvl(pair.Value) != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            buff += "&key=" + mch_key;

            //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(buff));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString().ToUpper();
        }
        /// <summary>
        /// 获取SHA算法的Sign签名
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GenSHASignKey(FrameDLRObject obj, Encoding encoding)
        {
            SortedDictionary<string, object> p = new SortedDictionary<string, object>();
            foreach (var k in obj.Keys)
            {
                p.Add(k, obj.GetValue(k));
            }
            string buff = "";
            foreach (KeyValuePair<string, object> pair in p)
            {
                if (pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');

            return GenSHASignKey(buff, encoding);
        }
        /// <summary>
        /// 获取SHA算法的Sign签名,ASCII编码
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GenSHASignKey(FrameDLRObject obj)
        {
            return GenSHASignKey(obj, Encoding.ASCII);
        }
        #endregion
    }
}
