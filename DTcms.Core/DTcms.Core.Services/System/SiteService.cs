using AutoMapper;
using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 站点接口实现
    /// </summary>
    public class SiteService(IDbContextFactory contentFactory, ICacheService cacheService,
        IUserService userService, IMapper mapper) : BaseService(contentFactory, cacheService), ISiteService
    {
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// 添加站点(含菜单创建)
        /// </summary>
        public async Task<Sites> AddAsync(SitesEditDto modelDto)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库

            if (modelDto.Name == null)
            {
                throw new ResponseException($"站名英文名称不能为空");
            }
            //检查站点名称是否重复
            if (await _context.Set<Sites>().FirstOrDefaultAsync(x => x.Name != null
                && x.Name.ToLower() == modelDto.Name.ToLower()) != null)
            {
                throw new ResponseException($"站点名称[{modelDto.Name}]已存在", ErrorCode.RepeatField);
            }

            //映射成实体
            var model = _mapper.Map<Sites>(modelDto);
            model.AddBy = _userService.GetUserName();
            model.AddTime = DateTime.Now;

            //开启事务
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    //添加站点
                    await _context.Set<Sites>().AddAsync(model);
                    await this.SaveAsync();
                    //添加菜单
                    ManagerMenus navModel = new()
                    {
                        ParentId = 1,
                        ChannelId = 0,
                        Name = "site_" + model.DirPath,
                        Title = model.Title,
                        IsSystem = 1,
                        Controller = "Site",
                        Resource = "Show",
                        AddBy = model.AddBy,
                        AddTime = DateTime.Now
                    };
                    await _context.Set<ManagerMenus>().AddAsync(navModel);
                    await this.SaveAsync();
                    //提交事务
                    await transaction.CommitAsync();
                }
                catch(Exception ex)
                {
                    //回滚事务
                    await transaction.RollbackAsync();
                    throw new ResponseException($"保存时发生错误：{ex.Message}");
                }
            }

            //删除缓存(列表)
            await this.RemoveCacheAsync<Sites>();

            return model;
        }

        /// <summary>
        /// 修改站点(含菜单修改)
        /// </summary>
        public async Task<bool> UpdateAsync(int id, SitesEditDto modelDto)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库

            if (modelDto.Name == null)
            {
                throw new ResponseException($"站名英文名称不能为空");
            }
            //检查数据是否存在
            var model = await _context.Set<Sites>().Include(x => x.Domains).FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new ResponseException($"站点不存在或已删除");

            //检查站点名称是否变更
            if (!modelDto.Name.Equals(model.Name) && await _context.Set<Sites>().FirstOrDefaultAsync(x => x.Name != null
                && x.Name.Equals(modelDto.Name, StringComparison.CurrentCultureIgnoreCase)) != null)
            {
                throw new ResponseException($"站点名称[{modelDto.Name}]已存在", ErrorCode.RepeatField);
            }

            //对比模板目录是否发生改变，是则更改导航菜单名称
            if (modelDto.DirPath != model.DirPath || modelDto.Title != model.Title)
            {
                var navModel = await _context.Set<ManagerMenus>().FirstOrDefaultAsync(x => x.Name != null && x.Name == $"site_{model.DirPath}");
                if(navModel != null)
                {
                    navModel.Name = "site_" + modelDto.DirPath;
                    navModel.Title = modelDto.Title;
                    _context.Set<ManagerMenus>().Update(navModel);
                }
            }
            //将DTO映射到源数据,修改站点信息
            _mapper.Map(modelDto, model);
            _context.Set<Sites>().Update(model);
            var result = await this.SaveAsync();
            if (result)
            {
                await this.RemoveCacheAsync<Sites>(true);
            }
            return result;
        }

        /// <summary>
        /// 删除站点(含菜单删除)
        /// </summary>
        public async Task<bool> DeleteAsync(Expression<Func<Sites, bool>> funcWhere)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库
            var list = await _context.Set<Sites>().Include(x => x.Domains).Where(funcWhere).ToListAsync();
            if (list == null) return false;
            //检查站点下是否有频道
            var ids = list.Select(x => x.Id).ToList();
            var channel = await _context.Set<SiteChannels>().FirstOrDefaultAsync(x => ids.Contains(x.SiteId));
            if (channel != null)
            {
                throw new ResponseException($"所选站点还有频道，无法删除。");
            }
            foreach (var modelt in list)
            {
                //删除菜单
                var navModel = await _context.Set<ManagerMenus>().FirstOrDefaultAsync(x => !string.IsNullOrWhiteSpace(x.Name) && x.Name.Equals($"site_{modelt.DirPath}"));
                if (navModel != null)
                {
                    _context.Set<ManagerMenus>().Remove(navModel);
                }
                //加入追踪列表
                _context.Set<Sites>().Attach(modelt);
            }
            //删除站点
            _context.Set<Sites>().RemoveRange(list);
            //一次保存到数据库
            var result = await this.SaveAsync();
            if (result)
            {
                await this.RemoveCacheAsync<Sites>(true);
            }
            return result;
        }
    }
}