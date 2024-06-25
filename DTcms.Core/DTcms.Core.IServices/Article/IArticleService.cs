using DTcms.Core.Model.Models;
using System.Linq.Expressions;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 文章接口
    /// </summary>
    public interface IArticleService : IBaseService
    {
        /// <summary>
        /// 根据ID返回上一条下一条(缓存)
        /// </summary>
        Task<IEnumerable<Articles>> QueryNextByCacheAsync(string cacheKey, long id, Expression<Func<Articles, bool>> funcWhere);

        /// <summary>
        /// 更新浏览量(局部更新)
        /// </summary>
        Task<int> UpdateClickAsync(string? cacheKey, long articleId, int clickCount);

        /// <summary>
        /// 根据条件删除一条记录
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<Articles, bool>> funcWhere);
    }
}
