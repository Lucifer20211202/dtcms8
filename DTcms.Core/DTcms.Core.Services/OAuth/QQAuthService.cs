using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.OAuth.QQ;
using DTcms.Core.Model.ViewModels;
using Microsoft.AspNetCore.Http;

namespace DTcms.Core.Services
{
    /// <summary>
    /// QQ授权接口实现
    /// </summary>
    public class QQAuthService(IHttpContextAccessor httpContextAccessor, ISiteOAuthService oAuthService,
        ISiteOAuthLoginService oAuthLoginService, IMemberService menberService) : IQQAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ISiteOAuthService _oAuthService = oAuthService;
        private readonly ISiteOAuthLoginService _oAuthLoginService = oAuthLoginService;
        private readonly IMemberService _menberService = menberService;

        /// <summary>
        /// OAuth2生成授权跳转链接
        /// </summary>
        public async Task<string> OAuth2UrlAsync(int siteId, string? type, string? redirectUri)
        {
            //取得开放平台信息
            var model = await _oAuthService.QueryAsync<SiteOAuths>(x => x.SiteId == siteId && x.Provider == QQConfig.Provider && x.Type == type && x.Status == 0);
            if (model == null)
            {
                throw new ResponseException("授权平台设置有误，请联系客服");
            }
            return $"{QQConfig.OAuthEndpoint}?response_type=code&client_id={model.ClientId}&redirect_uri={redirectUri}&state={siteId}";

        }

        /// <summary>
        /// OAuth2授权账户
        /// </summary>
        public async Task<int> OAuth2SignInAsync(int siteId, string? type, string? code, string? redirectUri)
        {
            //取得开放平台信息
            var model = await _oAuthService.QueryAsync<SiteOAuths>(x => x.SiteId == siteId && x.Provider == QQConfig.Provider && x.Type == type && x.Status == 0);
            if (model == null)
            {
                throw new ResponseException("授权平台设置有误，请联系客服");
            }
            //第一步：通过Authorization Code获取Access Token
            var tokenUrl = $"{QQConfig.TokenEndpoint}?grant_type=authorization_code&client_id={model.ClientId}&client_secret={model.ClientSecret}&code={code}&redirect_uri={redirectUri}&fmt=json";
            var tokenResult = await RequestHelper.GetAsync(tokenUrl);
            var tokenDto = JsonHelper.ToJson<TokenResultDto>(tokenResult);
            if (tokenDto?.AccessToken == null)
            {
                var dic = JsonHelper.ToJson<Dictionary<string, string>>(tokenResult);
                throw new ResponseException($"获取AccessToken失败，错误码：{dic?["error"]}，请联系客服");
            }
            //第二步：获取用户OpenID
            var openUrl = $"{QQConfig.UserIdentityEndpoint}?access_token={tokenDto.AccessToken}&fmt=json";
            var openResult = await RequestHelper.GetAsync(openUrl);
            var openDto = JsonHelper.ToJson<OpenIdResultDto>(openResult);
            if (openDto?.OpenId == null)
            {
                var dic = JsonHelper.ToJson<Dictionary<string, string>>(openResult);
                throw new ResponseException($"获取用户OpenId失败，错误码：{dic?["error"]}，请联系客服");
            }
            //如果用户已授权，则直接返回
            var loginModel = await _oAuthLoginService.QueryAsync<SiteOAuthLogins>(x => x.Provider == QQConfig.Provider && x.OpenId == openDto.OpenId);
            if (loginModel != null)
            {
                return loginModel.UserId;
            }
            //第三步：获取用户信息
            var userUrl = $"{QQConfig.UserInfoEndpoint}?access_token={tokenDto.AccessToken}&oauth_consumer_key={model.ClientId}&openid={openDto.OpenId}";
            var userResult = await RequestHelper.GetAsync(userUrl);
            var userDto = JsonHelper.ToJson<UserInfoResultDto>(userResult);
            if (userDto?.Ret != "0")
            {
                throw new ResponseException($"获取用户信息失败，错误码：{userDto?.Ret}，错误消息：{userDto?.Msg}，请联系客服");
            }
            //第四步：新增会员
            var memberModel = await _menberService.AddAsync(new MembersEditDto
            {
                SiteId = siteId,
                UserName = UtilConvert.GetGuidToString(),
                Password = openDto.OpenId.ToLower(),
                RealName = userDto.NickName,
                Avatar = userDto.FigureUrl2
            }) ?? throw new ResponseException("开通账户失败，请联系客服");
            //第五步：关联用户授权
            await _oAuthLoginService.AddAsync<SiteOAuthLogins>(new SiteOAuthLogins
            {
                OAuthId = model.Id,
                Provider = model.Provider,
                UserId = memberModel.UserId,
                OpenId = openDto.OpenId
            });
            return memberModel.UserId;
        }
    }
}
