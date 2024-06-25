using DTcms.Core.Common.Emums;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 文章附件
    /// </summary>
    public interface IArticleAttachService : IBaseService
    {
        /// <summary>
        /// 更新下载数量
        /// </summary>
        Task<bool> UpdateDownCount(long id, WriteRoRead writeAndRead = WriteRoRead.Write);
    }
}
