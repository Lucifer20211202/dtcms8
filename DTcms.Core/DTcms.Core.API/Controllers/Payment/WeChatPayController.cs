using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.IServices.WeChat;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.WeChat;
using Microsoft.AspNetCore.Mvc;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 微信支付下单
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class WeChatPayController(IWeChatExecuteService weChatExecuteService, IOrderPaymentService orderPaymentService) : ControllerBase
    {
        private readonly IWeChatExecuteService _weChatExecuteService = weChatExecuteService;
        private readonly IOrderPaymentService _orderPaymentService = orderPaymentService;

        /// <summary>
        /// 微信统一下单
        /// 示例：/wechatpay
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Pay(WeChatPayDto modelDto)
        {
            if (modelDto.OutTradeNo == null)
            {
                throw new ResponseException("无法获取订单号有误，请重试");
            }
            //查询订单状态
            var model = await _orderPaymentService.QueryAsync<OrderPayments>(x => x.TradeNo == modelDto.OutTradeNo, null, WriteRoRead.Write)
                ?? throw new ResponseException("订单交易号有误，请重试");
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

            //判断支付接口类型
            var pay = await _orderPaymentService.QueryAsync<SitePayments>(x => x.Id == modelDto.PaymentId);
            if (pay == null)
            {
                throw new ResponseException("支付方式有误，请重试");
            }
            if (pay.Type == "native")
            {
                return Ok(await _weChatExecuteService.NativePayAsync(modelDto));
            }
            throw new ResponseException("支付方式有误，请重试");
        }

        /// <summary>
        /// 返回授权拼接好地址
        /// 示例：/wechatpay/oauth
        /// </summary>
        [HttpPost("oauth")]
        public async Task<IActionResult> OAuth(WeChatPayOAuthDto modelDto)
        {
            if (modelDto.OutTradeNo == null)
            {
                throw new ResponseException("无法获取订单号有误，请重试");
            }
            //查询订单状态
            var model = await _orderPaymentService.QueryAsync<OrderPayments>(x => x.TradeNo == modelDto.OutTradeNo, null, WriteRoRead.Write)
                ?? throw new ResponseException("订单交易号有误，请重试");
            modelDto.PaymentId = model.PaymentId.GetValueOrDefault();

            //调用授权返回URL
            var result = await _weChatExecuteService.OAuthAsync(modelDto);
            return Ok(new { url = result });
        }
    }
}