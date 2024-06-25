using AutoMapper;
using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Extensions;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 文章接口实现
    /// </summary>
    public class ArticleService(IDbContextFactory contentFactory, ICacheService cacheService, IMapper mapper)
        : BaseService(contentFactory, cacheService), IArticleService
    {
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// 根据ID返回上一条下一条(缓存)
        /// </summary>
        public async Task<IEnumerable<Articles>> QueryNextByCacheAsync(string cacheKey, long id, Expression<Func<Articles, bool>> funcWhere)
        {
            string className = typeof(Articles).Name; //获取类名的字符串
            string classKey = $"{className}:List:{cacheKey}";

            return await _cacheService.GetOrSetAsync(classKey, async () =>
            {
                _context = _contextFactory.CreateContext(WriteRoRead.Read); //连接数据库
                var result = new List<Articles>(); //准备一个空的列表

                //查找当前的文章
                var model = await _context.Set<Articles>().FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new ResponseException("记录不存在或已删除");

                //查找上一条文章
                var preModel = await _context.Set<Articles>()
                    .Include(x=>x.ArticleGroups)
                    .Where(x => x.SiteId == model.SiteId && x.ChannelId == model.ChannelId && x.Id < model.Id)
                    .Where(funcWhere)
                    .OrderByDescending(x => x.SortId)
                    .ThenByDescending(x => x.Id)
                    .FirstOrDefaultAsync();
                if (preModel != null)
                {
                    result.Add(preModel);
                }
                //查找下一条文章
                var nextModel = await _context.Set<Articles>()
                    .Include(x => x.ArticleGroups)
                    .Where(x => x.SiteId == model.SiteId && x.ChannelId == model.ChannelId && x.Id > model.Id)
                    .Where(funcWhere)
                    .OrderBy(x => x.SortId)
                    .ThenBy(x => x.Id)
                    .FirstOrDefaultAsync();
                if (nextModel != null)
                {
                    result.Add(nextModel);
                }

                return result;
            }) ?? [];
        }

        /// <summary>
        /// 更新浏览量(局部更新)
        /// </summary>
        public async Task<int> UpdateClickAsync(string? cacheKey, long articleId, int clickCount)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库
            //创建实体
            Articles model = new()
            {
                Id = articleId,
                Click = clickCount
            };
            var entry = _context.Entry<Articles>(model);
            //设置修改状态
            entry.State = EntityState.Unchanged;
            entry.Property(p => p.Click).IsModified = true;
            //提交保存
            var result = await this.SaveAsync();

            //删除缓存
            if (result)
            {
                if (cacheKey != null)
                {
                    await _cacheService.RemoveAsync($"{typeof(Articles).Name}:Show:{cacheKey}"); //删除缓存
                }
                await _cacheService.RemoveAsync($"{typeof(Articles).Name}:Show:{articleId.ToString()}"); //删除缓存
                return model.Click;
            }
            return 0;
        }

        /// <summary>
        /// 根据条件删除一条记录
        /// </summary>
        public async Task<bool> DeleteAsync(Expression<Func<Articles, bool>> funcWhere)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库
            //删除文章主从表
            var list = await _context.Set<Articles>()
                .Include(x => x.ArticleFields)
                .Include(x => x.CategoryRelations)
                .Include(x => x.LabelRelations)
                .Include(x => x.ArticleAlbums)
                .Include(x => x.ArticleAttachs)
                .Include(x => x.ArticleComments).ThenInclude(x => x.CommentLikes)
                .Where(funcWhere).ToListAsync();
            if (list == null) return false;
            _context.Set<Articles>().RemoveRange(list);
            //提交执行删除
            var result = await this.SaveAsync();

            //删除缓存
            if (result)
            {
                await this.RemoveCacheAsync<Articles>(true);
            }

            return result;
        }
    }
}
