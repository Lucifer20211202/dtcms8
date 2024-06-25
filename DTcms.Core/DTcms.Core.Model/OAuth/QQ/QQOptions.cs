using Newtonsoft.Json;

namespace DTcms.Core.Model.OAuth.QQ
{
    /// <summary>
    /// 获取AccessToken返回参数
    /// </summary>
    public class TokenResultDto
    {
        /// <summary>
        /// 授权令牌，Access_Token
        /// </summary>
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        /// <summary>
        /// access token的有效期，单位为秒
        /// </summary>
        [JsonProperty("expires_in")]
        public string? ExpiresIn { get; set; }

        /// <summary>
        /// 用于刷新的RefreshToken
        /// </summary>
        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }
    }

    /// <summary>
    /// 通过AccessToken用户OPENID返回参数
    /// </summary>
    public class OpenIdResultDto
    {
        /// <summary>
        /// 平台提供的APPID
        /// </summary>
        [JsonProperty("client_id")]
        public string? ClientId { get; set; }

        /// <summary>
        /// 授权用户唯一标识
        /// </summary>
        [JsonProperty("openid")]
        public string? OpenId { get; set; }
    }

    /// <summary>
    /// 获取用户信息返回参数
    /// </summary>
    public class UserInfoResultDto
    {
        /// <summary>
        /// 返回码0正确，其它失败
        /// </summary>
        [JsonProperty("ret")]
        public string? Ret { get; set; }

        /// <summary>
        /// 如果ret<0，会有相应的错误信息提示，返回数据全部用UTF-8编码
        /// </summary>
        [JsonProperty("msg")]
        public string? Msg { get; set; }

        /// <summary>
        /// 用户在QQ空间的昵称
        /// </summary>
        [JsonProperty("nickname")]
        public string? NickName { get; set; }

        /// <summary>
        /// 大小为30×30像素的QQ空间头像URL
        /// </summary>
        [JsonProperty("figureurl")]
        public string? FigureUrl { get; set; }

        /// <summary>
        /// 大小为50×50像素的QQ空间头像URL
        /// </summary>
        [JsonProperty("figureurl_1")]
        public string? FigureUrl1 { get; set; }

        /// <summary>
        /// 大小为100×100像素的QQ空间头像URL
        /// </summary>
        [JsonProperty("figureurl_2")]
        public string? FigureUrl2 { get; set; }

        /// <summary>
        /// 大小为40×40像素的QQ头像URL
        /// </summary>
        [JsonProperty("figureurl_qq_1")]
        public string? FigureUrlQQ1 { get; set; }

        /// <summary>
        /// 大小为100×100像素的QQ头像URL。
        /// 需要注意，不是所有的用户都拥有QQ的100x100的头像，但40x40像素则是一定会有
        /// </summary>
        [JsonProperty("figureurl_qq_2")]
        public string? FigureUrlQQ2 { get; set; }

        /// <summary>
        /// 性别。 如果获取不到则默认返回"男"
        /// </summary>
        [JsonProperty("gender")]
        public string? Gender { get; set; }
    }

    /// <summary>
    /// 通过JsCode返回参数
    /// </summary>
    public class JsCodeResultDto
    {
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        [JsonProperty("openid")]
        public string? OpenId { get; set; }

        /// <summary>
        /// 会话密钥
        /// </summary>
        [JsonProperty("session_key")]
        public string? SessionKey { get; set; }

        /// <summary>
        /// 用户在开放平台的唯一标识符，在满足 UnionID 下发条件的情况下会返回
        /// </summary>
        [JsonProperty("unionid")]
        public string? UnionId { get; set; }

        /// <summary>
        /// 错误码0正确，其它错误
        /// </summary>
        [JsonProperty("errcode")]
        public string? ErrCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [JsonProperty("errmsg")]
        public string? ErrMsg { get; set; }
    }
}
