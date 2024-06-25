using AutoMapper;
using DTcms.Core.Common.Emums;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Common.Extensions;
using DTcms.Core.Common.Helpers;
using DTcms.Core.Model.ViewModels;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 文章类别接口实现
    /// </summary>
    public class ArticleCategoryService(IDbContextFactory contentFactory, ICacheService cacheService, IUserService userService, IMapper mapper)
        : BaseService(contentFactory, cacheService), IArticleCategoryService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IUserService _userService = userService;

        /// <summary>
        /// 根据父ID返回目录树
        /// </summary>
        public async Task<IEnumerable<ArticleCategorysDto>> QueryListAsync(int channelId, int parentId, WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库
            var listData = await _context.Set<ArticleCategorys>().Where(x => x.ChannelId.Equals(channelId)).ToListAsync();
            //调用递归重新生成目录树
            List<ArticleCategorysDto> result = await GetChilds(listData, parentId);
            return result;
        }

        /// <summary>
        /// 更新一条数据(重组父子关系)
        /// </summary>
        public async Task<bool> UpdateAsync(int id, ArticleCategorysEditDto modelDto)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write); //连接数据库
            //查找记录
            var model = await _context.Set<ArticleCategorys>().FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //检查频道是否存在
            var channelModel = await _context.Set<SiteChannels>().FirstOrDefaultAsync(x => x.Id == modelDto.ChannelId);
            if (channelModel == null)
            {
                throw new ResponseException($"频道[{modelDto.ChannelId}]不存在或已删除");
            }
            //需要更新的集合
            List<ArticleCategorys> updateList = new();
            //检查是否有子集
            var listData = await _context.Set<ArticleCategorys>().Where(x => x.ChannelId.Equals(model.ChannelId)).ToListAsync();
            //检查是否有子集
            List<ArticleCategorys> childList = GetChildList(listData, [], model.Id);
            //检查现在父级是否在原来的子集中
            var parentModel = childList.Find(t => t.Id.Equals(modelDto.ParentId));
            if (parentModel != null)
            {
                //交换父级元素
                parentModel.ParentId = model.ParentId;
                updateList.Add(parentModel);
            }
            //AutoMapper将DTO映射到源数据
            _mapper.Map(modelDto, model);
            //添加站点主键
            model.SiteId = channelModel.SiteId;
            //实体加入要修改的集合
            updateList.Add(model);
            //批量更新
            foreach (var t in updateList)
            {
                _context.Set<ArticleCategorys>().Attach(t);
                _context.Entry(t).State = EntityState.Modified;
            }
            var result = await this.SaveAsync();

            //删除缓存
            if (result)
            {
                await this.RemoveCacheAsync<ArticleCategorys>(true);
            }

            return result;
        }

        /// <summary>
        /// 根据条件删除数据(迭代删除)
        /// </summary>
        public async Task<bool> DeleteAsync(Expression<Func<ArticleCategorys, bool>> funcWhere)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库
            var listData = await _context.Set<ArticleCategorys>().ToListAsync();//查询所有数据
            var list = await _context.Set<ArticleCategorys>().Where(funcWhere).ToListAsync();
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
                await this.RemoveCacheAsync<ArticleCategorys>(true);
            }

            return result;
        }

        /// <summary>
        /// 根据父ID返回一级列表(缓存带文章)
        /// </summary>
        public async Task<IEnumerable<ArticleCategorysClientDto>> QueryArticleListAsync(string cacheKey, int channelId, int categoryTop, int articleTop, int status,
            Expression<Func<Articles, bool>> funcWhere, string orderBy)
        {
            string className = typeof(ArticleCategorys).Name; //获取类名的字符串
            string classKey = $"{className}:List:{cacheKey}";

            return await _cacheService.GetOrSetAsync(classKey, async () =>
            {
                _context = _contextFactory.CreateContext(WriteRoRead.Read);//连接数据库

                var query = _context.Set<ArticleCategorys>()
                    .Where(x => x.ChannelId == channelId && x.ParentId == 0 && (status == -1 || x.Status == status))
                    .OrderByBatch(orderBy)
                    .Take(categoryTop)
                    .Select(c => new
                    {
                        Category = c,
                        Article = _context.Set<Articles>()
                            .Include(x => x.ArticleGroups)
                            //.Include(x => x.ArticleAlbums)
                            .Include(x => x.CategoryRelations)
                            .Where(x => x.CategoryRelations.Any(r => r.CategoryId == c.Id))
                            .Where(funcWhere)
                            .OrderBy(x => x.SortId)
                            .Take(articleTop)
                            .ToList()
                    })
                    .Select(ca => new ArticleCategorysClientDto
                    {
                        Id = ca.Category.Id,
                        Title = ca.Category.Title,
                        ImgUrl = ca.Category.ImgUrl,
                        Data = ca.Article.Select(x => new ArticlesClientDto
                        {
                            Id = x.Id,
                            CallIndex = x.CallIndex,
                            Title = x.Title,
                            Source = x.Source,
                            Zhaiyao = x.Zhaiyao,
                            ImgUrl = x.ImgUrl,
                            VideoUrl = x.VideoUrl,
                            Click = x.Click,
                            CommentCount = x.CommentCount,
                            LikeCount = x.LikeCount,
                            SortId = x.SortId,
                            AddTime = x.AddTime,
                            /*ArticleAlbums = x.ArticleAlbums.Select(ab => new ArticleAlbumsDto
                            {
                                Id = ab.Id,
                                ArticleId = ab.ArticleId,
                                ThumbPath = ab.ThumbPath,
                                OriginalPath = ab.OriginalPath,
                                Remark = ab.Remark,
                                AddTime = ab.AddTime,
                                SortId = ab.SortId
                            })*/
                        })
                    });

                return await query.ToListAsync();
            }) ?? [];
        }

        /// <summary>
        /// 根据父ID返回一级列表(缓存)
        /// </summary>
        public async Task<IEnumerable<ArticleCategorys>> QueryListByCacheAsync(string cacheKey, int top, int channelId, int parentId)
        {
            string className = typeof(ArticleCategorys).Name; //获取类名的字符串
            string classKey = $"{className}:List:{cacheKey}";

            return await _cacheService.GetOrSetAsync(classKey, async () =>
            {
                _context = _contextFactory.CreateContext(WriteRoRead.Read); //连接数据库
                var result = _context.Set<ArticleCategorys>().Where(x => x.ParentId == parentId && x.ChannelId == channelId).OrderByBatch("SortId,Id");
                if (top > 0) result = result.Take(top); //等于0显示所有数据
                return await result.ToListAsync();
            }) ?? [];
        }

        /// <summary>
        /// 根据父ID返回目录树(缓存)
        /// </summary>
        public async Task<IEnumerable<ArticleCategorysDto>> QueryListByCacheAsync(string cacheKey, int channelId, int parentId)
        {
            string className = typeof(ArticleCategorys).Name; //获取类名的字符串
            string classKey = $"{className}:List:{cacheKey}";

            return await _cacheService.GetOrSetAsync(classKey, async () =>
            {
                _context = _contextFactory.CreateContext(WriteRoRead.Read); //连接数据库
                var listData = await _context.Set<ArticleCategorys>().Where(x => x.ChannelId.Equals(channelId)).ToListAsync();
                //调用递归重新生成目录树
                return await GetChilds(listData, parentId);
            }) ?? [];
        }

        #region 辅助私有方法
        /// <summary>
        /// 迭代循环删除
        /// </summary>
        private async Task DeleteChilds(IEnumerable<ArticleCategorys> listData, long parentId)
        {
            if (_context == null)
            {
                throw new ResponseException("请先连接到数据库");
            }
            IEnumerable<ArticleCategorys> models = listData.Where(x => x.ParentId == parentId);
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
        private List<ArticleCategorys> GetChildList(List<ArticleCategorys> listData, List<ArticleCategorys> list, int parentId)
        {
            IEnumerable<ArticleCategorys> models = listData.Where(x => x.ParentId == parentId).OrderByBatch("SortId");//查找并排序
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
        private async Task<List<ArticleCategorysDto>> GetChilds(IEnumerable<ArticleCategorys> listData, int parentId)
        {
            List<ArticleCategorysDto> listDto = new();
            IEnumerable<ArticleCategorys> models = listData.Where(x => x.ParentId == parentId).OrderByBatch("SortId");//查找并排序
            foreach (ArticleCategorys modelt in models)
            {
                ArticleCategorysDto modelDto = new()
                {
                    Id = modelt.Id,
                    ParentId = modelt.ParentId,
                    SiteId = modelt.SiteId,
                    CallIndex = modelt.CallIndex,
                    Title = modelt.Title,
                    ImgUrl = modelt.ImgUrl,
                    LinkUrl = modelt.LinkUrl,
                    Content = modelt.Content,
                    SortId = modelt.SortId,
                    SeoTitle = modelt.SeoTitle,
                    SeoKeyword = modelt.SeoKeyword,
                    SeoDescription = modelt.SeoDescription,
                    Status = modelt.Status,
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
