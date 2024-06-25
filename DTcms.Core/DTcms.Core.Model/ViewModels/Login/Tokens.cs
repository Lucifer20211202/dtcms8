using System;
using System.Collections.Generic;
using System.Text;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 通讯密钥Token
    /// </summary>
    public class Tokens(string accessToken, string refreshToken)
    {
        /// <summary>
        /// 正式使用的Token
        /// </summary>
        public string? AccessToken { get; set; } = accessToken;

        /// <summary>
        /// 用于刷新的Token
        /// </summary>
        public string? RefreshToken { get; set; } = refreshToken;
    }
}
