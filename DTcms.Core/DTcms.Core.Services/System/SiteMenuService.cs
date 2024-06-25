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
    /// 站点菜单接口实现
    /// </summary>
    public class SiteMenuService(IDbContextFactory contentFactory, ICacheService cacheService) : BaseService(contentFactory, cacheService), ISiteMenuService
    {
        /// <summary>
        /// 根据父ID返回目录树
        /// </summary>
        public async Task<IEnumerable<SiteMenusDto>> QueryListAsync(int parentId, WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库
            var listData = await _context.Set<SiteMenus>().ToListAsync();
            //调用递归重新生成目录树
            List<SiteMenusDto> result = await GetChilds(listData, parentId);
            return result;
        }

        /// <summary>
        /// 根据条件删除数据(迭代删除)
        /// </summary>
        public async Task<bool> DeleteAsync(Expression<Func<SiteMenus, bool>> funcWhere)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库
            var listData = await _context.Set<SiteMenus>().ToListAsync();//查询所有数据
            var list = await _context.Set<SiteMenus>().Where(funcWhere).ToListAsync();
            if (list == null)
            {
                return false;
            }
            foreach (var modelt in list)
            {
                await DeleteChilds(listData, modelt.Id);//删除子节点
                _context.RemoveRange(modelt);//删除当前节点
            }

            var result = await this.SaveAsync();
            //删除缓存
            if (result)
            {
                await this.RemoveCacheAsync<SiteMenus>(true);
            }
            return result;
        }

        /// <summary>
        /// 从缓存中返回所有地区目录树
        /// </summary>
        public async Task<IEnumerable<SiteMenusDto>> QueryListAsync(string cacheKey, int parentId)
        {
            string className = typeof(SiteMenus).Name; //获取类名的字符串
            string classKey = $"{className}:List:{cacheKey}";

            return await _cacheService.GetOrSetAsync(classKey, async () =>
            {
                _context = _contextFactory.CreateContext(WriteRoRead.Read);//连接数据库
                var listData = await _context.Set<SiteMenus>().ToListAsync();
                //调用递归重新生成目录树
                return await GetChilds(listData, parentId);
            }) ?? [];
        }

        #region 辅助私有方法
        /// <summary>
        /// 迭代循环删除
        /// </summary>
        private async Task DeleteChilds(IEnumerable<SiteMenus> listData, int parentId)
        {
            if (_context == null)
            {
                throw new ResponseException("请先连接到数据库");
            }

            IEnumerable<SiteMenus> models = listData.Where(x => x.ParentId == parentId);
            foreach (var modelt in models)
            {
                await DeleteChilds(listData, modelt.Id);//迭代
                _context.RemoveRange(modelt);
            }
        }

        /// <summary>
        /// 递归返回子级数，不是目录树
        /// </summary>
        /// <param name="data">主数据</param>
        /// <param name="parentId">父级</param>
        private List<SiteMenus> GetChildList(List<SiteMenus> listData, List<SiteMenus> list, int parentId)
        {
            IEnumerable<SiteMenus> models = listData.Where(x => x.ParentId == parentId).OrderByBatch("SortId");//查找并排序
            if (models != null)
            {
                foreach (var m in models)
                {
                    list.Add(m);
                    GetChildList(listData, list, m.Id);
                }
            }
            return list;
        }

        /// <summary>
        /// 迭代循环返回目录树(私有方法)
        /// </summary>
        private async Task<List<SiteMenusDto>> GetChilds(IEnumerable<SiteMenus> listData, int parentId)
        {
            List<SiteMenusDto> listDto = [];
            IEnumerable<SiteMenus> models = listData.Where(x => x.ParentId == parentId).OrderByBatch("SortId");//查找并排序
            foreach (SiteMenus modelt in models)
            {
                SiteMenusDto modelDto = new()
                {
                    Id = modelt.Id,
                    ParentId = modelt.ParentId,
                    Title = modelt.Title,
                    SubTitle = modelt.SubTitle,
                    IconUrl=modelt.IconUrl,
                    LinkUrl = modelt.LinkUrl,
                    SortId = modelt.SortId,
                    Status = modelt.Status,
                    Remark = modelt.Remark,
                    AddBy = modelt.AddBy,
                    AddTime = modelt.AddTime,
                };
                modelDto.Children.AddRange(
                    await GetChilds(listData, modelt.Id)
                );
                listDto.Add(modelDto);
            }
            return listDto;
        }
        #endregion
    }
}
