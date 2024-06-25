using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.IServices.WeChat;
using DTcms.Core.Model.WeChat;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DTcms.Core.Services.WeChat
{
    /// <summary>
    /// 微信支付通知实现
    /// </summary>
    public class WeChatNotifyService(ISitePaymentService sitePaymentService) : WeChatBase(sitePaymentService), IWeChatNotifyService
    {
        /// <summary>
        /// 确认解密回调信息
        /// </summary>
        public async Task<WeChatPayNotifyDecryptDto?> ConfirmAsync(HttpRequest request, int paymentId)
        {
            //获取HEADERS和BODY内容
            var headers = GetHeaders(request);
            using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
            var body = await reader.ReadToEndAsync();
            var notify = body.ToObject<WeChatPayNotifyDto>();

            //取得微信支付账户
            var wxAccount = await GetAccountAsync(paymentId);

            //验证签名
            await CheckNotifySign(headers, body, wxAccount);
            //解密数据
            var resourcePlaintext = AEADAES256GCM.Decrypt(notify?.Resource?.Nonce, notify?.Resource?.Ciphertext, notify?.Resource?.AssociatedData, wxAccount.Apiv3Key);
            //throw new ResponseException($"解密结果：{resourcePlaintext}");
            //返回成功信息
            return resourcePlaintext.ToObject<WeChatPayNotifyDecryptDto>();
        }

        #region 私有辅助方法
        /// <summary>
        /// 返回通知的Header
        /// </summary>
        private WeChatPayHeaderDto GetHeaders(HttpRequest request)
        {
            var headers = new WeChatPayHeaderDto();

            if (request.Headers.TryGetValue("Wechatpay-Serial", out var serialValues))
            {
                headers.Serial = serialValues.First();
            }

            if (request.Headers.TryGetValue("Wechatpay-Timestamp", out var timestampValues))
            {
                headers.Timestamp = timestampValues.First();
            }

            if (request.Headers.TryGetValue("Wechatpay-Nonce", out var nonceValues))
            {
                headers.Nonce = nonceValues.First();
            }

            if (request.Headers.TryGetValue("Wechatpay-Signature", out var signatureValues))
            {
                headers.Signature = signatureValues.First();
            }
            return headers;
        }

        /// <summary>
        /// 验证签名是否正确
        /// </summary>
        private async Task CheckNotifySign(WeChatPayHeaderDto headers, string body, WeChatPayAccountDto wxAccount)
        {
            if (string.IsNullOrEmpty(headers.Serial))
            {
                throw new ResponseException($"sign check fail: {nameof(headers.Serial)} is empty!");
            }
            if (string.IsNullOrEmpty(headers.Signature))
            {
                throw new ResponseException($"sign check fail: {nameof(headers.Signature)} is empty!");
            }
            if (string.IsNullOrEmpty(body))
            {
                throw new ResponseException("sign check fail: body is empty!");
            }
            var cert = await new WeChatCertificate().GetCertificateAsync(wxAccount, headers.Serial);
            var message = $"{headers.Timestamp}\n{headers.Nonce}\n{body}\n";
            var signCheck = SHA256WithRSA.Verify(cert?.Certificate?.GetRSAPublicKey(), message, headers.Signature);
            if (!signCheck)
            {
                throw new ResponseException("sign check fail: check Sign and Data Fail!" + headers.ToJson() + ",,," + body);
            }
        }
        #endregion
    }
}
