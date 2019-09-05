using EFFC.Frame.Net.Base.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace EFFC.Frame.Net.Resource.Others
{
    /// <summary>
    /// 邮件发送工具
    /// </summary>
    public class MailHelper : IResourceEntity
    {
        /// <summary>
        /// Mail发送初始化
        /// </summary>
        /// <param name="serverhost">mail服务器</param>
        /// <param name="loginid">登录账户</param>
        /// <param name="loginpass">登录密码</param>
        /// <param name="port">mail服务器端口号，默认0，即根据isSSL来决定用何种默认端口号</param>
        /// <param name="isSSL">是否使用SSL连接，默认为true</param>
        public MailHelper(string serverhost, string loginid, string loginpass, int port = 0, bool isSSL = true)
        {
            if (string.IsNullOrEmpty(serverhost)
                || string.IsNullOrEmpty(loginid)
                || string.IsNullOrEmpty(loginpass))
            {
                throw new ArgumentNullException("mailhelper:serverhost,loginid,loginpass are empty");
            }

            MailServerHost = serverhost;
            MailLoginID = loginid;
            MailLoginPassword = loginpass;
            MailIsSSL = isSSL;
            if (port == 0)
            {
                MailServerPort = MailIsSSL ? 465 : 25;
            }
            else
            {
                MailServerPort = port;
            }
        }
        /// <summary>
        /// 邮件服务器
        /// </summary>
        public string MailServerHost
        {
            get; protected set;
        }
        /// <summary>
        /// 邮件服务器的端口号
        /// </summary>
        public int MailServerPort { get; protected set; }
        /// <summary>
        /// 邮件发送是否需要使用SSL
        /// </summary>
        public bool MailIsSSL
        {
            get; protected set;
        }
        /// <summary>
        /// 邮件登录账户
        /// </summary>
        public string MailLoginID
        {
            get; protected set;
        }
        /// <summary>
        /// 邮件登录密码
        /// </summary>
        public string MailLoginPassword
        {
            get; protected set;
        }
        #region IResourceEntity
        string _id = $"Mail_{Guid.NewGuid().ToString()}";
        public string ID => _id;



        public void Release()
        {
        }
        #endregion
        /// <summary>
        /// 执行mail发送
        /// </summary>
        /// <param name="from">发送者邮箱</param>
        /// <param name="to">接收者邮箱，多个用逗号或分号分隔</param>
        /// <param name="title">邮件标题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="cc">抄送邮箱，多个用逗号或分号分隔</param>
        /// <param name="isHtml">内容是否为HTML格式</param>
        public bool SendMail(string from, string to, string title, string body, string cc = "", bool isHtml = false)
        {
            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    //发件人邮箱地址，方法重载不同，可以根据需求自行选择。
                    mailMessage.From = new MailAddress(from);
                    //收件人邮箱地址。
                    if (!string.IsNullOrEmpty(to))
                    {
                        foreach (var s in to.Replace(",", ";").Split(";"))
                        {
                            mailMessage.To.Add(new MailAddress(s));
                        }

                    }
                    //收件人邮箱地址。
                    if (!string.IsNullOrEmpty(cc))
                    {
                        foreach (var s in to.Replace(",", ";").Split(";"))
                        {
                            mailMessage.CC.Add(s);
                        }

                    }
                    mailMessage.IsBodyHtml = isHtml;
                    //邮件标题。
                    mailMessage.Subject = title;
                    //邮件内容。
                    mailMessage.Body = body;

                    using (SmtpClient client = new SmtpClient())
                    {
                        //在这里我使用的是qq邮箱，所以是smtp.qq.com，如果你使用的是126邮箱，那么就是smtp.126.com。
                        client.Host = MailServerHost;
                        client.Port = MailServerPort;
                        //使用安全加密连接。
                        client.EnableSsl = MailIsSSL;
                        //不和请求一块发送。
                        client.UseDefaultCredentials = false;
                        //验证发件人身份(发件人的邮箱，邮箱里的生成授权码);
                        client.Credentials = new NetworkCredential(MailLoginID, MailLoginPassword);

                        client.Send(mailMessage);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
