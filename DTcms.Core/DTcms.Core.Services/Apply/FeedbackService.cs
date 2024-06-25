using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 留言反馈接口实现
    /// </summary>
    public class FeedbackService(IDbContextFactory contentFactory, ICacheService cacheService)
        : BaseService(contentFactory, cacheService), IFeedbackService { }
}
