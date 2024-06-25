using AutoMapper;
using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Extensions;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 管理员角色接口实现
    /// </summary>
    public class ManagerRoleService(IDbContextFactory contentFactory, ICacheService cacheService,
        UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IManagerMenuService navigationService, IMapper mapper)
        : BaseService(contentFactory, cacheService), IManagerRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly IManagerMenuService _navigationService = navigationService;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// 查询一条记录
        /// </summary>
        public async Task<ManagerRolesDto> QueryAsync(Expression<Func<ApplicationRole, bool>> funcWhere, WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库

            //获取角色信息
            var role = await _context.Set<ApplicationRole>().FirstOrDefaultAsync(funcWhere);
            if (role == null)
            {
                throw new ResponseException("管理角色不存在或已删除");
            }
            //获取导航菜单列表
            var navList = await _navigationService.QueryListByRoleIdAsync(role.Id);
            //映射成实体
            var model = _mapper.Map<ManagerRolesDto>(role);
            model.Navigation = navList.ToList();
            return model;
        }

        /// <summary>
        /// 查询指定数量列表
        /// </summary>
        public async Task<IEnumerable<ApplicationRole>> QueryListAsync(int top, Expression<Func<ApplicationRole, bool>> funcWhere,
            string orderBy, WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库
            var result = _context.Set<ApplicationRole>().Where(funcWhere).OrderByBatch(orderBy);//调用Linq扩展类进行排序
            if (top > 0) result = result.Take(top);//等于0显示所有数据
            return await result.ToListAsync();
        }

        /// <summary>
        /// 查询分页列表
        /// </summary>
        public async Task<PaginationList<ApplicationRole>> QueryPageAsync(int pageSize, int pageIndex, Expression<Func<ApplicationRole, bool>> funcWhere,
            string orderBy, WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库
            var result = _context.Set<ApplicationRole>().Where(funcWhere).OrderByBatch(orderBy); //调用Linq扩展类排序
            return await PaginationList<ApplicationRole>.CreateAsync(pageIndex, pageSize, result);
        }

        /// <summary>
        /// 添加一条记录
        /// </summary>
        public async Task<int> AddAsync(ManagerRolesEditDto modelDto)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库

            if (modelDto.Name == null)
            {
                throw new ResponseException("角色标识不能为空");
            }
            //检查名称是否重复
            if (await _context.Set<ApplicationRole>().FirstOrDefaultAsync(x => x.Name != null && x.Name.ToLower() == modelDto.Name.ToLower()) != null)
            {
                throw new ResponseException("角色名称已重复，请更正");
            }
            //查找出角色的Claim
            List<ApplicationRoleClaim> roleClaims = await GetRoleClaim(modelDto.Navigation);
            //映射成实体
            var model = _mapper.Map<ApplicationRole>(modelDto);
            //角色的Claim赋值
            model.RoleClaims = roleClaims;
            //保存角色
            var result = await _roleManager.CreateAsync(model);
            if (!result.Succeeded)
            {
                throw new ResponseException($"保存失败，错误代码:{result.Errors.FirstOrDefault()?.Code}");
            }
            return model.Id;
        }

        /// <summary>
        /// 修改一条记录
        /// </summary>
        public async Task<bool> UpdateAsync(int id, ManagerRolesEditDto modelDto)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库

            if (modelDto.Name == null)
            {
                throw new ResponseException("角色标识不能为空");
            }
            //根据ID获取记录,非踪查询
            var model = await _context.Set<ApplicationRole>().Include(x => x.RoleClaims).FirstOrDefaultAsync(x => x.Id == id);
            //如果不存在则抛出异常
            if (model == null)
            {
                throw new ResponseException("数据不存在或已删除");
            }
            //检查名称是否重复
            if (await _context.Set<ApplicationRole>().FirstOrDefaultAsync(x => x.Name != null && x.Id != id && x.Name.ToLower() == modelDto.Name.ToLower()) != null)
            {
                throw new ResponseException("角色名称已重复，请更正");
            }
            //查找出角色的Claim
            List<ApplicationRoleClaim> roleClaims = await GetRoleClaim(modelDto.Navigation);
            //将DTO映射到源数据
            _mapper.Map(modelDto, model);
            //规范化角色验证
            await _roleManager.UpdateNormalizedRoleNameAsync(model);
            //角色的Claim赋值
            model.RoleClaims = roleClaims;
            //保存角色
            _context.Set<ApplicationRole>().Update(model);
            return await this.SaveAsync();
        }

        /// <summary>
        /// 根据条件删除记录
        /// </summary>
        public async Task<bool> DeleteAsync(Expression<Func<ApplicationRole, bool>> funcWhere)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库

            var list = await _context.Set<ApplicationRole>()
                .Include(x => x.RoleClaims).Where(funcWhere).ToListAsync();
            if (list == null)
            {
                return false;
            }
            //系统默认角色不允许删除
            if (list.FirstOrDefault(x => x.IsSystem == 1) != null)
            {
                throw new ResponseException("系统默认的角色，无法删除");
            }
            //检查该角色下是否有用户，有则报错
            var userRole = await _context.Set<ApplicationUserRole>().FirstOrDefaultAsync(x => list.Select(x => x.Id).Contains(x.RoleId));
            if (userRole != null)
            {
                throw new ResponseException("所选的角色里尚有用户，无法删除");
            }
            foreach (var modelt in list)
            {
                //加入追踪列表
                _context.Set<ApplicationRole>().Attach(modelt);
            }
            _context.Set<ApplicationRole>().RemoveRange(list);
            return await this.SaveAsync();
        }

        #region 辅助私有方法
        /// <summary>
        /// 查找角色Claim列表
        /// </summary>
        private async Task<List<ApplicationRoleClaim>> GetRoleClaim(IEnumerable<ManagerMenuRolesDto> navList)
        {
            List<ApplicationRoleClaim> roleClaims = new List<ApplicationRoleClaim>();
            foreach (var modelt in navList)
            {
                foreach (var resourceModelt in modelt.Resource)
                {
                    if (resourceModelt.IsSelected)
                    {
                        roleClaims.Add(new ApplicationRoleClaim()
                        {
                            ClaimType = modelt.Name,
                            ClaimValue = $"{modelt.Controller}.{resourceModelt.Name}"
                        });
                    }
                }
                //如果还有子导航则继续查找
                if (modelt.Children != null && modelt.Children.Count > 0)
                {
                    roleClaims.AddRange(await GetRoleClaim(modelt.Children));
                }
            }
            return roleClaims;
        }
        #endregion
    }
}