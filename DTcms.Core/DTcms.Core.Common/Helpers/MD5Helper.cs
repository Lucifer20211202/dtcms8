﻿using System.Security.Cryptography;
using System.Text;

namespace DTcms.Core.Common.Helpers
{
    /// <summary>
    /// MD5加密
    /// </summary>
    public class MD5Helper
    {
        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <returns>加密字符串</returns>
        public static string MD5Encrypt16(string? password)
        {
            if (!string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password))
            {
                MD5 md5 = MD5.Create(); //实例化一个md5对像
                string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(password)), 4, 8);
                t2 = t2.Replace("-", string.Empty);
                return t2;
            }
            return string.Empty;
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <returns>加密字符串</returns>
        public static string MD5Encrypt32(string? password = "")
        {
            string pwd = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password))
                {
                    MD5 md5 = MD5.Create(); //实例化一个md5对像
                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
                    byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                    // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
                    foreach (var item in s)
                    {
                        // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                        pwd = string.Concat(pwd, item.ToString("X2"));
                    }
                }
            }
            catch
            {
                throw new Exception($"错误的 password 字符串:【{password}】");
            }
            return pwd;
        }

        /// <summary>
        /// 64位MD5加密
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <returns>加密字符串</returns>
        public static string MD5Encrypt64(string? password)
        {
            if (!string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password))
            {
                // 实例化一个md5对像
                // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
                MD5 md5 = MD5.Create();
                byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(s);
            }
            return string.Empty;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        public static string Compute(string? password)
        {
            if (!string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password))
            {
                using var md5 = MD5.Create();
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hash).Replace("-", "");
            }
            return string.Empty;
        }
    }
}