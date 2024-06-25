using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 友情链接接口实现
    /// </summary>
    public class LinkService(IDbContextFactory contentFactory, ICacheService cacheService) : BaseService(contentFactory, cacheService), ILinkService { }
}
