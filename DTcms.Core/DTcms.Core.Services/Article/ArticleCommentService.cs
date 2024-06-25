using AutoMapper;
using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Common.Extensions;
using DTcms.Core.Model.ViewModels;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 文章评论接口实现
    /// </summary>
    public class ArticleCommentService(IDbContextFactory contentFactory, ICacheService cacheService, IMapper mapper,
        IUserService userService, IHttpContextAccessor httpContextAccessor) : BaseService(contentFactory, cacheService), IArticleCommentService
    {
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        /// <summary>
        /// 查询前几条数据
        /// </summary>
        public async Task<IEnumerable<ArticleCommentsDto>> QueryListAsync(int top, Expression<Func<ArticleComments, bool>> funcWhere, string orderBy,
            WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库

            //获取第一级的分页数据
            var result = _context.Set<ArticleComments>().Include(x => x.User).ThenInclude(x => x!.Member)
                .Where(x => x.RootId == 0)
                .Where(funcWhere).OrderByBatch(orderBy);//调用Linq扩展类排序
            if (top > 0) result = result.Take(top);
            var rootList = await result.ToListAsync();

            //所有主键
            List<int> ids = rootList.Select(t => t.Id).ToList();
            var listDto = _mapper.Map<IEnumerable<ArticleCommentsDto>>(rootList);
            //查询子集数据
            var childrenList = await _context.Set<ArticleComments>().Where(x => ids.Contains(x.RootId)).Where(funcWhere).ToListAsync();
            //是否有子集
            if (childrenList != null)
            {
                foreach (var item in listDto)
                {
                    var childList = childrenList.Where(t => t.RootId == item.Id).ToList();
                    item.Children = _mapper.Map<IEnumerable<ArticleCommentsDto>>(childList).ToList();
                }
            }
            return listDto;
        }

        /// <summary>
        /// 查询分页列表
        /// </summary>
        public async Task<PaginationList<ArticleCommentsDto>> QueryPageAsync(int pageSize, int pageIndex, Expression<Func<ArticleComments, bool>> funcWhere,
            string orderBy, WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead); //连接数据库

            //获取第一级的分页数据
            var result = _context.Set<ArticleComments>().Include(x => x.User).ThenInclude(x => x!.Member)
                .Where(x => x.RootId == 0)
                .Where(funcWhere).OrderByBatch(orderBy);//调用Linq扩展类排序
            var rootList = await PaginationList<ArticleComments>.CreateAsync(pageIndex, pageSize, result);

            //获取所有主键
            List<int> ids = rootList.Items.Select(t => t.Id).ToList();
            //查询子集数据
            var childrenList = await _context.Set<ArticleComments>().Where(x => ids.Contains(x.RootId)).Where(funcWhere).OrderByBatch(orderBy).ToListAsync();
            //将分页数据映射成DTO
            var listDto = _mapper.Map<IEnumerable<ArticleCommentsDto>>(rootList.Items.ToList());
            foreach (var item in listDto)
            {
                var childList = childrenList.Where(t => t.RootId == item.Id).ToList();
                item.Children = _mapper.Map<IEnumerable<ArticleCommentsDto>>(childList).ToList();
            }

            return PaginationList<ArticleCommentsDto>.Create(pageIndex, pageSize, rootList.TotalCount, listDto.ToList());
        }

        /// <summary>
        /// 查询前几条数据(缓存)
        /// </summary>
        public async Task<IEnumerable<ArticleCommentsDto>> QueryListByCacheAsync(string cacheKey, int top, Expression<Func<ArticleComments, bool>> funcWhere, string orderBy,
            WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            string className = typeof(ArticleComments).Name; //获取类名的字符串
            string classKey = $"{className}:List:{cacheKey}";

            return await _cacheService.GetOrSetAsync(classKey, async () =>
            {
                _context = _contextFactory.CreateContext(writeAndRead);//连接数据库

                //获取第一级的分页数据
                var result = _context.Set<ArticleComments>().Include(x => x.User).ThenInclude(x => x!.Member)
                    .Where(x => x.RootId == 0)
                    .Where(funcWhere).OrderByBatch(orderBy);//调用Linq扩展类排序
                if (top > 0) result = result.Take(top);
                var rootList = await result.ToListAsync();

                //所有主键
                List<int> ids = rootList.Select(t => t.Id).ToList();
                var listDto = _mapper.Map<IEnumerable<ArticleCommentsDto>>(rootList);
                //查询子集数据
                var childrenList = await _context.Set<ArticleComments>().Where(x => ids.Contains(x.RootId)).Where(funcWhere).ToListAsync();
                //是否有子集
                if (childrenList != null)
                {
                    foreach (var item in listDto)
                    {
                        var childList = childrenList.Where(t => t.RootId == item.Id).ToList();
                        item.Children = _mapper.Map<IEnumerable<ArticleCommentsDto>>(childList).ToList();
                    }
                }
                return listDto;
            }) ?? [];
        }

        /// <summary>
        /// 查询分页列表(缓存)
        /// </summary>
        public async Task<PaginationList<ArticleCommentsDto>> QueryPageByCacheAsync(string cacheKey, int pageSize, int pageIndex, Expression<Func<ArticleComments, bool>> funcWhere,
            string orderBy, WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            string className = typeof(ArticleComments).Name; //获取类名的字符串
            string classKey = $"{className}:List:{cacheKey}";

            return await _cacheService.GetOrSetAsync(classKey, async () =>
            {
                _context = _contextFactory.CreateContext(writeAndRead); //连接数据库

                //获取第一级的分页数据
                var result = _context.Set<ArticleComments>().Include(x => x.User).ThenInclude(x => x!.Member)
                    .Where(x => x.RootId == 0)
                    .Where(funcWhere).OrderByBatch(orderBy);//调用Linq扩展类排序
                var rootList = await PaginationList<ArticleComments>.CreateAsync(pageIndex, pageSize, result);

                //获取所有主键
                List<int> ids = rootList.Items.Select(t => t.Id).ToList();
                //查询子集数据
                var childrenList = await _context.Set<ArticleComments>().Where(x => ids.Contains(x.RootId)).Where(funcWhere).OrderByBatch(orderBy).ToListAsync();
                //将分页数据映射成DTO
                var listDto = _mapper.Map<IEnumerable<ArticleCommentsDto>>(rootList.Items.ToList());
                foreach (var item in listDto)
                {
                    var childList = childrenList.Where(t => t.RootId == item.Id).ToList();
                    item.Children = _mapper.Map<IEnumerable<ArticleCommentsDto>>(childList).ToList();
                }

                return PaginationList<ArticleCommentsDto>.Create(pageIndex, pageSize, rootList.TotalCount, listDto.ToList());
            }) ?? new PaginationList<ArticleCommentsDto>(0, pageIndex, pageSize, []);
        }

        /// <summary>
        /// 添加一条数据
        /// </summary>
        public async Task<ArticleComments?> AddAsync(ArticleCommentsEditDto modelDto)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库

            var user = await _userService.GetUserAsync() ?? throw new ResponseException("用户未登录或已超时");
            var articleModel = await _context.Set<Articles>().FirstOrDefaultAsync(x => x.Id == modelDto.ArticleId && x.Status == 0)
                ?? throw new ResponseException("评论的文章不存在或已删除");

            var model = _mapper.Map<ArticleComments>(modelDto);
            model.AddTime = DateTime.Now;
            model.UserId = user.Id;
            model.UserName = user.UserName;
            model.UserIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress.ToString();
            model.ChannelId = articleModel.ChannelId;

            //检查是否存在父级
            if (modelDto.ParentId > 0)
            {
                var parentModel = await _context.Set<ArticleComments>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.ParentId)
                    ?? throw new ResponseException("被评论的内容不存在或已删除");
                //继承父级的RootId
                model.RootId = parentModel.RootId == 0 ? parentModel.Id : parentModel.RootId;
                model.AtUserId = parentModel.UserId;
                model.AtUserName = parentModel.UserName;
            }
            else
            {
                model.RootId = 0;
                model.ParentId = 0;
            }

            //保存到数据库
            await _context.Set<ArticleComments>().AddAsync(model); //添加评论
            articleModel.CommentCount++;
            _context.Set<Articles>().Update(articleModel); //更新评论总数
            var result = await this.SaveAsync();
            //删除缓存
            if (result)
            {
                await this.RemoveCacheAsync<ArticleComments>(true);
            }

            model.User = user;
            return model;
        }
    }
}