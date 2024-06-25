using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.IServices.Alipay;
using DTcms.Core.Model.Alipay;
using DTcms.Core.Model.Models;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 支付宝支付下单
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class AlipayController(IAlipayExecuteService alipayExecuteService, IOrderPaymentService orderPaymentService) : ControllerBase
    {
        private readonly IAlipayExecuteService _alipayExecuteService = alipayExecuteService;
        private readonly IOrderPaymentService _orderPaymentService = orderPaymentService;

        /// <summary>
        /// 支付宝统一下单
        /// 示例：/alipay
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Pay(AlipayTradeDto modelDto)
        {
            if (modelDto.OutTradeNo == null)
            {
                throw new ResponseException("无法获取订单号有误，请重试");
            }
            //查询订单状态
            var model = await _orderPaymentService.QueryAsync<OrderPayments>(x => x.TradeNo == modelDto.OutTradeNo, null, WriteRoRead.Write)
                ?? throw new ResponseException("订单号有误，请重试");
            if (model.Status == 2)
            {
                throw new ResponseException("订单已支付，请勿重复付款");
            }
            if (model.Status == 3)
            {
                throw new ResponseException("订单已取消，无法再次支付");
            }
            if (model.PaymentAmount <= 0)
            {
                throw new ResponseException("支付金额必须大于零");
            }
            //赋值支付方式及总金额
            modelDto.PaymentId = model.PaymentId.GetValueOrDefault();
            modelDto.Total = model.PaymentAmount;

            //防止用户URL编码，URL解码
            modelDto.ReturnUri = HttpUtility.UrlDecode(modelDto.ReturnUri);
            //判断支付接口类型
            var pay = await _orderPaymentService.QueryAsync<SitePayments>(x => x.Id == modelDto.PaymentId)
                ?? throw new ResponseException("支付方式有误，请重试");
            if (pay.Type == "pc")
            {
                return Ok(await _alipayExecuteService.PcPayAsync(modelDto));
            }
            throw new ResponseException("支付方式有误，请重试");
        }
    }
}
