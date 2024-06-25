using DTcms.Core.IServices;
using DTcms.Core.IServices.Alipay;
using Microsoft.AspNetCore.Mvc;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 支付宝支付回调
    /// </summary>
    [Route("alipay/notify")]
    [ApiController]
    public class AlipayNotifyController(IAlipayNotifyService alipayNotifyService, IOrderPaymentService orderPaymentService) : ControllerBase
    {
        private readonly IAlipayNotifyService _alipayNotifyService = alipayNotifyService;
        private readonly IOrderPaymentService _orderPaymentService = orderPaymentService;

        /// <summary>
        /// 支付成功回调
        /// 示例：/alipay/notify/transactions/app
        /// </summary>
        [HttpPost("transactions/{type}")]
        public async Task<IActionResult> Transactions([FromRoute] string type)
        {
            bool result = false;
            if (type.ToLower() == "pc")
            {
                var model = await _alipayNotifyService.PagePay(Request);
                if (model != null && model.OutTradeNo != null)
                {
                    //修改订单状态
                    result = await _orderPaymentService.ConfirmAsync(model.OutTradeNo);
                }
            }
            //返回结果
            if (!result)
            {
                return BadRequest("fail");
            }
            return Ok("success");
        }
    }
}