using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Extensions;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 会员信息接口实现
    /// </summary>
    public class MemberService(IDbContextFactory contentFactory, ICacheService cacheService,
        UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
        IHttpContextAccessor httpContextAccessor) : BaseService(contentFactory, cacheService), IMemberService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

        /// <summary>
        /// 获取记录总数量
        /// </summary>
        public async Task<int> QueryCountAsync(Expression<Func<Members, bool>> funcWhere, WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库
            return await _context.Set<Members>().Include(x => x.User).Where(funcWhere).CountAsync();
        }

        /// <summary>
        /// 获取会员组ID
        /// </summary>
        public async Task<int> QueryGroupIdAsync(int userId, WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            if (userId == 0) return 0;

            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库
            var model = await _context.Set<Members>().FirstOrDefaultAsync(x => x.UserId == userId);
            if (model != null)
            {
                return model.GroupId;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 统计会员注册数量
        /// </summary>
        public async Task<IEnumerable<MembersReportDto>> QueryCountListAsync(int top, Expression<Func<Members, bool>> funcWhere,
            WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库
            var result = _context.Set<Members>()
                .Where(funcWhere)
                .GroupBy(x => new { x.RegTime.Month, x.RegTime.Day })
                .Select(g => new MembersReportDto
                {
                    Title = $"{g.Key.Month.ToString()}月{g.Key.Day.ToString()}日",
                    Count = g.Count()
                });
            if (top > 0) result = result.Take(top);//等于0显示所有数据
            return await result.ToListAsync();
        }

        /// <summary>
        /// 添加会员
        /// </summary>
        public async Task<Members?> AddAsync(MembersEditDto modelDto)
        {
            //判断用户名邮箱手机是否为空
            if (string.IsNullOrWhiteSpace(modelDto.UserName)
                && string.IsNullOrWhiteSpace(modelDto.Email) && string.IsNullOrWhiteSpace(modelDto.Phone))
            {
                throw new ResponseException("用户名、邮箱、手机至少填写一项");
            }
            //判断密码是否为空
            if (string.IsNullOrWhiteSpace(modelDto.Password))
            {
                throw new ResponseException("请输入登录密码");
            }
            //如果用户名为空，则自动生成用户名
            if (string.IsNullOrWhiteSpace(modelDto.UserName))
            {
                modelDto.UserName = UtilConvert.GetGuidToString();
            }
            //判断用户名是否重复
            if (await ExistsAsync<ApplicationUser>(x => x.UserName == modelDto.UserName))
            {
                throw new ResponseException("用户名已重复，请更换");
            }
            //判断邮箱是否重复
            if (modelDto.Email.IsNotNullOrEmpty()
                && await ExistsAsync<ApplicationUser>(x => x.Email == modelDto.Email))
            {
                throw new ResponseException("邮箱地址已重复，请更换");
            }
            //判断手机号是否重复
            if (modelDto.Phone.IsNotNullOrEmpty()
                && await ExistsAsync<ApplicationUser>(x => x.PhoneNumber == modelDto.Phone))
            {
                throw new ResponseException("手机号码已重复，请更换");
            }
            //检查会员组是否存在
            if (modelDto.GroupId <= 0)
            {
                var group = await QueryAsync<MemberGroups>(x => x.IsDefault == 1)
                    ?? throw new ResponseException("没有找到默认会员组");
                modelDto.GroupId = group.Id;
            }
            //获取会员角色
            var role = await QueryAsync<ApplicationRole>(x => x.RoleType == (byte)RoleType.Member)
                ?? throw new ResponseException("会员角色不存在");

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
            //将用户会员信息关联
            user.Member = new Members()
            {
                SiteId = modelDto.SiteId,
                UserId = user.Id,
                ReferrerId = modelDto.ReferrerId,
                GroupId = modelDto.GroupId,
                Avatar = modelDto.Avatar,
                RealName = modelDto.RealName,
                Sex = modelDto.Sex,
                Birthday = modelDto.Birthday,
                RegIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress.ToString(),
                RegTime = DateTime.Now
            };
            //HASH密码，保存用户
            var result = await _userManager.CreateAsync(user, modelDto.Password);
            if (!result.Succeeded)
            {
                throw new ResponseException($"{result.Errors.FirstOrDefault()?.Description}");
            }

            //查询结果
            return await base.QueryAsync<Members>(x => x.UserId == user.Id, query => query.Include(x => x.User).Include(x => x.Group), WriteRoRead.Write);
        }

        /// <summary>
        /// 修改会员
        /// </summary>
        public async Task<bool> UpdateAsync(int userId, MembersEditDto modelDto)
        {
            //查找记录会员及用户信息
            var model = await QueryAsync<Members>(x => x.UserId == userId)
                ?? throw new ResponseException("会员不存在或已删除");
            //检查用户信息是否存在
            var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new ResponseException("会员不存在或已删除");
            //如果用户名发生改变，则检查重复
            if (!string.IsNullOrWhiteSpace(modelDto.UserName) && user.UserName != modelDto.UserName
                && await ExistsAsync<ApplicationUser>(x => x.Id != userId && x.UserName == modelDto.UserName))
            {
                throw new ResponseException("用户名已重复，请更换");
            }
            //如果邮箱发生改变，则检查重复
            if (!string.IsNullOrWhiteSpace(modelDto.Email) && user.Email != modelDto.Email
                && await ExistsAsync<ApplicationUser>(x => x.Id != userId && x.Email == modelDto.Email))
            {
                throw new ResponseException("邮箱地址已重复，请更换");
            }
            //如果手机号发生改变，则检查重复
            if (!string.IsNullOrWhiteSpace(modelDto.Phone) && user.PhoneNumber != modelDto.Phone
                && await ExistsAsync<ApplicationUser>(x => x.Id != userId && x.PhoneNumber == modelDto.Phone))
            {
                throw new ResponseException("手机号码已重复，请更换");
            }
            //推荐人不能选择自己
            if (modelDto.ReferrerId == userId)
            {
                throw new ResponseException("推荐人不能选择自己，请重试");
            }

            //用户信息
            if (modelDto.UserName.IsNotNullOrWhiteSpace())
            {
                user.UserName = modelDto.UserName;
            }
            user.Email = modelDto.Email;
            user.PhoneNumber = modelDto.Phone;
            user.Status = modelDto.Status;
            //会员信息
            user.Member = new Members()
            {
                Id = model.Id,
                UserId = model.UserId,
                Amount = model.Amount,
                Point = model.Point,
                Exp = model.Exp,
                RegIp = model.RegIp,
                RegTime = model.RegTime,
                ReferrerId = modelDto.ReferrerId,
                SiteId = modelDto.SiteId,
                GroupId = modelDto.GroupId,
                Avatar = modelDto.Avatar,
                RealName = modelDto.RealName,
                Sex = modelDto.Sex,
                Birthday = modelDto.Birthday
            };
            var result = await _userManager.UpdateAsync(user);
            //异常错误提示
            if (!result.Succeeded)
            {
                throw new ResponseException($"{result.Errors.FirstOrDefault()?.Description}");
            }
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
        /// 删除会员
        /// </summary>
        public async Task<bool> DeleteAsync(int userId)
        {
            //检查用户信息是否存在
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }
            //删除会员,角色,用户
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }
    }
}
