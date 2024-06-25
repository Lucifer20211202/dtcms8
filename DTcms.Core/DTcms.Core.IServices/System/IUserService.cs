using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using System.Security.Claims;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// Identity用户接口
    /// </summary>
    public interface IUserService : IBaseService
    {
        /// <summary>
        /// 获取当前登录用户ID
        /// </summary>
        int GetUserId();

        /// <summary>
        /// 获取当前登录用户名
        /// </summary>
        string? GetUserName();

        /// <summary>
        /// 判断当前登录用户是否超管
        /// </summary>
        Task<bool> IsSuperAdminAsync();

        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        Task<ApplicationUser?> GetUserAsync();

        /// <summary>
        /// 获取当前登录用户角色所有Claims
        /// </summary>
        Task<List<Claim>> GetRoleClaimsAsync();

        /// <summary>
        /// 获取指定角色所有Claims
        /// </summary>
        Task<List<Claim>> GetRoleClaimsAsync(int roleId);

        /// <summary>
        /// 修改用户密码
        /// </summary>
        Task<bool> UpdatePasswordAsync(PasswordDto modelDto);
    }
}
