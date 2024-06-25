using DTcms.Core.Model.Alipay;

namespace DTcms.Core.IServices.Alipay
{
    /// <summary>
    /// 支付宝支付接口
    /// </summary>
    public interface IAlipayExecuteService
    {
        /// <summary>
        /// 电脑网站下单支付
        /// </summary>
        Task<AlipayPageParamDto> PcPayAsync(AlipayTradeDto modelDto);

        /// <summary>
        /// 处理退款请求
        /// </summary>
        Task<bool> RefundAsync(AlipayRefundDto modelDto);
    }
}
