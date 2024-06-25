using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 会员组接口实现
    /// </summary>
    public class MemberGroupService(IDbContextFactory contentFactory, ICacheService cacheService)
        : BaseService(contentFactory, cacheService), IMemberGroupService { }
}
