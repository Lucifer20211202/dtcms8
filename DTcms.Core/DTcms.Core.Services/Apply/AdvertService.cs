using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Extensions;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Linq.Expressions;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 广告位接口实现
    /// </summary>
    public class AdvertService(IDbContextFactory contentFactory, ICacheService cacheService) : BaseService(contentFactory, cacheService), IAdvertService
    {
        /// <summary>
        /// 从缓存中查询一条记录
        /// </summary>
        public async Task<Adverts?> QueryAsync(string cacheKey, Expression<Func<Adverts, bool>> advertWhere, Func<AdvertBanners, bool> bannerWhere)
        {
            string className = typeof(Adverts).Name; //获取类名的字符串
            string classKey = $"{className}:Show:{cacheKey}";

            return await _cacheService.GetOrSetAsync<Adverts>(classKey, async () =>
            {
                _context = _contextFactory.CreateContext(WriteRoRead.Read);//连接数据库
                var result = await _context.Set<Adverts>().Include(x => x.Banners).FirstOrDefaultAsync(advertWhere);
                if (result != null && result.Banners.Count > 0)
                {
                    result.Banners = result.Banners.Where(bannerWhere).OrderBy(x => x.SortId).ToList();
                }
                return result;
            });
        }
    }
}