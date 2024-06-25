using AutoMapper;
using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Extensions;
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
    /// 站点频道接口实现
    /// </summary>
    public class SiteChannelService(IDbContextFactory contentFactory, ICacheService cacheService,
        IManagerMenuService navigationService, IUserService userService, IMapper mapper)
        : BaseService(contentFactory, cacheService), ISiteChannelService
    {
        private readonly IManagerMenuService _navigationService = navigationService;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// 添加频道(含菜单导航)
        /// </summary>
        public async Task<SiteChannels> AddAsync(SiteChannelsEditDto modelDto)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库

            if (modelDto.Name == null)
            {
                throw new ResponseException($"频道英文名称不能为空");
            }
            //检查频道名称是否重复(同一点站点频道名不能重复)
            if (await _context.Set<SiteChannels>().FirstOrDefaultAsync(
                x => x.SiteId == modelDto.SiteId
                && x.Name != null
                && x.Name.ToLower().Equals(modelDto.Name.ToLower())) != null)
            {
                throw new ResponseException($"频道名称[{modelDto.Name}]已重复", ErrorCode.RepeatField);
            }
            //检查站点信息是否正确
            if (await _context.Set<Sites>().FirstOrDefaultAsync(x => x.Id == modelDto.SiteId) == null)
            {
                throw new ResponseException($"站点不存在或已删除");
            }
            //联合查询站点菜单
            var navModel = await _navigationService.QueryBySiteIdAsync(modelDto.SiteId)
                ?? throw new ResponseException("站点菜单不存在或已删除");
            //查找菜单模型列表
            var modelList = await _navigationService.QueryModelAsync(NavType.Channel);
            if (modelList == null)
            {
                throw new ResponseException("频道菜单模型数据不存在");
            }
            //映射成实体
            var model = _mapper.Map<SiteChannels>(modelDto);
            model.AddBy = _userService.GetUserName();
            model.AddTime = DateTime.Now;

            //开启事务
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    //1.添加频道
                    await _context.Set<SiteChannels>().AddAsync(model);
                    await this.SaveAsync();
                    //2.查找菜单模型，添加相应的导航
                    await AddNavigation(modelList, model, navModel.Name ?? string.Empty, navModel.Id, 0);
                    //提交事务
                    await transaction.CommitAsync();
                }
                catch
                {
                    //回滚事务
                    await transaction.RollbackAsync();
                    throw new ResponseException("保存频道时发生意外错误");
                }
            }

            //删除频道和菜单缓存
            await this.RemoveCacheAsync<SiteChannels>();
            await this.RemoveCacheAsync<ManagerMenus>();

            return model;
        }

        /// <summary>
        /// 修改一条记录
        /// </summary>
        public async Task<bool> UpdateAsync(int id, SiteChannelsEditDto modelDto)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库

            //检查数据是否存在
            var model = await _context.Set<SiteChannels>().Include(x => x.Fields).FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new ResponseException($"数据不存在或已删除");

            //频道名称不可更改
            if (model.Name != modelDto.Name)
            {
                throw new ResponseException($"频道名称不可更改", ErrorCode.ParamError);
            }
            //所属站点不可更改
            if (model.SiteId != modelDto.SiteId)
            {
                throw new ResponseException($"所属站点不可更改", ErrorCode.ParamError);
            }
            //联合查询站点菜单
            var siteNavModel = await _navigationService.QueryBySiteIdAsync(modelDto.SiteId)
                ?? throw new ResponseException("站点菜单不存在或已删除");

            //修改菜单
            var navModel = await _context.Set<ManagerMenus>()
                .FirstOrDefaultAsync(x => x.ParentId == siteNavModel.Id && x.ChannelId == model.Id);
            if (navModel != null)
            {
                navModel.Title = modelDto.Title;
                _context.Set<ManagerMenus>().Update(navModel);
            }
            //将DTO映射到源数据,修改站点信息
            _mapper.Map(modelDto, model);
            _context.Set<SiteChannels>().Update(model);
            //保存到数据库
            var result = await this.SaveAsync();

            //删除频道和菜单缓存
            if (result)
            {
                await this.RemoveCacheAsync<SiteChannels>(true);
                await this.RemoveCacheAsync<ManagerMenus>(true);
            }

            return result;
        }

        /// <summary>
        /// 删除频道(含扩展字段及菜单)
        /// </summary>
        public async Task<bool> DeleteAsync(Expression<Func<SiteChannels, bool>> funcWhere)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库

            var list = await _context.Set<SiteChannels>()
                .Include(x => x.Fields).Where(funcWhere).ToListAsync();
            if (list == null)
            {
                return false;
            }

            foreach (var modelt in list)
            {
                //删除菜单
                var navModel = await _context.Set<ManagerMenus>().FirstOrDefaultAsync(x => x.ChannelId == modelt.Id);
                if (navModel != null)
                {
                    _context.Set<ManagerMenus>().Remove(navModel);
                }
                //加入追踪列表
                _context.Set<SiteChannels>().Attach(modelt);
            }
            //删除频道
            _context.Set<SiteChannels>().RemoveRange(list);
            //一次保存到数据库
            var result = await this.SaveAsync();

            //删除频道和菜单缓存
            if (result)
            {
                await this.RemoveCacheAsync<SiteChannels>(true);
                await this.RemoveCacheAsync<ManagerMenus>(true);
            }

            return result;
        }

        /// <summary>
        /// 迭代添加导航菜单(私有方法)
        /// </summary>
        /// <param name="modelData">菜单模型列表</param>
        /// <param name="channelModel">当前频道</param>
        /// <param name="navParentName">导航菜单父名称</param>
        /// <param name="navParentId">导航菜单父ID</param>
        /// <param name="modelParentId">菜单模型父ID</param>
        private async Task AddNavigation(IEnumerable<ManagerMenuModels> modelData, SiteChannels channelModel,
            string navParentName, int navParentId, int modelParentId)
        {
            if (_context == null)
            {
                throw new ResponseException($"请选连接数据库");
            }
            ManagerMenus navModel;//创建导航菜单
            //查找并排序
            IEnumerable<ManagerMenuModels> models = modelData.Where(x => x.ParentId == modelParentId).OrderByBatch("SortId,Id");
            foreach (var modelt in models)
            {
                //实例化菜单
                navModel = new ManagerMenus
                {
                    ParentId = navParentId,
                    ChannelId = channelModel.Id,
                    Name = $"{navParentName}_{channelModel.Name}_{modelt.Name}",
                    Title = modelParentId == 0 ? channelModel.Title : modelt.Title,
                    SubTitle = modelt.SubTitle,
                    IconUrl = modelt.IconUrl,
                    LinkUrl = modelt.LinkUrl,
                    SortId = modelt.SortId,
                    IsSystem = 1,
                    Controller = $"{modelt.Controller}@{channelModel.Id}",
                    Resource = modelt.Resource,
                    AddBy = channelModel.AddBy
                };
                //保存导航菜单
                await _context.Set<ManagerMenus>().AddAsync(navModel);
                await this.SaveAsync();
                //迭代循环查找并添加，直到结束
                await AddNavigation(modelData, channelModel, navParentName, navModel.Id, modelt.Id);
            }
        }
    }
}
