using EFFC.Frame.Net.Base.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Test
{
    public class MobileAPITest
    {
        static string url = "http://221.178.251.172:8080/tsms_op_service/router";
        public static void Test()
        {
            Console.WriteLine(ComFunc.getMD5_String((ComFunc.getMD5_String("123").ToLower() + "9653")).ToLower());
            //Console.WriteLine(getsignid_jxs("ZYTEST", "DF10F4AA148004B4FE0E90FF6407F9A1"));
        }
        //江苏DMS经销商获取sign
        public static string getsignid_jxs(string appkey_jxs, string secret_jxs)
        {
            string errMsg = "", sign = "";
            string sessionid = "", responseData = "";
            //E6GPSIntfhelper E6GPSIntfhelper = new E6GPSIntfhelper(_connStr, _url, null, null, null, null);
            //得到签名   
            SortedDictionary<string, string> parameter = new SortedDictionary<string, string>();
            parameter.Add("method", "user.login");
            parameter.Add("appKey", appkey_jxs);
            parameter.Add("v", "1.0");
            parameter.Add("format", "xml");
            parameter.Add("locale", "zh_CN");
            sign = SignRequest(parameter, secret_jxs, true);

            //拼接字符串
            SortedDictionary<string, string> parameter1 = new SortedDictionary<string, string>();
            parameter1.Add("sign", sign);
            parameter1.Add("v", "1.0");
            parameter1.Add("method", "user.login");
            parameter1.Add("format", "xml");
            parameter1.Add("locale", "zh_CN");
            parameter1.Add("appKey", appkey_jxs);
            string sessionidXML = PostURL3(url, parameter1, out errMsg);
            XmlDocument xd = new XmlDocument();
            //防xxe攻击
            xd.XmlResolver = null;
            xd.LoadXml(sessionidXML);
            sessionid = xd.SelectSingleNode("loginResponse").Attributes["sessionId"].Value.ToString();

            return sessionid;
        }
        public static string PostURL3(string url, SortedDictionary<string, string> strXML, out string err)
        {
            err = "OK";
            System.Net.ServicePointManager.Expect100Continue = false;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                byte[] requestBytes = System.Text.Encoding.ASCII.GetBytes(QueryString(strXML));
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = requestBytes.Length;
                req.Timeout = 60000;
                req.ReadWriteTimeout = 60000;
                Stream requestStream = req.GetRequestStream();
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.UTF8);
                string backstr = sr.ReadToEnd();
                requestStream = null;
                sr.Close();
                res.Close();
                return backstr;
            }
            catch (Exception ep)
            {
                err = ep.Message;
                return err;
            }
            finally
            {
                req = null;
            }
        }

        public static string QueryString(IDictionary<string, string> dict)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in dict)
            {
                builder.Append(item.Key).Append("=").Append(item.Value).Append("&");
            }
            return builder.ToString().Trim('&');
        }

        //获取加密字符串
        /// <summary>
        /// 给请求签名。
        /// </summary>
        /// <param name="parameters">所有字符型的请求参数</param>
        /// <param name="secret">签名密钥</param>
        /// <param name="qhs">是否前后都加密钥进行签名</param>
        /// <returns>签名</returns>
        public static string SignRequest(IDictionary<string, string> parameters, string secret, bool qhs)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters, StringComparer.Ordinal);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder(secret);
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    query.Append(key).Append(value);
                }
            }
            if (qhs)
            {
                query.Append(secret);
            }
            // query.Append("abcdefage24appKey000001formatxmllocalezh_CNmethoduser.createsessionIdAAAAsex1userN ametomsonv1.0abcdef");
            // 第三步：使用SHA1加密
            byte[] bytes = SHA1.Create("SHA1").ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));
            StringBuilder sign = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                string str16 = (bytes[i] & 0xFF).ToString("X");
                if (str16.Length == 1)
                {
                    sign.Append("0");
                }
                sign.Append(str16);
            }
            string sign1 = sign.ToString();
            // 第四步：转16进制
            string a = BitConverter.ToString(bytes).Replace("-", string.Empty);
            return a;

        }
    }
}
