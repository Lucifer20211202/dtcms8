using DTcms.Core.IServices;
using DTcms.Core.IServices.WeChat;
using DTcms.Core.Model.WeChat;
using Microsoft.AspNetCore.Mvc;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 微信支付回调
    /// </summary>
    [Route("wechatpay/notify")]
    [ApiController]
    public class WeChatPayNotifyController(IWeChatNotifyService weChatNotifyService, IOrderPaymentService orderPaymentService) : ControllerBase
    {
        private readonly IWeChatNotifyService _weChatNotifyService = weChatNotifyService;
        private readonly IOrderPaymentService _orderPaymentService = orderPaymentService;

        /// <summary>
        /// 支付成功回调
        /// 示例：/wechatpay/notify/transactions/1
        /// </summary>
        [HttpPost("transactions/{paymentId}")]
        public async Task<IActionResult> Transactions([FromRoute] int paymentId)
        {
            bool result = false; //处理状态
            var notify = await _weChatNotifyService.ConfirmAsync(Request, paymentId);
            if (notify != null && notify.TradeState == "SUCCESS" && notify.OutTradeNo != null)
            {
                //修改订单状态
                result = await _orderPaymentService.ConfirmAsync(notify.OutTradeNo);
            }
            //返回结果
            if (result)
            {
                return Ok(new WeChatPayNotifyResultDto());
            }
            return BadRequest(new WeChatPayNotifyResultDto()
            {
                Code = "FAIL",
                Message = "FAIL"
            });
        }
    }
}