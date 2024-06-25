using System;
using System.Collections.Generic;
using System.Text;

namespace DTcms.Core.Model.OAuth.WeChat
{
    /// <summary>
    /// 微信登录常量定义
    /// </summary>
    public static class WeChatConfig
    {
        /// <summary>
        /// 授权标识
        /// </summary>
        public const string Provider = "wechat";

        /// <summary>
        /// 获取Code
        /// 开放平台：https://open.weixin.qq.com/connect/qrconnect?appid=APPID&redirect_uri=REDIRECT_URI&response_type=code&scope=snsapi_login&state=STATE#wechat_redirect
        /// 公 众 号：https://open.weixin.qq.com/connect/oauth2/authorize?appid=APPID&redirect_uri=REDIRECT_URI&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect
        /// </summary>
        public const string OAuthEndpoint = "https://open.weixin.qq.com/connect/qrconnect";
        public const string OAuthH5Endpoint = "https://open.weixin.qq.com/connect/oauth2/authorize";

        /// <summary>
        /// 获取AccessToken
        /// https://api.weixin.qq.com/sns/oauth2/access_token?appid=APPID&secret=SECRET&code=CODE&grant_type=authorization_code
        /// </summary>
        public const string TokenEndpoint = "https://api.weixin.qq.com/sns/oauth2/access_token";

        /// <summary>
        /// 获取用户信息
        /// https://api.weixin.qq.com/sns/userinfo?access_token=ACCESS_TOKEN&openid=OPENID
        /// </summary>
        public const string UserInfoEndpoint = "https://api.weixin.qq.com/sns/userinfo";

        /// <summary>
        /// 通过JSCODE获取用户OPENID
        /// https://api.weixin.qq.com/sns/jscode2session?appid=APPID&secret=SECRET&js_code=JSCODE&grant_type=authorization_code
        /// </summary>
        public const string JsCodeEndpoint = "https://api.weixin.qq.com/sns/jscode2session";
    }
}
