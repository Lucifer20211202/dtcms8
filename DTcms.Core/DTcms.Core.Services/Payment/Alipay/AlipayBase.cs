using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.Model.Alipay;
using DTcms.Core.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.Services.Alipay
{
    /// <summary>
    /// 支付宝支付基类
    /// </summary>
    public class AlipayBase(ISitePaymentService sitePaymentService)
    {
        private readonly ISitePaymentService _sitePaymentService = sitePaymentService;

        /// <summary>
        /// 获取支付宝账户信息
        /// </summary>
        public async Task<AlipayAccountDto> GetAccountAsync(int id)
        {
            //取得微信支付账户
            var payModel = await _sitePaymentService.QueryAsync<SitePayments>(x => x.Id == id, query => query.Include(x => x.Payment))
                ?? throw new ResponseException("支付方式有误，请联系客服");

            var model = new AlipayAccountDto
            {
                SiteId = payModel.SiteId,
                AppId = payModel.Key1,
                AlipayPublicKey = payModel.Key2,
                AppPrivateKey = payModel.Key3,
                NotifyUrl = payModel.Payment?.NotifyUrl,
                NotifyType = payModel.Type
            };
            if (string.IsNullOrEmpty(model.AppId)
                || string.IsNullOrEmpty(model.AlipayPublicKey)
                || string.IsNullOrEmpty(model.AppPrivateKey)
                || string.IsNullOrEmpty(model.NotifyUrl)
                || string.IsNullOrEmpty(model.NotifyType))
            {
                throw new ResponseException("支付宝设置有误，请联系客服");
            }
            return model;
        }
    }
}
