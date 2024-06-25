using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;

namespace DTcms.Core.Services
{
    public class SitePaymentService(IDbContextFactory contentFactory, ICacheService cacheService) : BaseService(contentFactory,cacheService), ISitePaymentService{ }
}
