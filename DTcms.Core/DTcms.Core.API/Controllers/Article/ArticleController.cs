using AutoMapper;
using DTcms.Core.API.Filters;
using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Extensions;
using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 文章接口
    /// </summary>
    [Route("admin/article")]
    [ApiController]
    public class ArticleController(IArticleService articlesService, IArticleCategoryService categoryService,
        IArticleLikeService likeService, IUserService userService, IMapper mapper) : ControllerBase
    {
        private readonly IArticleService _articleService = articlesService;
        private readonly IArticleCategoryService _categoryService = categoryService;
        private readonly IArticleLikeService _likeService = likeService;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取数据
        /// 示例：/admin/article/1/1
        /// </summary>
        [HttpGet("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Article", ActionType.View, "channelId")]
        public async Task<IActionResult> GetById([FromRoute] long id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<ArticlesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _articleService.QueryAsync<Articles>(x => x.Id == id,
                query => query.Include(x => x.ArticleFields)
                .Include(x => x.ArticleGroups)
                .Include(x => x.ArticleAlbums)
                .Include(x => x.ArticleAttachs)
                .Include(x => x.CategoryRelations).ThenInclude(x => x.Category)
                .Include(x => x.LabelRelations).ThenInclude(x => x.Label)
                .Include(x => x.SiteChannel))
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<ArticlesDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取指定数量列表
        /// 示例：/admin/article/view/1/10
        /// </summary>
        [HttpGet("view/{channelId}/{top}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Article", ActionType.View, "channelId")]
        public async Task<IActionResult> GetList([FromRoute] int channelId, [FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<ArticlesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<ArticlesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据库列表
            var resultFrom = await _articleService.QueryListAsync<Articles>(top,
                x => x.ChannelId == channelId
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                query => query.Include(x => x.ArticleFields)
                .Include(x => x.ArticleAlbums),
                searchParam.OrderBy ?? "SortId,-Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<ArticlesListDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取分页列表
        /// 示例：/admin/article/1?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Article", ActionType.View, "channelId")]
        public async Task<IActionResult> GetList([FromQuery] ArticleParameter searchParam, [FromQuery] PageParamater pageParam, [FromRoute] int channelId)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<ArticlesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<ArticlesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表，如果ID大于0则查询该用户下所有的列表
            var list = await _articleService.QueryPageAsync<Articles>(pageParam.PageSize,
                pageParam.PageIndex,
                x => x.ChannelId == channelId
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword)))
                && (searchParam.CategoryId <= 0 || x.CategoryRelations.Any(x => x.CategoryId == searchParam.CategoryId))
                && (searchParam.LabelId <= 0 || x.LabelRelations.Any(x => x.LabelId == searchParam.LabelId))
                && (searchParam.StartDate == null || DateTime.Compare(x.AddTime, searchParam.StartDate.GetValueOrDefault()) >= 0)
                && (searchParam.EndDate == null || DateTime.Compare(x.AddTime, searchParam.EndDate.GetValueOrDefault()) <= 0),
                query => query.Include(x => x.CategoryRelations).ThenInclude(x => x.Category)
                .Include(x => x.LabelRelations).ThenInclude(x => x.Label),
                searchParam.OrderBy ?? "SortId,-Id");

            //x-pagination
            var paginationMetadata = new
            {
                totalCount = list.TotalCount,
                pageSize = list.PageSize,
                pageIndex = list.PageIndex,
                totalPages = list.TotalPages
            };
            Response.Headers.Append("x-pagination", JsonHelper.ToJson(paginationMetadata));

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<ArticlesListDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/article/1
        /// </summary>
        [HttpPost("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Article", ActionType.Add, "channelId")]
        public async Task<IActionResult> Add([FromBody] ArticlesAddDto modelDto)
        {
            //验证数据是否合法
            if (!TryValidateModel(modelDto))
            {
                return ValidationProblem(ModelState);
            }
            //检查频道是否存在
            var channelModel = await _articleService.QueryAsync<SiteChannels>(x => x.Id == modelDto.ChannelId)
                ?? throw new ResponseException("频道不存在或已删除");

            //获取当前用户信息
            var userInfo = await _userService.GetUserAsync()
                ?? throw new ResponseException("用户未登录或已超时");
            //获取管理员身份
            var manageInfo = await _articleService.QueryAsync<Managers>(x => x.UserId == userInfo.Id)
                ?? throw new ResponseException($"管理员身份有误，请核实后操作");
            //如果需要审核则设置为审核状态
            if (manageInfo.IsAudit > 0)
            {
                modelDto.Status = 1;
            }
            modelDto.SiteId = channelModel.SiteId;
            modelDto.AddBy = userInfo.UserName;
            modelDto.AddTime = DateTime.Now;
            //映射成实体
            var model = _mapper.Map<Articles>(modelDto);
            //内容摘要提取内容前255个字符
            if (string.IsNullOrWhiteSpace(model.Zhaiyao) && !string.IsNullOrWhiteSpace(model.Content))
            {
                model.Zhaiyao = HtmlHelper.CutString(model.Content, 250);
            }
            //写入数据库
            var mapModel = await _articleService.AddAsync(model);
            //映射成DTO再返回，否则出错
            var result = _mapper.Map<ArticlesDto>(mapModel);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/admin/article/1/15
        /// </summary>
        [HttpPut("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Article", ActionType.Edit, "channelId")]
        public async Task<IActionResult> Update([FromRoute] long id, [FromBody] ArticlesEditDto modelDto)
        {
            //验证数据是否合法
            if (!TryValidateModel(modelDto))
            {
                return ValidationProblem(ModelState);
            }
            //内容摘要提取内容前255个字符
            if (string.IsNullOrWhiteSpace(modelDto.Zhaiyao) && !string.IsNullOrWhiteSpace(modelDto.Content))
            {
                modelDto.Zhaiyao = HtmlHelper.CutString(modelDto.Content, 250);
            }
            //检查频道是否存在
            var channelModel = await _articleService.QueryAsync<SiteChannels>(x => x.Id == modelDto.ChannelId)
                ?? throw new ResponseException("频道不存在或已删除");
            //获取当前用户信息
            var userInfo = await _userService.GetUserAsync()
                ?? throw new ResponseException($"用户未登录或已超时");
            //获取管理员信息
            var manageInfo = await _articleService.QueryAsync<Managers>(x => x.UserId == userInfo.Id)
                ?? throw new ResponseException("管理员身份有误，请核实后操作");
            //如里需要审核则设置为待审状态
            if (manageInfo.IsAudit > 0)
            {
                modelDto.Status = 1;
            }
            modelDto.SiteId = channelModel.SiteId;
            modelDto.UpdateBy = userInfo.UserName;
            modelDto.UpdateTime = DateTime.Now;

            //查找记录
            var model = await _articleService.QueryAsync<Articles>(x => x.Id == id,
                query => query.Include(x => x.ArticleFields)
                .Include(x => x.ArticleGroups)
                .Include(x => x.ArticleAlbums)
                .Include(x => x.ArticleAttachs)
                .Include(x => x.CategoryRelations)
                .Include(x => x.LabelRelations), WriteRoRead.Write)
                ?? throw new ResponseException($"记录[{id}]不存在或已删除");

            //Dto映射到Model
            _mapper.Map(modelDto, model);
            //注意：下面两种都可以调，由于EFCORE已跟踪，直接调用保存也可以的
            //await _articleService.SaveAsync();
            await _articleService.UpdateAsync<Articles>(model);
            return NoContent();
        }

        /// <summary>
        /// 局部更新一条记录
        /// 示例：/admin/article/1/1
        /// Body：[{"op":"replace","path":"/title","value":"new title"}]
        /// </summary>
        [HttpPatch("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Article", ActionType.Edit, "channelId")]
        public async Task<IActionResult> Update([FromRoute] long id, [FromBody] JsonPatchDocument<ArticlesEditDto> patchDocument)
        {
            //注意：要使用写的数据库进行查询，才能正确写入数据主库
            var model = await _articleService.QueryAsync<Articles>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            var modelToPatch = _mapper.Map<ArticlesEditDto>(model);
            patchDocument.ApplyTo(modelToPatch, ModelState);
            //验证数据是否合法
            if (!TryValidateModel(modelToPatch))
            {
                return ValidationProblem(ModelState);
            }
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelToPatch, model);
            var result = await _articleService.SaveAsync();

            //由于没有用到方法，手动删除缓存
            if (result)
            {
                await _articleService.RemoveCacheAsync<Articles>(true);
            }

            return NoContent();
        }

        /// <summary>
        /// 批量审核记录
        /// 示例：/admin/article/1?ids=1,2,3
        /// </summary>
        [HttpPut("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Article", ActionType.Audit, "channelId")]
        public async Task<IActionResult> AuditByIds([FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var arrIds = Ids.ToIEnumerable<long>() ?? throw new ResponseException("传输参数不符合规范");

            //找出符合条件的记录
            var list = await _articleService.QueryListAsync<Articles>(1000, x => x.Status == 1 && arrIds.Contains(x.Id), null, null, WriteRoRead.Write);
            if (!list.Any())
            {
                throw new ResponseException("没有找到需要审核的记录");
            }
            foreach (var item in list)
            {
                item.Status = 0;
            }
            //保存到数据库
            var result = await _articleService.SaveAsync();

            //由于没有用到方法，手动删除缓存
            if (result)
            {
                await _articleService.RemoveCacheAsync<Articles>(true);
            }

            return NoContent();
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/article/1/1
        /// </summary>
        [HttpDelete("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Article", ActionType.Delete, "channelId")]
        public async Task<IActionResult> Delete([FromRoute] long id)
        {
            if (!await _articleService.ExistsAsync<Articles>(x => x.Id == id))
            {
                throw new ResponseException($"数据[{id}]不存在或已删除");
            }
            var result = await _articleService.DeleteAsync(x => x.Id == id);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录(级联数据)
        /// 示例：/admin/article/1?ids=1,2,3
        /// </summary>
        [HttpDelete("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Article", ActionType.Delete, "channelId")]
        public async Task<IActionResult> DeleteByIds([FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var arrIds = Ids.ToIEnumerable<long>() ?? throw new ResponseException("传输参数不符合规范");
            //执行批量删除操作
            await _articleService.DeleteAsync(x => arrIds.Contains(x.Id));

            return NoContent();
        }

        /// <summary>
        /// 获取总记录数量
        /// 示例：/admin/article/view/count
        /// </summary>
        [HttpGet("view/count")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetCount([FromQuery] ArticleParameter searchParam)
        {
            var result = await _articleService.CountAsync<Articles>(
                x => (searchParam.Status <= 0 || x.Status == searchParam.Status));
            //返回成功200
            return Ok(result);
        }
        #endregion

        #region 当前用户调用接口========================
        /// <summary>
        /// 更新点赞量
        /// 示例：/account/article/like/1
        /// </summary>
        [Authorize]
        [HttpPut("/account/article/like/{articleId}")]
        [Authorize]
        public async Task<IActionResult> UpdateLike([FromRoute] long articleId)
        {
            var likeCount = await _likeService.UserUpdateLikeAsync(articleId);
            return Ok(likeCount);
        }
        #endregion

        #region 前台调用接口============================
        /// <summary>
        /// 根据文章别名或ID获取数据(缓存)
        /// 示例：/client/article/show/1
        /// </summary>
        [HttpGet("/client/article/show/{key}")]
        public async Task<IActionResult> ClientGetByKey([FromRoute] string key, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<ArticlesClientDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //查询数据库获取实体
            Articles? model = null;
            if (int.TryParse(key, out int articleId))
            {
                model = await _articleService.QueryAsync<Articles>(key,
                    x => x.Id == articleId && x.Status == 0,
                    query => query.Include(x => x.SiteChannel)
                    .Include(x => x.ArticleFields)
                    .Include(x => x.ArticleGroups)
                    .Include(x => x.ArticleAlbums)
                    .Include(x => x.ArticleAttachs)
                    .Include(x => x.CategoryRelations).ThenInclude(x => x.Category)
                    .Include(x => x.LabelRelations).ThenInclude(x => x.Label));
            }
            if (model == null)
            {
                model = await _articleService.QueryAsync<Articles>(key,
                    x => x.Status == 0
                    && (x.CallIndex != null && x.CallIndex.ToLower() == key.ToLower())
                    && (param.SiteId <= 0 || x.SiteId == param.SiteId),
                    query => query.Include(x => x.SiteChannel)
                    .Include(x => x.ArticleFields)
                    .Include(x => x.ArticleGroups)
                    .Include(x => x.ArticleAlbums)
                    .Include(x => x.ArticleAttachs)
                    .Include(x => x.CategoryRelations).ThenInclude(x => x.Category)
                    .Include(x => x.LabelRelations).ThenInclude(x => x.Label));
            }
            //查询数据库获取实体
            if (model == null)
            {
                throw new ResponseException($"数据{key}不存在或已删除");
            }
            //检查用户组是否有权限
            if (model.ArticleGroups.Count > 0)
            {
                var userId = _userService.GetUserId();
                var memberModel = await _articleService.QueryAsync<Members>(x => x.UserId == userId)
                    ?? throw new ResponseException($"请登录后查看内容");
                if (!model.ArticleGroups.Any(x => x.GroupId == memberModel.GroupId))
                {
                    throw new ResponseException("出错了，此内容仅对部分会员开放");
                }
            }
            //浏览量加一
            model.Click++;
            await _articleService.UpdateClickAsync(key, model.Id, model.Click);
            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<ArticlesClientDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 根据文章ID获取上一条下一条(缓存)
        /// 示例：/client/article/next/1/10
        /// </summary>
        [HttpGet("/client/article/next/{id}")]
        public async Task<IActionResult> ClientGetList([FromRoute] long id, [FromQuery] ArticleParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<ArticlesClientListDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<ArticlesClientListDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //得到当前用户会员组ID
            var userId = _userService.GetUserId();
            int memberGroupId = 0;
            if (userId > 0)
            {
                var memberModel = await _articleService.QueryAsync<Members>(x => x.UserId == userId);
                if (memberModel != null)
                {
                    memberGroupId = memberModel.GroupId;
                }
            }

            //获取缓存Key
            var cacheKey = $"{userId}:{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据库列表
            var resultFrom = await _articleService.QueryNextByCacheAsync(cacheKey, id,
                x => x.ArticleGroups.Count == 0 || x.ArticleGroups.Any(g => g.GroupId == memberGroupId));

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<ArticlesClientListDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 根据文章ID获取同类别下的其它列表(缓存)
        /// 示例：/client/article/view/1/10
        /// </summary>
        [HttpGet("/client/article/view/{id}/{top}")]
        public async Task<IActionResult> ClientGetList([FromRoute] int id, [FromRoute] int top, [FromQuery] ArticleParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<ArticlesClientListDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<ArticlesClientListDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //获取文章实体
            var model = await _articleService.QueryAsync<Articles>(x => x.Id == id,
                query => query.Include(x => x.CategoryRelations))
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");
            //得到所有分类的ID
            List<int> categorys = model.CategoryRelations.Select(x => x.CategoryId).ToList();

            //得到当前用户会员组ID
            var userId = _userService.GetUserId();
            int memberGroupId = 0;
            if (userId > 0)
            {
                var memberModel = await _articleService.QueryAsync<Members>(x => x.UserId == userId);
                if (memberModel != null)
                {
                    memberGroupId = memberModel.GroupId;
                }
            }
            
            //获取缓存Key
            var cacheKey = $"{userId}:{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据库列表
            var resultFrom = await _articleService.QueryListAsync<Articles>(cacheKey, top,
                x => (x.ArticleGroups.Count == 0 || x.ArticleGroups.Any(g => g.GroupId == memberGroupId)) //检查是否有权限
                && x.CategoryRelations.Any(r => categorys.Contains(r.CategoryId)) //当前文章的分类
                && (searchParam.LabelId <= 0 || x.LabelRelations.Any(t => t.LabelId == searchParam.LabelId)) //标签搜索
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))) //搜索关键字
                && (searchParam.StartDate == null || DateTime.Compare(x.AddTime, searchParam.StartDate.GetValueOrDefault()) >= 0)
                && (searchParam.EndDate == null || DateTime.Compare(x.AddTime, searchParam.EndDate.GetValueOrDefault()) <= 0)
                && x.Id != id,
                query => query.Include(x => x.ArticleFields)
                .Include(x => x.ArticleGroups)
                .Include(x => x.CategoryRelations)
                .Include(x => x.LabelRelations),
                searchParam.OrderBy ?? "SortId,-Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<ArticlesClientListDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 根据频道ID或名称获取指定数量列表(缓存)
        /// 示例：/client/article/channel/news/view/10?siteId=1
        /// </summary>
        [HttpGet("/client/article/channel/{channelKey}/view/{top}")]
        public async Task<IActionResult> ClientGetList([FromRoute] string channelKey, [FromRoute] int top, [FromQuery] ArticleParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<ArticlesClientListDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<ArticlesClientListDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            if (searchParam.SiteId == 0)
            {
                throw new ResponseException("所属站点不能为空");
            }
            if (string.IsNullOrWhiteSpace(channelKey))
            {
                throw new ResponseException("频道名称不能为空");
            }

            //检查是否频道ID或者名称
            SiteChannels? channelModel = null;
            if (int.TryParse(channelKey, out int channelId))
            {
                channelModel = await _articleService.QueryAsync<SiteChannels>(x => x.Id.Equals(channelId) && x.SiteId.Equals(searchParam.SiteId));
            }
            if (channelModel == null)
            {
                channelModel = await _articleService.QueryAsync<SiteChannels>(x => x.Name != null && x.Name.Equals(channelKey) && x.SiteId.Equals(searchParam.SiteId));
            }
            if (channelModel == null)
            {
                throw new ResponseException("频道不存在或已删除");
            }

            //得到当前用户会员组ID
            var userId = _userService.GetUserId();
            int memberGroupId = 0;
            if (userId > 0)
            {
                var memberModel = await _articleService.QueryAsync<Members>(x => x.UserId == userId);
                if (memberModel != null)
                {
                    memberGroupId = memberModel.GroupId;
                }
            }

            //获取缓存Key
            var cacheKey = $"{userId}:{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据库列表
            var resultFrom = await _articleService.QueryListAsync<Articles>(cacheKey, top,
                x => x.ChannelId == channelModel.Id
                && (x.ArticleGroups.Count == 0 || x.ArticleGroups.Any(g => g.GroupId == memberGroupId)) //检查是否有权限
                && (searchParam.CategoryId <= 0 || x.CategoryRelations.Any(t => t.CategoryId == searchParam.CategoryId))//查询分类
                && (searchParam.LabelId <= 0 || x.LabelRelations.Any(t => t.LabelId == searchParam.LabelId))//标签搜索
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword)))//查询关键字
                && (searchParam.StartDate == null || DateTime.Compare(x.AddTime, searchParam.StartDate.GetValueOrDefault()) >= 0)
                && (searchParam.EndDate == null || DateTime.Compare(x.AddTime, searchParam.EndDate.GetValueOrDefault()) <= 0),
                query => query.Include(x => x.ArticleFields)
                .Include(x => x.ArticleGroups)
                .Include(x => x.CategoryRelations).ThenInclude(x => x.Category)
                .Include(x => x.LabelRelations).ThenInclude(x => x.Label)
                .Include(x => x.ArticleAlbums),
                searchParam.OrderBy ?? "SortId,-Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<ArticlesClientListDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 根据频道ID或名称获取分页列表(缓存)
        /// 示例：/client/article/channel/news?siteId=1&pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet("/client/article/channel/{channelKey}")]
        public async Task<IActionResult> ClientGetList([FromRoute] string channelKey, [FromQuery] ArticleParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<ArticlesClientListDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<ArticlesClientListDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            if (searchParam.SiteId == 0)
            {
                throw new ResponseException("所属站点不能为空");
            }

            //检查是否频道ID或者名称
            SiteChannels? channelModel = null;
            if (int.TryParse(channelKey, out int channelId))
            {
                channelModel = await _articleService.QueryAsync<SiteChannels>(x => x.Id.Equals(channelId) && x.SiteId.Equals(searchParam.SiteId));
            }
            if (channelModel == null)
            {
                channelModel = await _articleService.QueryAsync<SiteChannels>(x => x.Name != null && x.Name.Equals(channelKey) && x.SiteId.Equals(searchParam.SiteId));
            }
            if (channelModel == null)
            {
                throw new ResponseException("频道不存在或已删除");
            }

            //得到当前用户会员组ID
            var userId = _userService.GetUserId();
            int memberGroupId = 0;
            if (userId > 0)
            {
                var memberModel = await _articleService.QueryAsync<Members>(x => x.UserId == userId);
                if (memberModel != null)
                {
                    memberGroupId = memberModel.GroupId;
                }
            }

            //获取缓存Key
            var cacheKey = $"{userId}:{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据列表，如果ID大于0则查询该用户下所有的列表
            var list = await _articleService.QueryPageAsync<Articles>(cacheKey, pageParam.PageSize,
                pageParam.PageIndex,
                x => x.ChannelId == channelModel.Id
                && (x.ArticleGroups.Count == 0 || x.ArticleGroups.Any(g => g.GroupId == memberGroupId)) //检查是否有权限
                && (searchParam.CategoryId <= 0 || x.CategoryRelations.Any(t => t.CategoryId == searchParam.CategoryId))//查询分类
                && (searchParam.LabelId <= 0 || x.LabelRelations.Any(t => t.LabelId == searchParam.LabelId))//标签搜索
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword)))
                && (searchParam.StartDate == null || DateTime.Compare(x.AddTime, searchParam.StartDate.GetValueOrDefault()) >= 0)
                && (searchParam.EndDate == null || DateTime.Compare(x.AddTime, searchParam.EndDate.GetValueOrDefault()) <= 0),
                query => query.Include(x => x.ArticleFields)
                .Include(x => x.ArticleGroups)
                .Include(x => x.ArticleAlbums)
                .Include(x => x.CategoryRelations).ThenInclude(x => x.Category)
                .Include(x => x.LabelRelations).ThenInclude(x => x.Label),
                searchParam.OrderBy ?? "SortId,-Id");

            //x-pagination
            var paginationMetadata = new
            {
                totalCount = list.TotalCount,
                pageSize = list.PageSize,
                pageIndex = list.PageIndex,
                totalPages = list.TotalPages
            };
            Response.Headers.Append("x-pagination", JsonHelper.ToJson(paginationMetadata));

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<ArticlesClientListDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 获得一级分类下的文章前几条数据(缓存)
        /// </summary>
        [HttpGet("/client/article/channel/{channelKey}/view/{categoryTop}/{arcitleTop}")]
        public async Task<IActionResult> ClientGetCategoryList([FromRoute] string channelKey, [FromRoute] int categoryTop, [FromRoute] int arcitleTop, [FromQuery] BaseParameter searchParam)
        {
            if (searchParam.SiteId == 0)
            {
                throw new ResponseException("所属站点不能为空");
            }
            //获得频道实体
            var channelModel = await _articleService.QueryAsync<SiteChannels>(x => x.Name != null && x.Name.Equals(channelKey) && x.SiteId == searchParam.SiteId)
                ?? throw new ResponseException("频道不存在，请检查后重试。");
            //得到当前用户会员组ID
            var userId = _userService.GetUserId();
            int memberGroupId = 0;
            if (userId > 0)
            {
                var memberModel = await _articleService.QueryAsync<Members>(x => x.UserId == userId);
                if (memberModel != null)
                {
                    memberGroupId = memberModel.GroupId;
                }
            }

            //获取前几条分类,最多获取前10个分类
            if (categoryTop > 10) categoryTop = 10;
            if (arcitleTop > 10) arcitleTop = 10;
            //获取缓存Key
            var cacheKey = $"{userId}:{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";

            var result = await _categoryService.QueryArticleListAsync(cacheKey,channelModel.Id, categoryTop, arcitleTop, 0,
                x => x.ChannelId == channelModel.Id
                && (x.ArticleGroups.Count == 0 || x.ArticleGroups.Any(g => g.GroupId == memberGroupId)) //检查是否有权限
                 && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                searchParam.OrderBy ?? "SortId,Id");

            return Ok(result);
        }

        /// <summary>
        /// 更新点击量
        /// 示例：/client/article/1
        /// </summary>
        [HttpPut("/client/article/{id}")]
        public async Task<IActionResult> UpdateClick([FromRoute] long id)
        {
            //查出原来的数据
            var model = await _articleService.QueryAsync<Articles>(x => x.Id == id) ?? throw new ResponseException("请求参数不正确");
            var count = await _articleService.UpdateClickAsync(model.CallIndex, model.Id, model.Click + 1);
            return Ok(count);
        }
        #endregion
    }
}