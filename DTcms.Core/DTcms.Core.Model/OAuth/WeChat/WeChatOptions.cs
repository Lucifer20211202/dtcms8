using Newtonsoft.Json;

namespace DTcms.Core.Model.OAuth.WeChat
{
    /// <summary>
    /// 获取AccessToken返回参数
    /// </summary>
    public class TokenResultDto
    {
        /// <summary>
        /// 接口调用凭证
        /// </summary>
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        /// <summary>
        /// access_token接口调用凭证超时时间，单位（秒）
        /// </summary>
        [JsonProperty("expires_in")]
        public string? ExpiresIn { get; set; }

        /// <summary>
        /// 用于刷新的RefreshToken
        /// </summary>
        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// 授权用户唯一标识
        /// </summary>
        [JsonProperty("openid")]
        public string? OpenId { get; set; }

        /// <summary>
        /// 用户授权的作用域，使用逗号（,）分隔
        /// </summary>
        [JsonProperty("scope")]
        public string? Scope { get; set; }

        /// <summary>
        /// 当且仅当该网站应用已获得该用户的userinfo授权时，才会出现该字段
        /// </summary>
        [JsonProperty("unionid")]
        public string? UnionId { get; set; }
    }

    /// <summary>
    /// 获取用户信息返回参数
    /// </summary>
    public class UserInfoResultDto
    {
        /// <summary>
        /// 授权用户唯一标识
        /// </summary>
        [JsonProperty("openid")]
        public string? OpenId { get; set; }

        /// <summary>
        /// 用户在QQ空间的昵称
        /// </summary>
        [JsonProperty("nickname")]
        public string? NickName { get; set; }

        /// <summary>
        /// 普通用户性别，1为男性，2为女性
        /// </summary>
        [JsonProperty("sex")]
        public string? Sex { get; set; }

        /// <summary>
        /// 普通用户个人资料填写的省份
        /// </summary>
        [JsonProperty("province")]
        public string? Province { get; set; }

        /// <summary>
        /// 普通用户个人资料填写的城市
        /// </summary>
        [JsonProperty("city")]
        public string? City { get; set; }

        /// <summary>
        /// 国家，如中国为CN
        /// </summary>
        [JsonProperty("country")]
        public string? Country { get; set; }

        /// <summary>
        /// 用户头像
        /// 最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像）
        /// 用户没有头像时该项为空
        /// </summary>
        [JsonProperty("headimgurl")]
        public string? HeadImgUrl { get; set; }

        /// <summary>
        /// 用户特权信息，json数组，如微信沃卡用户为（chinaunicom）
        /// </summary>
        [JsonProperty("Privilege")]
        public string[]? Privilege { get; set; }

        /// <summary>
        /// 用户统一标识。针对一个微信开放平台帐号下的应用，同一用户的unionid是唯一的
        /// </summary>
        [JsonProperty("unionid")]
        public string? UnionId { get; set; }
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

    /// <summary>
    /// 用户手机密文解密参数
    /// </summary>
    public class MobileResultDto
    {
        /// <summary>
        /// 用户绑定的手机号（国外手机号会有区号）
        /// </summary>
        [JsonProperty("phoneNumber")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// 没有区号的手机号
        /// </summary>
        [JsonProperty("purePhoneNumber")]
        public string? PurePhoneNumber { get; set; }

        /// <summary>
        /// 区号
        /// </summary>
        [JsonProperty("countryCode")]
        public string? CountryCode { get; set; }
    }
}
