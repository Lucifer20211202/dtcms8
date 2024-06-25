using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 会员附件下载记录接口实现
    /// </summary>
    public class MemberAttachRecordService(IDbContextFactory contentFactory, ICacheService cacheService)
        : BaseService(contentFactory, cacheService), IMemberAttachRecordService { }
}
