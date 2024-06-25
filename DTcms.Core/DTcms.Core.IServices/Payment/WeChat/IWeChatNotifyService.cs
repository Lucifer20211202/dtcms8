using DTcms.Core.Model.WeChat;
using Microsoft.AspNetCore.Http;

namespace DTcms.Core.IServices.WeChat
{
    public interface IWeChatNotifyService
    {
        /// <summary>
        /// 确认解密回调信息
        /// </summary>
        Task<WeChatPayNotifyDecryptDto?> ConfirmAsync(HttpRequest request, int paymentId);
    }
}
