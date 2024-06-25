using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.OAuth.WeChat;
using DTcms.Core.Model.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 微信授权接口实现
    /// </summary>
    public class WeChatAuthService : IWeChatAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISiteOAuthService _oAuthService;
        private readonly ISiteOAuthLoginService _oAuthLoginService;
        private readonly IMemberService _menberService;
        public WeChatAuthService(IHttpContextAccessor httpContextAccessor, ISiteOAuthService oAuthService, 
            ISiteOAuthLoginService oAuthLoginService, IMemberService menberService)
        {
            _httpContextAccessor = httpContextAccessor;
            _oAuthService = oAuthService;
            _oAuthLoginService = oAuthLoginService;
            _menberService = menberService;
        }

        /// <summary>
        /// OAuth2生成授权跳转链接
        /// </summary>
        public async Task<string> OAuth2UrlAsync(int siteId, string? type, string? redirectUri)
        {
            //取得开放平台信息
            var model = await _oAuthService.QueryAsync<SiteOAuths>(x => x.SiteId == siteId && x.Provider == WeChatConfig.Provider && x.Type == type && x.Status == 0);
            if (model == null)
            {
                throw new ResponseException("授权平台设置有误，请联系客服");
            }
            //检查是公众号还是PC
            if (type == "h5")
            {
                return $"{WeChatConfig.OAuthH5Endpoint}?appid={model.ClientId}&redirect_uri={redirectUri}&response_type=code&scope=snsapi_userinfo&state={siteId}#wechat_redirect";
            }
            return $"{WeChatConfig.OAuthEndpoint}?appid={model.ClientId}&redirect_uri={redirectUri}&response_type=code&scope=snsapi_login&state={siteId}#wechat_redirect";

        }

        /// <summary>
        /// OAuth2授权账户
        /// </summary>
        public async Task<int> OAuth2SignInAsync(int siteId, string? type, string? code)
        {
            //取得开放平台信息
            var model = await _oAuthService.QueryAsync<SiteOAuths>(x => x.SiteId == siteId && x.Provider == WeChatConfig.Provider && x.Type == type && x.Status == 0);
            if (model == null)
            {
                throw new ResponseException("授权平台设置有误，请联系客服");
            }
            //第一步：通过Authorization Code获取Access Token
            var tokenUrl = $"{WeChatConfig.TokenEndpoint}?appid={model.ClientId}&secret={model.ClientSecret}&code={code}&grant_type=authorization_code";
            var tokenResult = await RequestHelper.GetAsync(tokenUrl);
            var tokenDto = JsonHelper.ToJson<TokenResultDto>(tokenResult);
            if (tokenDto?.AccessToken == null)
            {
                var dic = JsonHelper.ToJson<Dictionary<string, string>>(tokenResult);
                throw new ResponseException($"获取AccessToken失败，错误码：{dic?["errcode"]}，请联系客服");
            }
            //如果用户已授权，则直接返回
            var loginModel = await _oAuthLoginService.QueryAsync<SiteOAuthLogins>(x => x.Provider == WeChatConfig.Provider && x.OpenId == tokenDto.OpenId);
            if (loginModel != null)
            {
                return loginModel.UserId;
            }
            //第二步：获取用户信息
            var userUrl = $"{WeChatConfig.UserInfoEndpoint}?access_token={tokenDto.AccessToken}&openid={tokenDto.OpenId}&lang=zh_CN";
            var userResult = await RequestHelper.GetAsync(userUrl);
            var userDto = JsonHelper.ToJson<UserInfoResultDto>(userResult);
            if (userDto?.OpenId == null)
            {
                var dic = JsonHelper.ToJson<Dictionary<string, string>>(userResult);
                throw new ResponseException($"获取用户信息失败，错误码：{dic?["errcode"]}，错误消息：{dic?["errmsg"]}，请联系客服");
            }
            //第三步：新增会员
            var userName = UtilConvert.GetGuidToString();
            var memberModel = await _menberService.AddAsync(new MembersEditDto
            {
                SiteId = siteId,
                UserName = userName,
                Password = userDto.OpenId.ToLower(),
                RealName = userDto.NickName,
                Avatar = userDto.HeadImgUrl
            }) ?? throw new ResponseException("开通账户失败，请联系客服");
            //第五步：关联用户授权
            await _oAuthLoginService.AddAsync<SiteOAuthLogins>(new SiteOAuthLogins
            {
                OAuthId = model.Id,
                Provider = model.Provider,
                UserId = memberModel.UserId,
                OpenId = userDto.OpenId
            });
            return memberModel.UserId;
        }
    }
}
