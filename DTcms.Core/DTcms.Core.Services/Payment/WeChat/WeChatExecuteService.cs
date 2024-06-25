using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.IServices.WeChat;
using DTcms.Core.Model.WeChat;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;

namespace DTcms.Core.Services.WeChat
{
    /// <summary>
    /// 微信支付接口实现
    /// </summary>
    public class WeChatExecuteService(IHttpContextAccessor httpContextAccessor, ISitePaymentService sitePaymentService)
        : WeChatBase(sitePaymentService), IWeChatExecuteService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        /// <summary>
        /// 扫码下单支付
        /// </summary>
        public async Task<WeChatPayNativeParamDto> NativePayAsync(WeChatPayDto modelDto)
        {
            //取得微信支付账户
            var wxAccount = await GetAccountAsync(modelDto.PaymentId);

            //赋值下单实体(注意附加数据为站点ID)
            WeChatPayBodyDto model = new WeChatPayBodyDto
            {
                AppId = wxAccount.AppId,
                MchId = wxAccount.MchId,
                Description = modelDto.Description,
                OutTradeNo = modelDto.OutTradeNo,
                NotifyUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{wxAccount.NotifyUrl}/transactions/{modelDto.PaymentId}",
                Amount = new Amount { Total = (int)(modelDto.Total * 100), Currency = "CNY" }
            };
            var url = WeChatPayConfig.NativePayUrl;
            var content = model.ToJson();
            var auth = BuildAuth(url, "POST", content, wxAccount.MchId, wxAccount.CertPath, wxAccount.MchId);
            //发送POST请求
            ICollection<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("Authorization", $"WECHATPAY2-SHA256-RSA2048 {auth}"));
            headers.Add(new KeyValuePair<string, string>("User-Agent", "Unknown"));
            headers.Add(new KeyValuePair<string, string>("Accept", "application/json"));
            var (statusCode, reHeaders, reBody) = await RequestHelper.PostAsync(url, headers, content);
            if (statusCode != 200)
            {
                throw new ResponseException($"微信下单失败，错误代码：{statusCode},{reBody}");
            }
            var codeUrl = JsonHelper.ToJson<WeChatPayNativeResultDto>(reBody)?.CodeUrl;
            if (codeUrl == null)
            {
                throw new ResponseException($"无法获取二维码地址");
            }
            //生成Base64图片
            var codeData = QRCodeHelper.GenerateToString(codeUrl);
            return new WeChatPayNativeParamDto { CodeData = codeData };
        }

        /// <summary>
        /// 拼接OAuth2的URI
        /// </summary>
        public async Task<string> OAuthAsync(WeChatPayOAuthDto modelDto)
        {
            //取得微信支付账户
            var wxAccount = await GetAccountAsync(modelDto.PaymentId);
            //拼接地址
            var url = WeChatPayConfig.OAuth2Url 
                + $"?appid={wxAccount.AppId}&redirect_uri={modelDto.RedirectUri}&response_type=code&scope=snsapi_base&state={modelDto.OutTradeNo}#wechat_redirect";
            return url;
        }

        /// <summary>
        /// 申请退款
        /// </summary>
        public async Task<bool> RefundAsync(WeChatPayRefundDto modelDto)
        {
            //取得微信支付账户
            var wxAccount = await GetAccountAsync(modelDto.PaymentId);
            //赋值退款参数实体
            WeChatPayRefundBodyDto model = new()
            {
                OutTradeNo = modelDto.OutTradeNo,
                OutRefundNo = modelDto.OutRefundId.ToString(),
                Reason = modelDto.Reason,
                Amount = new RefundBodyAmount
                {
                    Refund = (int)(modelDto.Refund * 100),
                    Total = (int)(modelDto.Total * 100),
                    Currency = "CNY"
                }
            };
            var url = WeChatPayConfig.RefundsUrl;
            var content = model.ToJson();
            var auth = BuildAuth(url, "POST", content, wxAccount.MchId, wxAccount.CertPath, wxAccount.MchId);
            //发送POST请求
            ICollection<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("Authorization", $"WECHATPAY2-SHA256-RSA2048 {auth}"));
            headers.Add(new KeyValuePair<string, string>("User-Agent", "Unknown"));
            headers.Add(new KeyValuePair<string, string>("Accept", "application/json"));
            var (statusCode, reHeaders, reBody) = await RequestHelper.PostAsync(url, headers, content);
            if (statusCode != 200)
            {
                throw new ResponseException($"微信申请退款失败，错误代码：{statusCode},{reBody}");
            }
            var result = JsonHelper.ToJson<WeChatPayRefundResultDto>(reBody);
            if (result != null && result.Status != null && (result.Status.Equals("SUCCESS") || result.Status.Equals("PROCESSING")))
            {
                return true;
            }

            return false;
        }

        #region 私有辅助方法
        /// <summary>
        /// 获取JSCODE微信用户OPENID
        /// </summary>
        private async Task<string> JsOAuthOpenId(string? appId, string? appSecret, string? jsCode)
        {
            string url = $"{WeChatPayConfig.JsOAuthUrl}?appid={appId}&secret={appSecret}&js_code={jsCode}";
            var result = await RequestHelper.GetAsync(url);
            var dic = JsonHelper.ToJson<Dictionary<string, string>>(result);
            if (dic != null && dic.ContainsKey("openid"))
            {
                return dic["openid"];
            }
            throw new ResponseException($"无法获取OpenID，应填写小程序的AppID和AppSecret");
        }

        /// <summary>
        /// 获取OAuth2微信用户OPENID
        /// </summary>
        private async Task<string> OAuth2OpenId(string? appId, string? appSecret, string? code)
        {
            string url = $"{WeChatPayConfig.AccessTokenUrl}?appid={appId}&secret={appSecret}&code={code}&grant_type=authorization_code";
            var result = await RequestHelper.GetAsync(url);
            var dic = JsonHelper.ToJson<Dictionary<string, string>>(result);
            if (dic != null && dic.ContainsKey("openid"))
            {
                return dic["openid"];
            }
            throw new ResponseException($"获取OpenId出错：{code}，{result}");
        }

        /// <summary>
        /// 生成头部Authorization
        /// </summary>
        private string BuildAuth(string url, string method, string? body, string? mchId, string? certPath, string? certPwd)
        {
            if (certPath == null)
            {
                throw new ResponseException($"证书路径有误，请检查后重试");
            }
            var uri = new Uri(url).PathAndQuery;
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var nonce = Guid.NewGuid().ToString("N");
            var message = $"{method}\n{uri}\n{timestamp}\n{nonce}\n{body}\n";
            var certificate2 = new X509Certificate2(certPath, certPwd, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            var signature = SHA256WithRSA.Sign(certificate2.GetRSAPrivateKey(), message);
            return $"mchid=\"{mchId}\",nonce_str=\"{nonce}\",timestamp=\"{timestamp}\",serial_no=\"{certificate2.GetSerialNumberString()}\",signature=\"{signature}\"";
        }
        #endregion
    }
}
