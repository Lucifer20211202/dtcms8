using DTcms.Core.Common.Emums;
using DTcms.Core.Model.ViewModels;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 手机短信接口
    /// </summary>
    public interface ISmsService : IBaseService
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        Task<string> Send(SmsMessageDto modelDto, WriteRoRead writeAndRead = WriteRoRead.Read);
    }
}
