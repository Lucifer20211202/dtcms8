using DTcms.Core.Common.Emums;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using System.Linq.Expressions;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 站点菜单接口
    /// </summary>
    public interface ISiteMenuService : IBaseService
    {
        /// <summary>
        /// 根据父ID返回目录树
        /// </summary>
        Task<IEnumerable<SiteMenusDto>> QueryListAsync(int parentId, WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 根据条件删除数据(迭代删除)
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<SiteMenus, bool>> funcWhere);

        /// <summary>
        /// 从缓存中返回目录树
        /// </summary>
        Task<IEnumerable<SiteMenusDto>> QueryListAsync(string cacheKey, int parentId);
    }
}
