using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.Model.Balance;
using DTcms.Core.Model.Models;
using Microsoft.AspNetCore.Mvc;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 余额支付接口
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class BalancePayController(ISitePaymentService sitePaymentService, IOrderPaymentService orderPaymentService) : ControllerBase
    {
        private readonly ISitePaymentService _sitePaymentService = sitePaymentService;
        private readonly IOrderPaymentService _orderPaymentService = orderPaymentService;

        /// <summary>
        /// 余额支付统一下单
        /// 示例：/balancepay
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Pay(BalancePayDto modelDto)
        {
            if (modelDto.OutTradeNo == null)
            {
                throw new ResponseException("订单交易号不允许为空");
            }
            //查询订单状态
            var model = await _orderPaymentService.QueryAsync<OrderPayments>(x => x.TradeNo == modelDto.OutTradeNo, null, WriteRoRead.Write);
            if (model == null)
            {
                throw new ResponseException("订单交易号有误，请重试");
            }
            if (model.TradeType == 1)
            {
                throw new ResponseException("余额支付不允许充值");
            }
            if (model.Status == 2)
            {
                throw new ResponseException("订单已支付，请勿重复付款");
            }
            if (model.Status == 3)
            {
                throw new ResponseException("订单已取消，无法再次付款");
            }
            if (model.PaymentAmount <= 0)
            {
                throw new ResponseException("支付金额必须大于零");
            }

            //判断支付接口类型
            var pay = await _orderPaymentService.QueryAsync<SitePayments>(x => x.Id == model.PaymentId && x.Type == "balance");
            if (pay == null)
            {
                throw new ResponseException("支付方式有误，请重试");
            }
            //修改订单状态
            var result = await _orderPaymentService.ConfirmAsync(modelDto.OutTradeNo);
            if (!result)
            {
                throw new ResponseException("确认余额支付失败，请重试");
            }
            return NoContent();
        }

    }
}
