using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using System.Linq.Expressions;

namespace DTcms.Core.IServices
{
    public interface IBaseService
    {
        /// <summary>
        /// 检查记录是否存在
        /// </summary>
        abstract Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> funcWhere, WriteRoRead writeAndRead = WriteRoRead.Read) where T : class;

        /// <summary>
        /// 查询记录总数
        /// include用法: query => query.Include(x => x.xxx)
        /// </summary>
        abstract Task<int> CountAsync<T>(Expression<Func<T, bool>> funcWhere,
            Func<IQueryable<T>, IQueryable<T>>? include = null, WriteRoRead writeAndRead = WriteRoRead.Read) where T : class;

        /// <summary>
        /// 查询一条记录
        /// include用法: query => query.Include(x => x.xxx)
        /// </summary>
        abstract Task<T?> QueryAsync<T>(Expression<Func<T, bool>> funcWhere,
            Func<IQueryable<T>, IQueryable<T>>? include = null, WriteRoRead writeAndRead = WriteRoRead.Read) where T : class;

        /// <summary>
        /// 查询返回指定数据列表
        /// include用法: query => query.Include(x => x.xxx)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="top">显示条数</param>
        /// <param name="funcWhere">查询表达式</param>
        /// <param name="include">子集合</param>
        /// <param name="orderBy">排序</param>
        abstract Task<IEnumerable<T>> QueryListAsync<T>(int top, Expression<Func<T, bool>> funcWhere,
            Func<IQueryable<T>, IQueryable<T>>? include = null, string? orderBy = null, WriteRoRead writeAndRead = WriteRoRead.Read) where T : class;

        /// <summary>
        /// 返回分页记录，带排序
        /// include用法: query => query.Include(x => x.xxx)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="funcWhere">查询表达式</param>
        /// <param name="include">子集合</param>
        /// <param name="orderBy">排序</param>
        abstract Task<PaginationList<T>> QueryPageAsync<T>(int pageSize, int pageIndex, Expression<Func<T, bool>> funcWhere,
            Func<IQueryable<T>, IQueryable<T>>? include = null, string? orderBy = null, WriteRoRead writeAndRead = WriteRoRead.Read) where T : class;

        /// <summary>
        /// 从缓存查询一条记录
        /// include用法: query => query.Include(x => x.xxx)
        /// </summary>
        abstract Task<T?> QueryAsync<T>(string cacheKey, Expression<Func<T, bool>> funcWhere,
            Func<IQueryable<T>, IQueryable<T>>? include = null) where T : class;

        /// <summary>
        /// 从缓存中返回指定数据列表
        /// include用法: query => query.Include(x => x.xxx)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="top">显示条数</param>
        /// <param name="funcWhere">查询表达式</param>
        /// <param name="include">子集合</param>
        /// <param name="orderBy">排序</param>
        abstract Task<IEnumerable<T>> QueryListAsync<T>(string cacheKey, int top,
            Expression<Func<T, bool>> funcWhere, Func<IQueryable<T>, IQueryable<T>>? include = null, string? orderBy = null) where T : class;

        /// <summary>
        /// 从缓存中返回分页记录，带排序
        /// include用法: query => query.Include(x => x.xxx)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="funcWhere">查询表达式</param>
        /// <param name="include">子集合</param>
        /// <param name="orderBy">排序</param>
        abstract Task<PaginationList<T>> QueryPageAsync<T>(string cacheKey, int pageSize, int pageIndex,
            Expression<Func<T, bool>> funcWhere, Func<IQueryable<T>, IQueryable<T>>? include = null, string? orderBy = null) where T : class;

        /// <summary>
        /// 新增一条数据
        /// </summary>
        abstract Task<T> AddAsync<T>(T t) where T : class;

        /// <summary>
        /// 新增多条数据(带事务)
        /// </summary>
        abstract Task<IEnumerable<T>> AddAsync<T>(IEnumerable<T> tList) where T : class;

        /// <summary>
        /// 修改一条数据
        /// </summary>
        abstract Task<bool> UpdateAsync<T>(T t) where T : class;

        /// <summary>
        /// 修改多条数据(带事务)
        /// </summary>
        abstract Task<bool> UpdateAsync<T>(IEnumerable<T> tList) where T : class;

        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        abstract Task<bool> DeleteAsync<T>(Expression<Func<T, bool>> funcWhere,
            Func<IQueryable<T>, IQueryable<T>>? include = null) where T : class;

        /// <summary>
        /// 根据实体删除一条数据
        /// </summary>
        abstract Task<bool> DeleteAsync<T>(T t) where T : class;

        /// <summary>
        /// 删除多条数据(带事务)
        /// </summary>
        abstract Task<bool> DeleteAsync<T>(IEnumerable<T> tList) where T : class;

        /// <summary>
        /// 清空缓存(列表和详情)
        /// </summary>
        Task RemoveCacheAsync<T>(bool isShow = false) where T : class;

        /// <summary>
        /// 保存数据(为了保证事务)
        /// </summary>
        Task<bool> SaveAsync();
    }
}
