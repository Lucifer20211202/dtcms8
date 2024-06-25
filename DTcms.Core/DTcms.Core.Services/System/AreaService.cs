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
    /// 全国省市区接口实现
    /// </summary>
    public class AreaService(IDbContextFactory contentFactory, ICacheService cacheService) : BaseService(contentFactory, cacheService), IAreaService
    {
        /// <summary>
        /// 从缓存中返回所有地区目录树
        /// </summary>
        public async Task<IEnumerable<AreasDto>> QueryListAsync(string cacheKey, int parentId)
        {
            string className = typeof(Areas).Name; //获取类名的字符串
            string classKey = $"{className}:List:{cacheKey}";

            return await _cacheService.GetOrSetAsync(classKey, async () =>
            {
                _context = _contextFactory.CreateContext(WriteRoRead.Read);//连接数据库
                var listData = await _context.Set<Areas>().ToListAsync();
                //调用递归重新生成目录树
                return await GetChilds(listData, parentId);
            }) ?? [];
        }

        /// <summary>
        /// 返回所有地区目录树
        /// </summary>
        public async Task<IEnumerable<AreasDto>> QueryListAsync(int parentId, WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库
            var listData = await _context.Set<Areas>().ToListAsync();
            //调用递归重新生成目录树
            List <AreasDto> result = await GetChilds(listData, parentId);
            return result;
        }

        /// <summary>
        /// 根据条件删除数据(迭代删除)
        /// </summary>
        public async Task<bool> DeleteAsync(Expression<Func<Areas, bool>> funcWhere)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库
            var listData = await _context.Set<Areas>().ToListAsync();//查询所有数据
            var list = await _context.Set<Areas>().Where(funcWhere).ToListAsync();
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
                await this.RemoveCacheAsync<Areas>();
            }
            return result;
        }

        /// <summary>
        /// 批量导入数据
        /// </summary>
        public async Task<bool> ImportAsync(List<AreasImportDto> listDto)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库
            await this.ImportChilds(listDto, 0);
            //删除缓存
            await this.RemoveCacheAsync<Areas>();
            return true;
        }

        #region 辅助私有方法
        /// <summary>
        /// 迭代循环删除
        /// </summary>
        private async Task DeleteChilds(IEnumerable<Areas> listData, int parentId)
        {
            if (_context == null)
            {
                throw new ResponseException("请先连接到数据库");
            }
            IEnumerable<Areas> models = listData.Where(x => x.ParentId == parentId);
            foreach (var modelt in models)
            {
                await DeleteChilds(listData, modelt.Id);//迭代
                _context.RemoveRange(modelt);
            }
        }

        /// <summary>
        /// 迭代循环返回目录树(私有方法)
        /// </summary>
        private async Task<List<AreasDto>> GetChilds(IEnumerable<Areas> listData, int parentId)
        {
            List<AreasDto> listDto = new();
            IEnumerable<Areas> models = listData.Where(x => x.ParentId == parentId).OrderByBatch("SortId");//查找并排序
            foreach (Areas modelt in models)
            {
                AreasDto modelDto = new()
                {
                    Id = modelt.Id,
                    ParentId = modelt.ParentId,
                    Title = modelt.Title,
                    SortId = modelt.SortId
                };
                modelDto.Children.AddRange(
                    await GetChilds(listData, modelt.Id)
                );
                listDto.Add(modelDto);
            }
            return listDto;
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        private async Task ImportChilds(List<AreasImportDto> listData, int parentId)
        {
            if (_context == null)
            {
                throw new ResponseException("请先连接到数据库");
            }
            foreach (AreasImportDto modelDto in listData)
            {
                Areas model = new()
                {
                    ParentId = parentId,
                    Title = modelDto.Name
                };
                await _context.Set<Areas>().AddAsync(model);
                await this.SaveAsync();
                if (modelDto.Children != null && modelDto.Children.Count > 0)
                {
                    await this.ImportChilds(modelDto.Children, model.Id);
                }
            }
        }
        #endregion
    }
}
