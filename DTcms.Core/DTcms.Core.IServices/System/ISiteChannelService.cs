using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using System.Linq.Expressions;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 站点频道接口
    /// </summary>
    public interface ISiteChannelService : IBaseService
    {
        /// <summary>
        /// 添加频道(含菜单导航)
        /// </summary>
        Task<SiteChannels> AddAsync(SiteChannelsEditDto modelDto);

        /// <summary>
        /// 修改一条记录
        /// </summary>
        Task<bool> UpdateAsync(int id, SiteChannelsEditDto modelDto);

        /// <summary>
        /// 删除频道(含扩展字段及菜单)
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<SiteChannels, bool>> funcWhere);
    }
}
