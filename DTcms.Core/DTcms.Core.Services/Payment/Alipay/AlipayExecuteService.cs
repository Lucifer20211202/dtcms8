using DTcms.Core.Common.Extensions;
using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.IServices.Alipay;
using DTcms.Core.Model.Alipay;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text;

namespace DTcms.Core.Services.Alipay
{
    /// <summary>
    /// 支付宝支付接口实现
    /// </summary>
    public class AlipayExecuteService(IHttpContextAccessor httpContextAccessor, ISitePaymentService sitePaymentService)
        : AlipayBase(sitePaymentService), IAlipayExecuteService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        /// <summary>
        /// 电脑网站下单支付
        /// </summary>
        public async Task<AlipayPageParamDto> PcPayAsync(AlipayTradeDto modelDto)
        {
            //取得支付宝账户
            var alipayAccount = await GetAccountAsync(modelDto.PaymentId);
            //添加请求参数
            var model = new AlipayTradePageDto
            {
                OutTradeNo = modelDto.OutTradeNo,
                ProductCode = "FAST_INSTANT_TRADE_PAY",
                TotalAmount = modelDto.Total,
                Subject = modelDto.Description,
                PassbackParams = modelDto.PaymentId.ToString()
            };
            var bizContent = model.ToJson(); //转换为JSON字符串
            //添加公共请求参数
            var param = AddPublicParam(alipayAccount, "alipay.trade.page.pay", modelDto.ReturnUri, bizContent);
            var url = AlipayConfig.SERVER_URL + "?" + BuildQuery(param);
            return new AlipayPageParamDto()
            {
                Url = url
            };
        }

        /// <summary>
        /// 处理退款请求
        /// </summary>
        public async Task<bool> RefundAsync(AlipayRefundDto modelDto)
        {
            //取得支付宝账户
            var alipayAccount = await GetAccountAsync(modelDto.PaymentId);
            //添加请求参数
            var model = new AlipayTradeRefundDto
            {
                OutTradeNo = modelDto.OutTradeNo,
                RefundAmount = modelDto.Refund,
                RefundReason = modelDto.Reason
            };
            var bizContent = model.ToJson(); //转换为JSON字符串
            //添加公共请求参数
            //生成参数字典
            Dictionary<string, string?> dic = new()
            {
                { AlipayConfig.APP_ID, alipayAccount.AppId },
                { AlipayConfig.METHOD, "alipay.trade.refund" },
                { AlipayConfig.FORMAT, "json" },
                { AlipayConfig.CHARSET, "utf-8" },
                { AlipayConfig.SIGN_TYPE, "RSA2" },
                { AlipayConfig.TIMESTAMP, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                { AlipayConfig.VERSION, "1.0" }
            };
            dic.Add(AlipayConfig.BIZ_CONTENT, bizContent);
            // 添加签名参数
            var signContent = GetSignContent(dic);
            dic.Add(AlipayConfig.SIGN, SHA256WithRSA.Sign(signContent, alipayAccount.AppPrivateKey));
            var url = AlipayConfig.SERVER_URL + "?" + BuildQuery(dic);
            var reBody = await RequestHelper.PostAsync(url, string.Empty, "application/x-www-form-urlencoded");
            var result = JsonHelper.ToJson<AlipayRefundParamDto>(reBody);
            if (result == null)
            {
                throw new ResponseException($"申请退款失败，请求接口失败");
            }
            if (result.Response == null)
            {
                throw new ResponseException($"申请退款失败：{reBody}");
            }

            if (result.Response.FundChange != null && result.Response.FundChange.Equals("Y"))
            {
                return true;
            }
            return false;
        }

        #region 私有辅助方法
        /// <summary>
        /// 添加公共请求参数
        /// </summary>
        private IDictionary<string, string?> AddPublicParam(AlipayAccountDto account, string method, string? returnUri, string bizContent)
        {
            //生成参数字典
            Dictionary<string, string?> dic = new()
            {
                { AlipayConfig.APP_ID, account.AppId },
                { AlipayConfig.METHOD, method },
                { AlipayConfig.FORMAT, "json" },
                { AlipayConfig.CHARSET, "utf-8" },
                { AlipayConfig.SIGN_TYPE, "RSA2" },
                { AlipayConfig.TIMESTAMP, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                { AlipayConfig.VERSION, "1.0" }
            };
            if (returnUri.IsNotNullOrEmpty())
            {
                dic.Add(AlipayConfig.RETURN_URL, returnUri);
            }
            dic.Add(AlipayConfig.NOTIFY_URL, $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{account.NotifyUrl}/transactions/{account.NotifyType}");
            dic.Add(AlipayConfig.BIZ_CONTENT, bizContent);
            // 添加签名参数
            var signContent = GetSignContent(dic);
            dic.Add(AlipayConfig.SIGN, SHA256WithRSA.Sign(signContent, account.AppPrivateKey));
            return dic;
        }

        /// <summary>
        /// 组装需要签名的字符串
        /// </summary>
        private string GetSignContent(IDictionary<string, string?> dic)
        {
            if (dic == null || dic.Count == 0)
            {
                return string.Empty;
            }
            //重新排序字典
            var sortPara = new SortedDictionary<string, string?>(dic);
            var sb = new StringBuilder();
            foreach (var iter in sortPara)
            {
                if (!string.IsNullOrEmpty(iter.Value))
                {
                    sb.Append(iter.Key).Append('=').Append(iter.Value).Append('&');
                }
            }
            return sb.ToString()[0..^1];
        }

        /// <summary>
        /// 组装普通文本请求参数
        /// </summary>
        /// <param name="dictionary">请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        public static string BuildQuery(IDictionary<string, string?> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            var sb = new StringBuilder();
            foreach (var iter in dictionary)
            {
                if (!string.IsNullOrEmpty(iter.Value))
                {
                    sb.Append(iter.Key + "=" + WebUtility.UrlEncode(iter.Value) + "&");
                }
            }

            return sb.ToString()[0..^1];
        }
        #endregion
    }
}
