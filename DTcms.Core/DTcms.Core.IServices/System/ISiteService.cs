using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using System.Linq.Expressions;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 站点信息接口
    /// </summary>
    public interface ISiteService : IBaseService
    {
        /// <summary>
        /// 添加站点(含菜单创建)
        /// </summary>
        Task<Sites> AddAsync(SitesEditDto modelDto);

        /// <summary>
        /// 修改站点(含菜单修改)
        /// </summary>
        Task<bool> UpdateAsync(int id, SitesEditDto modelDto);

        /// <summary>
        /// 删除站点(含菜单删除)
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<Sites, bool>> funcWhere);
    }
}
