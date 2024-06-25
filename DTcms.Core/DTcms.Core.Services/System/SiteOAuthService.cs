using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 开放平台接口实现
    /// </summary>
    public class SiteOAuthService(IDbContextFactory contentFactory, ICacheService cacheService) : BaseService(contentFactory, cacheService), ISiteOAuthService { }
}
