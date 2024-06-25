using MailKit.Net.Smtp;
using MimeKit;

namespace DTcms.Core.Common.Helpers
{
    /// <summary>
    /// 邮件工具类
    /// </summary>
    public class MailHelper
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="host">SMTP服务器地址</param>
        /// <param name="port">SMTP端口</param>
        /// <param name="useSsl">是否使用SSL</param>
        /// <param name="fromUserName">登录用户名</param>
        /// <param name="fromPassword">登录密码</param>
        /// <param name="fromName">发件人显示昵称</param>
        /// <param name="fromAddress">发件人邮箱地址</param>
        /// <param name="toAddress">收件人邮箱地址</param>
        /// <param name="toTitle">邮件标题</param>
        /// <param name="toBody">邮件内容</param>
        public static void Send(string? host, 
            int port,
            bool useSsl,
            string? fromUserName,
            string? fromPassword,
            string? fromName,
            string? fromAddress,
            string? toAddress,
            string? toTitle,
            string? toBody)
        {
            if (host == null || fromUserName == null || fromAddress == null)
            {
                return;
            }
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromAddress));
            message.To.Add(new MailboxAddress(null, toAddress));
            message.Subject = toTitle;
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = toBody
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            client.Connect(host, port, useSsl);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate(fromUserName, fromPassword);
            client.Send(message);
            client.Disconnect(true);
        }

    }
}
