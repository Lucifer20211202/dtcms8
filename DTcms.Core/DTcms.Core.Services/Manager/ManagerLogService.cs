using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 管理日志实现
    /// </summary>
    public class ManagerLogService(IDbContextFactory contentFactory, ICacheService cacheService)
        : BaseService(contentFactory, cacheService), IManagerLogService { }
}
