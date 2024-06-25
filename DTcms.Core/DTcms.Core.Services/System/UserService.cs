﻿using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DTcms.Core.Services
{
    /// <summary>
    /// Identity用户接口实现
    /// </summary>
    public class UserService(IDbContextFactory contentFactory,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IHttpContextAccessor httpContextAccessor,
        ICacheService cacheService) : BaseService(contentFactory, cacheService), IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

        /// <summary>
        /// 获取当前登录用户ID
        /// </summary>
        public int GetUserId()
        {
            var strUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(strUserId, out int userId))
            {
                return userId;
            }
            return 0;
        }

        /// <summary>
        /// 获取当前登录用户名
        /// </summary>
        public string? GetUserName()
        {
            return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        }

        /// <summary>
        /// 判断当前登录用户是否超管
        /// </summary>
        public async Task<bool> IsSuperAdminAsync()
        {
            //获得当前用户组
            var claimsList = _httpContextAccessor.HttpContext.User.Claims.Where(t => t.Type == ClaimTypes.Role).ToList();
            if (claimsList == null)
            {
                return false;
            }
            //遍历用户所拥有的角色
            foreach (var claim in claimsList)
            {
                var role = await _roleManager.FindByNameAsync(claim.Value);
                if (role == null)
                {
                    continue;
                }
                //如果是超级管理员则直接允许访问
                if (role.RoleType == (int)RoleType.SuperAdmin)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        public async Task<ApplicationUser?> GetUserAsync()
        {
            if (_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) == null)
            {
                return null;
            }
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return null;
            }
            return await _userManager.FindByIdAsync(userId);
        }

        /// <summary>
        /// 获取当前登录用户角色所有Claims
        /// </summary>
        public async Task<List<Claim>> GetRoleClaimsAsync()
        {
            List<Claim> claimList = [];
            //获得当前用户组
            List<Claim> claimRoleList = _httpContextAccessor.HttpContext.User.Claims.Where(t => t.Type == ClaimTypes.Role).ToList();
            if (claimRoleList == null)
            {
                return claimList;
            }
            //遍历用户所拥有的角色
            foreach (var claimRole in claimRoleList)
            {
                var role = await _roleManager.FindByNameAsync(claimRole.Value);
                if (role == null)
                {
                    continue;
                }
                //验证角色的权限是否一致
                IList<Claim> roleClaims = await _roleManager.GetClaimsAsync(role);
                if (roleClaims == null)
                {
                    continue;
                }
                foreach (var claim in roleClaims)
                {
                    if (claimList.Any(x => x.Type == claim.Type && x.Value == claim.Value))
                    {
                        continue;
                    }
                    claimList.Add(claim);
                }
            }
            return claimList;
        }

        /// <summary>
        /// 获取指定角色所有Claims
        /// </summary>
        public async Task<List<Claim>> GetRoleClaimsAsync(int roleId)
        {
            List<Claim> claimList = [];
            //获取角色的Claims
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                return claimList;
            }
            IList<Claim> roleClaims = await _roleManager.GetClaimsAsync(role);
            if (roleClaims == null)
            {
                return claimList;
            }
            foreach (var claim in roleClaims)
            {
                if (claimList.Any(x => x.Type == claim.Type && x.Value == claim.Value))
                {
                    continue;
                }
                claimList.Add(claim);
            }
            return claimList;
        }

        /// <summary>
        /// 修改当前用户密码
        /// </summary>
        public async Task<bool> UpdatePasswordAsync(PasswordDto modelDto)
        {
            var userId = (_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value) ?? throw new ResponseException("尚未登录或已超时，请登录后操作", ErrorCode.TokenExpired);
            var model = await _userManager.FindByIdAsync(userId) ?? throw new ResponseException("尚未登录或已超时，请登录后操作", ErrorCode.TokenExpired);
            if (modelDto.Password == null)
            {
                throw new ResponseException("请输入旧密码");
            }
            if (modelDto.NewPassword == null || modelDto.NewPassword != modelDto.ConfirmPassword)
            {
                throw new ResponseException("两次输入的密码不一至，请重试");
            }
            if(!await _userManager.CheckPasswordAsync(model, modelDto.Password))
            {
                throw new ResponseException("旧密码不正确，请重试");
            }

            var result = await _userManager.ChangePasswordAsync(model, modelDto.Password, modelDto.NewPassword);
            if (!result.Succeeded)
            {
                throw new ResponseException($"错误代码：{result.Errors.FirstOrDefault()?.Code}");
            }
            return true;
        }

    }
}