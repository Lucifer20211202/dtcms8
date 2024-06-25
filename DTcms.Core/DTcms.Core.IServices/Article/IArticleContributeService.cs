using DTcms.Core.Model.ViewModels;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 文章投稿接口
    /// </summary>
    public interface IArticleContributeService : IBaseService
    {
        /// <summary>
        /// 修改一条记录
        /// </summary>
        Task<bool> UpdateAsync(int id, ArticleContributesEditDto modelDto);

        /// <summary>
        /// 修改一条记录
        /// </summary>
        Task<bool> UserUpdateAsync(int id, ArticleContributesEditDto modelDto);
    }
}
