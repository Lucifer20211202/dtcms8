using DTcms.Core.Common.Emums;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 文章点赞接口
    /// </summary>
    public interface IArticleLikeService : IBaseService
    {
        /// <summary>
        /// 更新用户点赞数据
        /// </summary>
        Task<int> UserUpdateLikeAsync(long articleId);
    }
}
