using DTcms.Core.Common.Emums;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 广告内容接口实现
    /// </summary>
    public class AdvertBannerService(IDbContextFactory contentFactory, ICacheService cacheService)
        : BaseService(contentFactory, cacheService), IAdvertBannerService { }
}
