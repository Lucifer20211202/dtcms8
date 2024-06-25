using System;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 微信授权接口
    /// </summary>
    public interface IWeChatAuthService
    {
        /// <summary>
        /// OAuth2生成授权跳转链接
        /// </summary>
        Task<string> OAuth2UrlAsync(int siteId, string? type, string? redirectUri);

        /// <summary>
        /// OAuth2授权账户
        /// </summary>
        Task<int> OAuth2SignInAsync(int siteId, string? type, string? code);
    }
}
