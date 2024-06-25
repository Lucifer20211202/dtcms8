﻿using System.Security.Cryptography;
using System.Text;

namespace DTcms.Core.Common.Weixin
{
    /// <summary>
    /// 签名验证类
    /// </summary>
    public class CheckSignature
    {
        /// <summary>
        /// 在网站没有提供Token（或传入为null）的情况下的默认Token，建议在网站中进行配置。
        /// </summary>
        public const string Token = "weixin";

        /// <summary>
        /// 检查签名是否正确
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="token"></param>
        public static bool Check(string? signature, string? timestamp, string? nonce, string? token = null)
        {
            return signature == GetSignature(timestamp, nonce, token);
        }

        /// <summary>
        /// 返回正确的签名
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="token"></param>
        public static string GetSignature(string? timestamp, string? nonce, string? token = null)
        {
            token ??= Token;
            var arr = new[] { token, timestamp, nonce }.OrderBy(z => z).ToArray();
            var arrString = string.Join("", arr);
            var sha1Arr = SHA1.HashData(Encoding.UTF8.GetBytes(arrString));
            StringBuilder enText = new();
            foreach (var b in sha1Arr)
            {
                enText.AppendFormat("{0:x2}", b);
            }

            return enText.ToString();
        }
    }
}
