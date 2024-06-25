using DTcms.Core.Model.WeChat;

namespace DTcms.Core.IServices.WeChat
{
    /// <summary>
    /// 支付接口
    /// </summary>
    public interface IWeChatExecuteService
    {
        /// <summary>
        /// 扫码下单支付
        /// </summary>
        Task<WeChatPayNativeParamDto> NativePayAsync(WeChatPayDto modelDto);

        /// <summary>
        /// 拼接OAuth2的URI
        /// </summary>
        Task<string> OAuthAsync(WeChatPayOAuthDto modelDto);

        /// <summary>
        /// 申请退款
        /// </summary>
        Task<bool> RefundAsync(WeChatPayRefundDto modelDto);
    }
}
