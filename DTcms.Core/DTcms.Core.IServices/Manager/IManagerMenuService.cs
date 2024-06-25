using DTcms.Core.Common.Emums;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using System.Linq.Expressions;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 导航菜单接口
    /// </summary>
    public interface IManagerMenuService : IBaseService
    {
        /// <summary>
        /// 通过站点ID查找菜单
        /// </summary>
        Task<ManagerMenus?> QueryBySiteIdAsync(int siteId, WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 返回所有菜单目录树
        /// </summary>
        Task<IEnumerable<ManagerMenusDto>> QueryListAsync(int parentId, WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 返回当前用户所有菜单目录树
        /// </summary>
        Task<IEnumerable<ManagerMenusClientDto>> QueryListAsync(WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 返回指定角色所有菜单目录树
        /// </summary>
        Task<IEnumerable<ManagerMenuRolesDto>> QueryListByRoleIdAsync(int roleId, WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 根据导航类别查询模型列表
        /// </summary>
        Task<IEnumerable<ManagerMenuModels>> QueryModelAsync(NavType navType, WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 根据条件删除数据(迭代删除)
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<ManagerMenus, bool>> funcWhere);
    }
}
