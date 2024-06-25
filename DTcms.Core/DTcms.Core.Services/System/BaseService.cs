using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Extensions;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 构造函数
    /// 依赖注入数据库工厂
    /// </summary>
    public class BaseService(IDbContextFactory contextFactory, ICacheService cacheService) : IBaseService
    {
        protected DbContext? _context { get; set; } //DbContext对象
        protected IDbContextFactory _contextFactory { get; private set; } = contextFactory; //依赖注入工厂
        protected ICacheService _cacheService { get; private set; } = cacheService; //缓存对象

        /// <summary>
        /// 检查记录是否存在
        /// </summary>
        public virtual async Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> funcWhere, WriteRoRead writeAndRead = WriteRoRead.Read) where T : class
        {
            _context = _contextFactory.CreateContext(writeAndRead);
            return await _context.Set<T>().AnyAsync(funcWhere);
        }

        /// <summary>
        /// 查询记录总数
        /// </summary>
        public virtual async Task<int> CountAsync<T>(Expression<Func<T, bool>> funcWhere,
            Func<IQueryable<T>, IQueryable<T>>? include = null, WriteRoRead writeAndRead = WriteRoRead.Read) where T : class
        {
            _context = _contextFactory.CreateContext(writeAndRead);
            IQueryable<T> query = _context.Set<T>();

            if (include != null) query = include(query);

            return await query.Where(funcWhere).CountAsync();
        }

        /// <summary>
        /// 查询一条记录
        /// include用法: query => query.Include(x => x.a).Include(x => x.b)
        /// </summary>
        public virtual async Task<T?> QueryAsync<T>(Expression<Func<T, bool>> funcWhere,
            Func<IQueryable<T>, IQueryable<T>>? include = null, WriteRoRead writeAndRead = WriteRoRead.Read) where T : class
        {
            _context = _contextFactory.CreateContext(writeAndRead);
            IQueryable<T> query = _context.Set<T>();

            if (include != null) query = include(query);

            return await query.FirstOrDefaultAsync(funcWhere);
        }

        /// <summary>
        /// 查询返回指定数据列表
        /// include用法: query => query.Include(x => x.a).Include(x => x.b)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="top">显示条数</param>
        /// <param name="funcWhere">查询表达式</param>
        /// <param name="include">子集合</param>
        /// <param name="orderBy">排序</param>
        public virtual async Task<IEnumerable<T>> QueryListAsync<T>(int top, Expression<Func<T, bool>> funcWhere,
            Func<IQueryable<T>, IQueryable<T>>? include = null, string? orderBy = null, WriteRoRead writeAndRead = WriteRoRead.Read) where T : class
        {
            _context = _contextFactory.CreateContext(writeAndRead);
            IQueryable<T> query = _context.Set<T>().Where(funcWhere);

            if (top > 0) query = query.Take(top);
            if (include != null) query = include(query);
            if (orderBy != null) query = LinqExtensions.OrderByBatch<T>(query, orderBy);

            return await query.ToListAsync();
        }

        /// <summary>
        /// 返回分页记录，带排序
        /// include用法: query => query.Include(x => x.a).Include(x => x.b)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="funcWhere">查询表达式</param>
        /// <param name="include">子集合</param>
        /// <param name="orderBy">排序</param>
        public virtual async Task<PaginationList<T>> QueryPageAsync<T>(int pageSize, int pageIndex, Expression<Func<T, bool>> funcWhere,
            Func<IQueryable<T>, IQueryable<T>>? include = null, string? orderBy = null, WriteRoRead writeAndRead = WriteRoRead.Read) where T : class
        {
            _context = _contextFactory.CreateContext(writeAndRead);
            IQueryable<T> query = _context.Set<T>().Where(funcWhere);

            if (include != null) query = include(query);
            if (orderBy != null) query = LinqExtensions.OrderByBatch<T>(query, orderBy);

            return await PaginationList<T>.CreateAsync(pageIndex, pageSize, query);
        }

        /// <summary>
        /// 从缓存查询一条记录
        /// include用法: query => query.Include(x => x.a).Include(x => x.b)
        /// </summary>
        public virtual async Task<T?> QueryAsync<T>(string cacheKey, Expression<Func<T, bool>> funcWhere,
            Func<IQueryable<T>, IQueryable<T>>? include = null) where T : class
        {
            string className = typeof(T).Name; //获取类名的字符串
            string classKey = $"{className}:Show:{cacheKey}";

            return await _cacheService.GetOrSetAsync<T>(classKey, async () =>
            {
                _context = _contextFactory.CreateContext(WriteRoRead.Read);
                IQueryable<T> query = _context.Set<T>();

                if (include != null) query = include(query);

                return await query.FirstOrDefaultAsync(funcWhere);
            });
        }

        /// <summary>
        /// 从缓存中返回指定数据列表
        /// include用法: query => query.Include(x => x.a).Include(x => x.b)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="top">显示条数</param>
        /// <param name="funcWhere">查询表达式</param>
        /// <param name="include">子集合</param>
        /// <param name="orderBy">排序</param>
        public virtual async Task<IEnumerable<T>> QueryListAsync<T>(string cacheKey, int top,
            Expression<Func<T, bool>> funcWhere, Func<IQueryable<T>, IQueryable<T>>? include = null, string? orderBy = null) where T : class
        {
            string className = typeof(T).Name; //获取类名的字符串
            string classKey = $"{className}:List:{cacheKey}";

            return await _cacheService.GetOrSetAsync(classKey, async () =>
            {
                _context = _contextFactory.CreateContext(WriteRoRead.Read);
                IQueryable<T> query = _context.Set<T>().Where(funcWhere);

                if (top > 0) query = query.Take(top);
                if (include != null) query = include(query);
                if (orderBy != null) query = LinqExtensions.OrderByBatch<T>(query, orderBy);

                return await query.ToListAsync();
            }) ?? [];
        }

        /// <summary>
        /// 从缓存中返回分页记录，带排序
        /// include用法: query => query.Include(x => x.a).Include(x => x.b)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="funcWhere">查询表达式</param>
        /// <param name="include">子集合</param>
        /// <param name="orderBy">排序</param>
        public virtual async Task<PaginationList<T>> QueryPageAsync<T>(string cacheKey, int pageSize, int pageIndex,
            Expression<Func<T, bool>> funcWhere, Func<IQueryable<T>, IQueryable<T>>? include = null, string? orderBy = null) where T : class
        {
            string className = typeof(T).Name; //获取类名的字符串
            string classKey = $"{className}:List:{cacheKey}";

            return await _cacheService.GetOrSetAsync(classKey, async () =>
            {
                _context = _contextFactory.CreateContext(WriteRoRead.Read);
                IQueryable<T> query = _context.Set<T>().Where(funcWhere);

                if (include != null) query = include(query);
                if (orderBy != null) query = LinqExtensions.OrderByBatch<T>(query, orderBy);

                return await PaginationList<T>.CreateAsync(pageIndex, pageSize, query);
            }) ?? new PaginationList<T>(0, pageIndex, pageSize, []);
        }

        /// <summary>
        /// 新增一条数据
        /// </summary>
        public virtual async Task<T> AddAsync<T>(T t) where T : class
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);
            await _context.Set<T>().AddAsync(t);
            await this.SaveAsync();

            //删除缓存(列表和详情)
            await RemoveCacheAsync<T>();

            return t;
        }

        /// <summary>
        /// 新增多条数据(带事务)
        /// </summary>
        public virtual async Task<IEnumerable<T>> AddAsync<T>(IEnumerable<T> tList) where T : class
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);
            await _context.Set<T>().AddRangeAsync(tList);
            await this.SaveAsync();

            //删除缓存(列表和详情)
            await RemoveCacheAsync<T>();

            return tList;
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        public virtual async Task<bool> UpdateAsync<T>(T t) where T : class
        {
            //注意：如果已经连接则不需再连接数据库
            _context ??= _contextFactory.CreateContext(WriteRoRead.Write);
            if (t == null)
            {
                throw new ResponseException("数据不能为NULL");
            }
            _context.Set<T>().Attach(t);
            _context.Entry<T>(t).State = EntityState.Modified;
            var result = await this.SaveAsync();

            //删除缓存(列表和详情)
            await RemoveCacheAsync<T>(true);

            return result;
        }

        /// <summary>
        /// 修改多条数据(带事务)
        /// </summary>
        public virtual async Task<bool> UpdateAsync<T>(IEnumerable<T> tList) where T : class
        {
            //注意：如果已经连接则不需再连接数据库
            _context ??= _contextFactory.CreateContext(WriteRoRead.Write);
            foreach (var t in tList)
            {
                _context.Set<T>().Attach(t);
                _context.Entry<T>(t).State = EntityState.Modified;
            }
            var result = await this.SaveAsync();

            //删除缓存(列表和详情)
            await RemoveCacheAsync<T>(true);

            return result;
        }

        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        public virtual async Task<bool> DeleteAsync<T>(Expression<Func<T, bool>> funcWhere,
            Func<IQueryable<T>, IQueryable<T>>? include = null) where T : class
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);
            IQueryable<T> query = _context.Set<T>();

            if (include != null) query = include(query);
            IEnumerable<T> tList = await query.Where(funcWhere).ToListAsync();

            if (!tList.Any())
            {
                return false;
            }
            foreach (var t in tList)
            {
                _context.Set<T>().Attach(t);
            }
            _context.Set<T>().RemoveRange(tList);
            var result = await this.SaveAsync();

            //删除缓存(列表和详情)
            await RemoveCacheAsync<T>(true);

            return result;
        }

        /// <summary>
        /// 根据实体删除一条数据
        /// </summary>
        public virtual async Task<bool> DeleteAsync<T>(T t) where T : class
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);
            if (t == null)
            {
                throw new ResponseException("数据不存在或已删除");
            }
            _context.Set<T>().Attach(t);
            _context.Set<T>().Remove(t);
            var result = await this.SaveAsync();

            //删除缓存(列表和详情)
            await RemoveCacheAsync<T>(true);

            return result;
        }

        /// <summary>
        /// 删除多条数据(带事务)
        /// </summary>
        public virtual async Task<bool> DeleteAsync<T>(IEnumerable<T> tList) where T : class
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);
            foreach (var t in tList)
            {
                _context.Set<T>().Attach(t);
            }
            _context.Set<T>().RemoveRange(tList);
            var result = await this.SaveAsync();

            //删除缓存(列表和详情)
            await RemoveCacheAsync<T>(true);

            return result;
        }

        /// <summary>
        /// 清空缓存(列表和详情)
        /// </summary>
        public async Task RemoveCacheAsync<T>(bool isShow = false) where T : class
        {
            string className = typeof(T).Name; //获取类名的字符串
            await _cacheService.RemovePatternAsync($"{className}:List"); //删除缓存
            if (isShow)
            {
                await _cacheService.RemovePatternAsync($"{className}:Show"); //删除缓存
            }
        }

        /// <summary>
        /// 保存数据(异步为了保证事务)
        /// </summary>
        public async Task<bool> SaveAsync()
        {
            if (_context == null) return false;
            return (await _context.SaveChangesAsync() >= 0);
        }

        public virtual void Dispose()
        {
            _context?.Dispose();
        }
    }
}
