using DTcms.Core.Model.ViewModels;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 管理员信息接口
    /// </summary>
    public interface IManagerService : IBaseService
    {
        /// <summary>
        /// 添加管理员
        /// </summary>
        Task<ManagersDto> AddAsync(ManagersEditDto modelDto);

        /// <summary>
        /// 修改管理员
        /// </summary>
        Task<bool> UpdateAsync(int userId, ManagersEditDto modelDto);

        /// <summary>
        /// 删除管理员
        /// </summary>
        Task<bool> DeleteAsync(int userId);
    }
}
