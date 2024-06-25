using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.IServices.Alipay;
using DTcms.Core.Model.Alipay;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace DTcms.Core.Services.Alipay
{
    /// <summary>
    /// 支付宝支付通知实现
    /// </summary>
    public class AlipayNotifyService(ISitePaymentService sitePaymentService) : AlipayBase(sitePaymentService), IAlipayNotifyService
    {
        /// <summary>
        /// 电脑支付回调通知
        /// </summary>
        public async Task<AlipayPageNotifyDto> PagePay(HttpRequest request)
        {
            //获取响应数据
            var param = await GetParametersAsync(request);
            if (param.Count == 0 || !param.ContainsKey("trade_status"))
            {
                throw new ResponseException("无法获取支付宝异步通知参数");
            }
            //转换成实体
            var jsonStr = param.ToJson();
            var notify = jsonStr.ToObject<AlipayPageNotifyDto>();
            int paymentId = 0;
            if (!int.TryParse(notify?.PassbackParams, out paymentId))
            {
                throw new ResponseException("支付回调未能获取站点信息");
            }
            //取得支付账户
            var account = await GetAccountAsync(paymentId);
            //验证签名
            CheckNotifySign(param, account.AlipayPublicKey);
            return notify;
        }

        #region 私有辅助方法
        /// <summary>
        /// 获取通知参数集合
        /// </summary>
        public async Task<IDictionary<string, string>> GetParametersAsync(HttpRequest request)
        {
            var parameters = new Dictionary<string, string>();
            if (request.Method == "POST")
            {
                var form = await request.ReadFormAsync();
                foreach (var item in form)
                {
                    parameters.Add(item.Key, item.Value);
                }
            }
            else
            {
                foreach (var item in request.Query)
                {
                    parameters.Add(item.Key, item.Value);
                }
            }
            return parameters;
        }

        /// <summary>
        /// 验证签名是否正确
        /// </summary>
        private void CheckNotifySign(IDictionary<string, string>? dic, string? alipayPublicKey)
        {
            if (dic == null || dic.Count == 0)
            {
                throw new ResponseException("sign check fail: dictionary)} is Empty!");
            }

            if (!dic.TryGetValue(AlipayConfig.SIGN, out var sign))
            {
                throw new ResponseException("sign check fail: sign)} is Empty!");
            }

            dic.Remove(AlipayConfig.SIGN);
            dic.Remove(AlipayConfig.SIGN_TYPE);
            var content = GetSignContent(dic);
            if (!SHA256WithRSA.Verify(content, sign, alipayPublicKey))
            {
                throw new ResponseException("sign check fail: check Sign and Data Fail!");
            }
        }

        /// <summary>
        /// 获取签名内容
        /// </summary>
        private string GetSignContent(IDictionary<string, string> dic)
        {
            if (dic == null || dic.Count == 0)
            {
                return string.Empty;
            }
            var sortPara = new SortedDictionary<string, string>(dic);
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

        #endregion
    }
}
