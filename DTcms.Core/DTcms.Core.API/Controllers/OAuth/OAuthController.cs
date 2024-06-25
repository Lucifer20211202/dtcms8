using DTcms.Core.Common.Helpers;
using DTcms.Core.Common.Extensions;
using DTcms.Core.IServices;
using DTcms.Core.Model.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 第三方授权接口
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class OAuthController(ITokenService tokenService, IQQAuthService qqAuthExecute, IWeChatAuthService wechatAuthExecute) : ControllerBase
    {
        private readonly ITokenService _tokenService = tokenService;
        private readonly IQQAuthService _qqAuthExecute = qqAuthExecute;
        private readonly IWeChatAuthService _wechatAuthExecute = wechatAuthExecute;

        /// <summary>
        ///  OAuth2生成授权跳转链接
        /// </summary>
        [HttpGet("redirect")]
        public async Task<IActionResult> Redirect([FromQuery] int siteId, [FromQuery] string provider, [FromQuery] string? type, [FromQuery] string redirectUri)
        {
            //检查参数
            redirectUri = WebUtility.UrlEncode(redirectUri);
            if (!type.IsNotNullOrEmpty()) type = "web";
            //检查授权平台
            if (provider == Model.OAuth.QQ.QQConfig.Provider)
            {
                var result = await _qqAuthExecute.OAuth2UrlAsync(siteId, type, redirectUri);
                return Ok(result);
            }
            else if (provider == Model.OAuth.WeChat.WeChatConfig.Provider)
            {
                var result = await _wechatAuthExecute.OAuth2UrlAsync(siteId, type, redirectUri);
                return Ok(result);
            }
            throw new ResponseException("暂无该授权登录方式");
        }

        /// <summary>
        /// 第三方授权登录
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> SignIn([FromBody] LoginOAuthDto loginDto)
        {
            //检查授权平台
            int userId = 0;
            switch (loginDto.Provider)
            {
                case Model.OAuth.QQ.QQConfig.Provider:
                    if (loginDto.Type == "web" || loginDto.Type == "h5")
                    {
                        userId = await _qqAuthExecute.OAuth2SignInAsync(loginDto.SiteId, loginDto.Type, loginDto.Code, loginDto.RedirectUri);
                    }
                    break;
                case Model.OAuth.WeChat.WeChatConfig.Provider:
                    if (loginDto.Type == "web" || loginDto.Type == "h5")
                    {
                        userId = await _wechatAuthExecute.OAuth2SignInAsync(loginDto.SiteId, loginDto.Type, loginDto.Code);
                    }
                    break;
                default:
                    throw new ResponseException("暂无该授权登录方式");
            }

            var resultDto = await _tokenService.OAuthAsync(userId);
            return Ok(resultDto);
        }
    }
}
