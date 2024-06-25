using DTcms.Core.Model.Alipay;
using Microsoft.AspNetCore.Http;

namespace DTcms.Core.IServices.Alipay
{
    public interface IAlipayNotifyService
    {
        /// <summary>
        /// 电脑支付回调通知
        /// </summary>
        Task<AlipayPageNotifyDto> PagePay(HttpRequest request);
    }
}
