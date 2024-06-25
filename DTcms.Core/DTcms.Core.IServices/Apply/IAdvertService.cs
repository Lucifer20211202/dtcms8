using DTcms.Core.Model.Models;
using System.Linq.Expressions;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 广告位接口
    /// </summary>
    public interface IAdvertService : IBaseService
    {
        /// <summary>
        /// 从缓存中查询一条记录
        /// </summary>
        Task<Adverts?> QueryAsync(string cacheKey, Expression<Func<Adverts, bool>> advertWhere, Func<AdvertBanners, bool> bannerWhere);
    }
}
