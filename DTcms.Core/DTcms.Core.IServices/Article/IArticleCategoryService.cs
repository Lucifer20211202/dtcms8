using DTcms.Core.Common.Emums;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using System.Linq.Expressions;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 文章类别接口
    /// </summary>
    public interface IArticleCategoryService : IBaseService
    {
        /// <summary>
        /// 根据父ID返回目录树
        /// </summary>
        Task<IEnumerable<ArticleCategorysDto>> QueryListAsync(int channelId, int parentId, WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 更新一条数据(重组父子关系)
        /// </summary>
        /// <returns></returns>
        Task<bool> UpdateAsync(int id, ArticleCategorysEditDto modelDto);

        /// <summary>
        /// 根据条件删除数据(迭代删除)
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<ArticleCategorys, bool>> funcWhere);

        /// <summary>
        /// 根据父ID返回一级列表(缓存带文章)
        /// </summary>
        Task<IEnumerable<ArticleCategorysClientDto>> QueryArticleListAsync(string cacheKey, int channelId, int categoryTop, int articleTop, int status,
            Expression<Func<Articles, bool>> funcWhere, string orderBy);

        /// <summary>
        /// 根据父ID返回一级列表(缓存)
        /// </summary>
        Task<IEnumerable<ArticleCategorys>> QueryListByCacheAsync(string cacheKey, int top, int channelId, int parentId);

        /// <summary>
        /// 根据父ID返回目录树(缓存)
        /// </summary>
        Task<IEnumerable<ArticleCategorysDto>> QueryListByCacheAsync(string cacheKey, int channelId, int parentId);
    }
}
