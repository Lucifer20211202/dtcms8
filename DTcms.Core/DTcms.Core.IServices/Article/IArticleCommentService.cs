using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using System.Linq.Expressions;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 文章评论接口
    /// </summary>
    public interface IArticleCommentService : IBaseService
    {
        /// <summary>
        /// 查询前几条数据
        /// </summary>
        Task<IEnumerable<ArticleCommentsDto>> QueryListAsync(int top, Expression<Func<ArticleComments, bool>> funcWhere, string orderBy,
            WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 查询分页列表
        /// </summary>
        Task<PaginationList<ArticleCommentsDto>> QueryPageAsync(int pageSize, int pageIndex, Expression<Func<ArticleComments, bool>> funcWhere,
            string orderBy, WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 查询前几条数据(缓存)
        /// </summary>
        Task<IEnumerable<ArticleCommentsDto>> QueryListByCacheAsync(string cacheKey, int top, Expression<Func<ArticleComments, bool>> funcWhere, string orderBy,
            WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 查询分页列表(缓存)
        /// </summary>
        Task<PaginationList<ArticleCommentsDto>> QueryPageByCacheAsync(string cacheKey, int pageSize, int pageIndex, Expression<Func<ArticleComments, bool>> funcWhere,
            string orderBy, WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 添加一条数据
        /// </summary>
        Task<ArticleComments?> AddAsync(ArticleCommentsEditDto modelDto);

    }
}
