using AutoMapper;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.Services
{
    public class ManagerService(IDbContextFactory contentFactory, ICacheService cacheService,
        IMapper mapper,IUserService userService,UserManager<ApplicationUser> userManager,RoleManager<ApplicationRole> roleManager)
        : BaseService(contentFactory, cacheService), IManagerService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IUserService _userService = userService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

        /// <summary>
        /// 添加管理员
        /// </summary>
        public async Task<ManagersDto> AddAsync(ManagersEditDto modelDto)
        {
            //检查角色是否存在
            var role = await _roleManager.FindByIdAsync(modelDto.RoleId.ToString())
                ?? throw new ResponseException("指定的角色不存在");
            if (string.IsNullOrWhiteSpace(modelDto.Password))
            {
                throw new ResponseException("密码不能为空");
            }

            //创建用户对象
            var user = new ApplicationUser()
            {
                UserName = modelDto.UserName,
                Email = modelDto.Email,
                PhoneNumber = modelDto.Phone,
                Status = modelDto.Status
            };
            //将用户与角色关联
            user.UserRoles = new List<ApplicationUserRole>()
            {
                new()
                {
                    RoleId=role.Id,
                    UserId=user.Id
                }
            };
            //将用户管理员信息关联
            if (role.RoleType > 0)
            {
                //否则新增管理员信息表
                user.Manager = new Managers()
                {
                    UserId = user.Id,
                    Avatar = modelDto.Avatar,
                    RealName = modelDto.RealName,
                    IsAudit = modelDto.IsAudit,
                    AddTime = DateTime.Now
                };
            }
            //HASH密码，保存用户
            var result = await _userManager.CreateAsync(user, modelDto.Password);
            if (!result.Succeeded)
            {
                throw new ResponseException($"{result.Errors.FirstOrDefault()?.Description}");
            }
            //映射成DTO
            return _mapper.Map<ManagersDto>(user.Manager);
        }

        /// <summary>
        /// 修改管理员
        /// </summary>
        public async Task<bool> UpdateAsync(int userId, ManagersEditDto modelDto)
        {
            //检查管理员信息是否存在
            var manager = await this.QueryAsync<Managers>(x => x.UserId == userId, query => query.Include(x => x.User).ThenInclude(x => x!.UserRoles))
                ?? throw new ResponseException("管理员不存在或已删除");
            var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new ResponseException("用户不存在或已删除");
            //检查角色是否存在
            var role = await _roleManager.FindByIdAsync(modelDto.RoleId.ToString())
                ?? throw new ResponseException("指定的角色不存在");

            //用户信息
            if (!string.IsNullOrWhiteSpace(modelDto.UserName))
            {
                user.UserName = modelDto.UserName;
            }
            user.Email = modelDto.Email;
            user.PhoneNumber = modelDto.Phone;
            user.Status = modelDto.Status;
            //管理员信息
            user.Manager = new Managers()
            {
                Id = manager.Id,
                UserId = manager.UserId,
                Avatar = modelDto.Avatar,
                RealName = modelDto.RealName,
                IsAudit = modelDto.IsAudit,
                AddTime = manager.AddTime
            };
            var result = await _userManager.UpdateAsync(user);
            //异常错误提示
            if (!result.Succeeded)
            {
                throw new ResponseException($"{result.Errors.FirstOrDefault()?.Description}");
            }
            //角色信息更新，查找该用户拥有的角色删除
            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);
            //添加新角色
            await _userManager.AddToRoleAsync(user, role.Name ?? string.Empty);
            //如果密码不为空，则重置密码
            if (!string.IsNullOrWhiteSpace(modelDto.Password))
            {
                //生成token，用于重置密码
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                //重置密码
                await _userManager.ResetPasswordAsync(user, token, modelDto.Password);
            }
            return true;
        }

        /// <summary>
        /// 删除管理员
        /// </summary>
        public async Task<bool> DeleteAsync(int userId)
        {
            //检查管理员信息是否存在
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }
            //删除管理员,角色,用户
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }
    }
}
