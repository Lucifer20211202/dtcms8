using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;

namespace DTcms.Core.Services
{
    public class SiteOAuthLoginService(IDbContextFactory contentFactory, ICacheService cacheService) : BaseService(contentFactory, cacheService), ISiteOAuthLoginService { }
}
