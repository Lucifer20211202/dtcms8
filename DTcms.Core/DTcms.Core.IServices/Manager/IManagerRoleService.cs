using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using System.Linq.Expressions;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 管理员角色接口
    /// </summary>
    public interface IManagerRoleService : IBaseService
    {
        /// <summary>
        /// 查询一条记录
        /// </summary>
        Task<ManagerRolesDto> QueryAsync(Expression<Func<ApplicationRole, bool>> funcWhere, WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 查询指定数量列表
        /// </summary>
        Task<IEnumerable<ApplicationRole>> QueryListAsync(int top, Expression<Func<ApplicationRole, bool>> funcWhere,
            string orderBy, WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 查询分页列表
        /// </summary>
        Task<PaginationList<ApplicationRole>> QueryPageAsync(int pageSize, int pageIndex, Expression<Func<ApplicationRole, bool>> funcWhere,
            string orderBy, WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 添加一条记录
        /// </summary>
        Task<int> AddAsync(ManagerRolesEditDto modelDto);

        /// <summary>
        /// 修改一条记录
        /// </summary>
        Task<bool> UpdateAsync(int id, ManagerRolesEditDto modelDto);

        /// <summary>
        /// 根据条件删除记录
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<ApplicationRole, bool>> funcWhere);
    }
}
