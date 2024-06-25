using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Extensions;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 导航菜单接口实现
    /// </summary>
    public class ManagerMenuService(IDbContextFactory contentFactory, ICacheService cacheService, IUserService userService)
        : BaseService(contentFactory, cacheService), IManagerMenuService
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// 通过站点ID查找导航菜单
        /// </summary>
        public async Task<ManagerMenus?> QueryBySiteIdAsync(int siteId, WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库

            //兼容MYSQL的写法
            var s = await _context.Set<Sites>().Where(x => x.Id == siteId).FirstOrDefaultAsync();
            if (s == null)
            {
                throw new ResponseException("无法查询到当前站点信息");
            }
            var n = await _context.Set<ManagerMenus>().Where(x => x.Name == "site_" + s.DirPath).FirstOrDefaultAsync();
            return n;
        }

        /// <summary>
        /// 返回所有菜单目录树
        /// </summary>
        public async Task<IEnumerable<ManagerMenusDto>> QueryListAsync(int parentId, WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库
            var oldList = await _context.Set<ManagerMenus>().ToListAsync();
            //调用递归重新生成目录树
            List<ManagerMenusDto> result = await GetChilds(oldList, parentId);
            return result;
        }

        /// <summary>
        /// 返回当前用户所有菜单目录树
        /// </summary>
        public async Task<IEnumerable<ManagerMenusClientDto>> QueryListAsync(WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库

            //检查用户是否超管
            bool isSuperAdmin = await _userService.IsSuperAdminAsync();
            List<Claim> claims = await _userService.GetRoleClaimsAsync();
            var oldList = await _context.Set<ManagerMenus>().ToListAsync();
            //调用递归重新生成目录树
            List<ManagerMenusClientDto> result = await GetChilds(oldList, claims, 0, isSuperAdmin);
            return result;
        }

        /// <summary>
        /// 返回指定角色所有菜单目录树
        /// </summary>
        public async Task<IEnumerable<ManagerMenuRolesDto>> QueryListByRoleIdAsync(int roleId, WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库
            //获取角色的Claim
            List<Claim> claims = new();
            if (roleId > 0)
            {
                claims = await _userService.GetRoleClaimsAsync(roleId);
            }
            var oldList = await _context.Set<ManagerMenus>().ToListAsync();
            //调用递归重新生成目录树
            List<ManagerMenuRolesDto> result = await GetChilds(oldList, claims, 0);
            return result;
        }

        /// <summary>
        /// 根据导航类别查询模型列表
        /// </summary>
        public async Task<IEnumerable<ManagerMenuModels>> QueryModelAsync(NavType navType, WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库
            var result = await _context.Set<ManagerMenuModels>()
                .Where(x => x.NavType == navType.ToString()).ToListAsync();
            return result;
        }

        /// <summary>
        /// 根据条件删除数据(迭代删除)
        /// </summary>
        public async Task<bool> DeleteAsync(Expression<Func<ManagerMenus, bool>> funcWhere)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);
            var listData = await _context.Set<ManagerMenus>().ToListAsync();//查询所有数据
            var list = await _context.Set<ManagerMenus>().Where(funcWhere).ToListAsync();
            if (list == null)
            {
                return false;
            }
            foreach (var modelt in list)
            {
                await DeleteChilds(listData, modelt.Id);//删除子节点
                _context.RemoveRange(modelt);//删除当前节点
            }
            return await this.SaveAsync();
        }

        #region 辅助私有方法
        /// <summary>
        /// 迭代循环删除
        /// </summary>
        private async Task DeleteChilds(IEnumerable<ManagerMenus> listData, int parentId)
        {
            if (_context == null)
            {
                throw new ResponseException("请先连接数据库");
            }
            IEnumerable<ManagerMenus> models = listData.Where(x => x.ParentId == parentId);
            foreach (var modelt in models)
            {
                await DeleteChilds(listData, modelt.Id);//迭代
                _context.RemoveRange(modelt);
            }
        }

        /// <summary>
        /// 迭代循环目录树(私有方法)
        /// </summary>
        private async Task<List<ManagerMenusDto>> GetChilds(IEnumerable<ManagerMenus> oldList, int parentId)
        {
            List<ManagerMenusDto> listDto = new List<ManagerMenusDto>();
            IEnumerable<ManagerMenus> models = oldList.Where(x => x.ParentId == parentId).OrderByBatch("SortId");//查找并排序
            foreach (var modelt in models)
            {
                ManagerMenusDto modelDto = new()
                {
                    Id = modelt.Id,
                    ParentId = modelt.ParentId,
                    ChannelId = modelt.ChannelId,
                    Name = modelt.Name,
                    Title = modelt.Title,
                    SubTitle = modelt.SubTitle,
                    IconUrl = modelt.IconUrl,
                    LinkUrl = modelt.LinkUrl,
                    SortId = modelt.SortId,
                    Status = modelt.Status,
                    IsSystem = modelt.IsSystem,
                    Remark = modelt.Remark,
                    Controller = modelt.Controller,
                    Resource = modelt.Resource
                };
                modelDto.Children.AddRange(
                    await GetChilds(oldList, modelt.Id)
                );
                listDto.Add(modelDto);
            }
            return listDto;
        }

        /// <summary>
        /// 判断权限迭代循环目录树(私有方法)
        /// </summary>
        private async Task<List<ManagerMenusClientDto>> GetChilds(IEnumerable<ManagerMenus> oldList, List<Claim> claimList, int parentId, bool isSuperAdmin)
        {
            List<ManagerMenusClientDto> listDto = [];
            IEnumerable<ManagerMenus> models = oldList.Where(x => x.ParentId == parentId).OrderByBatch("SortId");//查找并排序
            foreach (var modelt in models)
            {
                //如果不是超管且没权限或菜单隐藏则跳过
                if ((!isSuperAdmin && !claimList.Any(x => x.Type == modelt.Name && x.Value == $"{modelt.Controller}.Show")) || modelt.Status == 1)
                {
                    continue;
                }
                ManagerMenusClientDto modelDto = new()
                {
                    Id = modelt.Id,
                    ParentId = modelt.ParentId,
                    ChannelId = modelt.ChannelId,
                    Name = modelt.Name,
                    Text = modelt.Title,
                    Icon = modelt.IconUrl,
                    Href = modelt.LinkUrl
                };
                modelDto.Children.AddRange(
                    await GetChilds(oldList, claimList, modelt.Id, isSuperAdmin)
                );
                listDto.Add(modelDto);
            }
            return listDto;
        }

        /// <summary>
        /// 判断权限迭代所有目录树(私有方法)
        /// </summary>
        private async Task<List<ManagerMenuRolesDto>> GetChilds(IEnumerable<ManagerMenus> oldList, List<Claim> claimList, int parentId)
        {
            if (claimList == null) return [];

            List<ManagerMenuRolesDto> listDto = [];
            IEnumerable<ManagerMenus> models = oldList.Where(x => x.ParentId == parentId).OrderByBatch("SortId");//查找并排序
            foreach (var modelt in models)
            {
                if(modelt.Status == 1)
                {
                    continue;
                }
                ManagerMenuRolesDto modelDto = new()
                {
                    Id = modelt.Id,
                    Name = modelt.Name,
                    Title = modelt.Title,
                    Controller = modelt.Controller

                };
                //菜单的权限资源列表
                if (modelt.Resource != null)
                {
                    List<ManagerMenuResourcesDto> resourceList = [];
                    var resourceArr = modelt.Resource.Split(",");
                    foreach (var item in resourceArr)
                    {
                        bool isSelected = false;
                        //检查是否传入角色权限列表
                        if (claimList.Count > 0 && claimList.Any(x => x.Type == modelt.Name && x.Value == $"{modelt.Controller}.{item}"))
                        {
                            isSelected = true;
                        }
                        var actionType = item.ToEnum<ActionType>();//把字符串转换为枚举
                        resourceList.Add(new ManagerMenuResourcesDto()
                        {
                            Name = actionType.ToString(), //获取英文名
                            Title = actionType.DisplayName(), //获取中文名
                            IsSelected = isSelected //选中
                        });
                    }
                    //加入权限资源
                    modelDto.Resource.AddRange(resourceList);
                }
                //继续迭代查找
                modelDto.Children.AddRange(
                    await GetChilds(oldList, claimList, modelt.Id)
                );
                listDto.Add(modelDto);
            }
            return listDto;
        }
        #endregion
    }
}
