using System;
using System.Collections.Generic;
using System.Text;

namespace DTcms.Core.Model.OAuth.QQ
{
    /// <summary>
    /// QQ登录常量定义
    /// </summary>
    public static class QQConfig
    {
        /// <summary>
        /// 授权标识
        /// </summary>
        public const string Provider = "qq";

        /// <summary>
        /// 获取Code
        /// https://graph.qq.com/oauth2.0/authorize?response_type=code&client_id=APPID&redirect_uri=REDIRECT_URI&state=STATE
        /// </summary>
        public const string OAuthEndpoint = "https://graph.qq.com/oauth2.0/authorize";

        /// <summary>
        /// 获取AccessToken
        /// https://graph.qq.com/oauth2.0/token?grant_type=authorization_code&client_id=APPID&client_secret=SECRET&code=CODE&redirect_uri=REDIRECT_URI
        /// </summary>
        public const string TokenEndpoint = "https://graph.qq.com/oauth2.0/token";

        /// <summary>
        /// 通过AccessToken用户OPENID
        /// https://graph.qq.com/oauth2.0/me?access_token=ACCESS_TOKEN
        /// </summary>
        public const string UserIdentityEndpoint = "https://graph.qq.com/oauth2.0/me";

        /// <summary>
        /// 获取用户信息
        /// https://graph.qq.com/user/get_user_info?access_token=ACCESS_TOKEN&oauth_consumer_key=APPID&openid=OPENID
        /// </summary>
        public const string UserInfoEndpoint = "https://graph.qq.com/user/get_user_info";

        /// <summary>
        /// 通过JSCODE获取用户OPENID
        /// https://api.q.qq.com/sns/jscode2session?appid=APPID&secret=SECRET&js_code=JSCODE&grant_type=authorization_code
        /// </summary>
        public const string JsCodeEndpoint = "https://api.q.qq.com/sns/jscode2session";
    }
}
