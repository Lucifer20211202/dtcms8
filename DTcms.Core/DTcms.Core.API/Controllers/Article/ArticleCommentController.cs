using AutoMapper;
using DTcms.Core.API.Filters;
using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Extensions;
using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 文章评论
    /// </summary>
    [Route("admin/article/comment")]
    [ApiController]
    public class ArticleCommentController(IArticleCommentService commentService, IArticleCommentLikeService likeService, IArticleService articleService,
        IUserService userService, IMapper mapper) : ControllerBase
    {
        private readonly IArticleCommentService _commentService = commentService;
        private readonly IArticleCommentLikeService _likeService = likeService;
        private readonly IArticleService _articleService = articleService;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取数据
        /// 示例：/admin/article/comment/1/1
        /// </summary>
        [HttpGet("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleComment", ActionType.View, "channelId")]
        public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<ArticleCommentsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _commentService.QueryAsync<ArticleComments>(x => x.Id == id, query => query.Include(c => c.User).ThenInclude(x => x!.Member))
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<ArticleCommentsDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取分页列表
        /// 示例：/admin/article/comment/1?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleComment", ActionType.View, "channelId")]
        public async Task<IActionResult> GetList([FromRoute] int channelId, [FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy!=null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<ArticleCommentsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<ArticleCommentsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表，如果ID大于0则查询该用户下所有的列表
            var list = await _commentService.QueryPageAsync(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => x.ChannelId == channelId
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Content != null && x.Content.Contains(searchParam.Keyword))),
                searchParam.OrderBy ?? "AddTime,-Id");

            //x-pagination
            var paginationMetadata = new
            {
                totalCount = list.TotalCount,
                pageSize = list.PageSize,
                pageIndex = list.PageIndex,
                totalPages = list.TotalPages
            };
            Response.Headers.Append("x-pagination", JsonHelper.ToJson(paginationMetadata));

            //根据字段进行塑形
            var result = list.Items.AsEnumerable().ShapeData(searchParam.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/ArticleComment/1/1
        /// </summary>
        [HttpPut("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleComment", ActionType.Edit, "channelId")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ArticleCommentsEditDto modelDto)
        {
            //查找记录
            var model = await _commentService.QueryAsync<ArticleComments>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelDto, model);
            var result = await _commentService.SaveAsync();
            return NoContent();
        }

        /// <summary>
        /// 批量审核
        /// 示例：/admin/article/comment/1?ids=1,2,3
        /// </summary>
        [HttpPut("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleComment", ActionType.Audit, "channelId")]
        public async Task<IActionResult> Updates([FromRoute] int channelId, [FromQuery] string ids)
        {
            if (ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var idList = ids.ToIEnumerable<int>() ?? throw new ResponseException("传输参数不符合规范");
            //查找记录
            var list = await _commentService.QueryListAsync<ArticleComments>(0, x => x.ChannelId == channelId && idList.Contains(x.Id))
                ?? throw new ResponseException("没有要审核的评论");

            foreach (var item in list)
            {
                item.Status = 0;
            }
            var result = await _commentService.UpdateAsync(list);
            return NoContent();
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/article/comment/1/1
        /// </summary>
        [HttpDelete("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleComment", ActionType.Delete, "channelId")]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromRoute] int channelId)
        {
            if (!await _commentService.ExistsAsync<ArticleComments>(x => x.Id == id))
            {
                throw new ResponseException($"数据{id}不存在或已删除");
            }
            var result = await _commentService.DeleteAsync<ArticleComments>(x => x.ChannelId == channelId && (x.Id == id || x.RootId == id));

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录(级联数据)
        /// 示例：/admin/article/comment/1?ids=1,2,3
        /// </summary>
        [HttpDelete("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleComment", ActionType.Delete, "channelId")]
        public async Task<IActionResult> DeleteByIds([FromRoute] int channelId, [FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var idList = Ids.ToIEnumerable<int>() ?? throw new ResponseException("传输参数不符合规范");
            //执行批量删除操作
            await _commentService.DeleteAsync<ArticleComments>(x => x.ChannelId == channelId && (idList.Contains(x.Id) || idList.Contains(x.RootId)));

            return NoContent();
        }
        #endregion

        #region 普通账户调用接口========================
        /// <summary>
        /// 添加一条记录
        /// 示例：/account/article/comment/add
        /// </summary>
        [Authorize]
        [HttpPost("/account/article/comment/add")]
        public async Task<IActionResult> Add([FromBody] ArticleCommentsEditDto modelDto)
        {
            //获取文章实体
            var articleModel = await _articleService.QueryAsync<Articles>(t => t.Id == modelDto.ArticleId, query => query.Include(x => x.SiteChannel))
                ?? throw new ResponseException("文章不存在或已删除");

            //判断是否关闭评论
            if (articleModel.IsComment == 0 || articleModel.SiteChannel?.IsComment == 0)
            {
                throw new ResponseException("该文章已关闭评论");
            }

            //写入数据库
            var model = await _commentService.AddAsync(modelDto);
            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<ArticleCommentsDto>(model);
            return Ok(result);
        }

        /// <summary>
        /// 更新点赞量
        /// 示例：/account/article/comment/like/1
        /// </summary>
        [Authorize]
        [HttpPut("/account/article/comment/like/{commentId}")]
        public async Task<IActionResult> UpdateLike([FromRoute] int commentId)
        {
            var likeCount = await _likeService.UserUpdateLikeAsync(commentId);
            return Ok(likeCount);

        }
        #endregion

        #region 前台调用接口============================
        /// <summary>
        /// 获取指定文章评价总数
        /// 示例：/client/article/comment/count/1
        /// </summary>
        [HttpGet("/client/article/comment/count/{articleId}")]
        public async Task<IActionResult> GetByArticleCount([FromRoute] int articleId, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<ArticleCommentsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            //获取数据库列表
            var result = await _commentService.CountAsync<ArticleComments>(
                x => x.Status == 0
                && x.ArticleId == articleId
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Content != null && x.Content.Contains(searchParam.Keyword))));
            //返回成功200
            return Ok(result);
        }

        /// <summary>
        /// 获取文章前几条评论列表(缓存)
        /// 示例：/client/article/comment/view/1/10
        /// </summary>
        [HttpGet("/client/article/comment/view/{articleId}/{top}")]
        public async Task<IActionResult> GetByArticleList([FromRoute] int articleId, [FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<ArticleCommentsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<ArticleCommentsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取缓存Key
            var cacheKey = $"{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据库列表
            var list = await _commentService.QueryListByCacheAsync(cacheKey, top,
                x => x.Status == 0
                && x.ArticleId == articleId
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Content != null && x.Content.Contains(searchParam.Keyword))),
                searchParam.OrderBy ?? "Id");

            //根据字段进行塑形
            var resultDto = list.AsEnumerable().ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取文章评论分页列表(缓存)
        /// 示例：/client/article/comment/1?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet("/client/article/comment/{articleId}")]
        public async Task<IActionResult> GetByArticlePageList([FromRoute] int articleId, [FromQuery] BaseParameter param, [FromQuery] PageParamater pageParam)
        {
            param.Fields = "Id,ParentId,RootId,UserName,UserAvatar,AtUserName," +
                "Content,LikeCount,DateDescription,Children";

            //检测参数是否合法
            if (param.OrderBy != null
                && !param.OrderBy.Replace("-", "").IsPropertyExists<ArticleCommentsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!param.Fields.IsPropertyExists<ArticleCommentsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取缓存Key
            var cacheKey = $"{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据列表，如果ID大于0则查询该用户下所有的列表
            var list = await _commentService.QueryPageByCacheAsync(cacheKey,
                pageParam.PageSize,
                pageParam.PageIndex,
                x => x.Status == 0
                && x.ArticleId == articleId
                && (string.IsNullOrWhiteSpace(param.Keyword) || (x.Content != null && x.Content.Contains(param.Keyword))),
                param.OrderBy ?? "Id");

            //x-pagination
            var paginationMetadata = new
            {
                totalCount = list.TotalCount,
                pageSize = list.PageSize,
                pageIndex = list.PageIndex,
                totalPages = list.TotalPages
            };
            Response.Headers.Append("x-pagination", JsonHelper.ToJson(paginationMetadata));

            //根据字段进行塑形
            var resultDto = list.Items.AsEnumerable().ShapeData(param.Fields);
            return Ok(resultDto);
        }
        #endregion
    }
}
