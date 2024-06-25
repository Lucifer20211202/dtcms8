using DTcms.Core.Common.Emums;
using DTcms.Core.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DTcms.Core.API.Handler
{
    /// <summary>
    /// 授权资源处理Handler
    /// </summary>
    public class PermissionAuthorizationHandler(RoleManager<ApplicationRole> roleManager) : AuthorizationHandler<OperationAuthorizationRequirement>
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

        /// <summary>
        /// 实现接口方法
        /// </summary>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
        {
            //取得当前Claim里的角色名称
            List<Claim> claimsList = context.User.Claims.Where(t => t.Type == ClaimTypes.Role).ToList();
            if (claimsList != null)
            {
                //遍历用户所拥有的角色
                foreach (var claim in claimsList)
                {
                    var role = await _roleManager.FindByNameAsync(claim.Value);
                    if (role == null)
                    {
                        continue;
                    }
                    //如果是超级管理员则直接允许访问
                    if(role.RoleType == (int)RoleType.SuperAdmin)
                    {
                        context.Succeed(requirement);
                        return;
                    }
                    //验证角色的权限是否一致
                    IList<Claim> roleClaims = await _roleManager.GetClaimsAsync(role);
                    if (roleClaims != null && roleClaims.Any(x => x.Value.Equals(requirement.Name, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }
    }
}