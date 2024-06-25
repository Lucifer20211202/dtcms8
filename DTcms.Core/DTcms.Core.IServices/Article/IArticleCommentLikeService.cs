using DTcms.Core.Common.Emums;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 评论点赞接口
    /// </summary>
    public interface IArticleCommentLikeService : IBaseService
    {
        /// <summary>
        /// 更新用户点赞数据
        /// </summary>
        Task<int> UserUpdateLikeAsync(int commentId);
    }
}
