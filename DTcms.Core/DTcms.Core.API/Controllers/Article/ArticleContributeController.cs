using AutoMapper;
using DTcms.Core.API.Filters;
using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Extensions;
using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using DTcms.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 文章投稿
    /// </summary>
    [Route("admin/article/contribute")]
    [ApiController]
    public class ArticleContributeController(IArticleContributeService articleContributeService,
        IUserService userService, IMapper mapper) : ControllerBase
    {
        private readonly IArticleContributeService _articleContributeService = articleContributeService;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取数据
        /// 示例：/admin/article/contribute/1/1
        /// </summary>
        [HttpGet("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleContribute", ActionType.View, "channelId")]
        public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<ArticleContributesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _articleContributeService.QueryAsync<ArticleContributes>(x => x.Id == id, null) ?? throw new ResponseException($"数据[{id}]不存在或已删除");
            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<ArticleContributesDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 根据ID获取数据
        /// 示例：/admin/article/contribute/1/1/view
        /// </summary>
        [HttpGet("{channelId}/{id}/view")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleContribute", ActionType.View, "channelId")]
        public async Task<IActionResult> GetByIdView([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<ArticleContributesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _articleContributeService.QueryAsync<ArticleContributes>(x => x.Id == id) ?? throw new ResponseException($"数据[{id}]不存在或已删除");
            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<ArticleContributesViewDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取总记录数量
        /// 示例：/admin/article/contribute/view/count
        /// </summary>
        [HttpGet("view/count")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleContribute", ActionType.View)]
        public async Task<IActionResult> GetCount([FromQuery] BaseParameter searchParam)
        {
            var result = await _articleContributeService.CountAsync<ArticleContributes>(x => searchParam.Status < 0 || x.Status == searchParam.Status);
            //返回成功200
            return Ok(result);
        }

        /// <summary>
        /// 获取指定数量列表
        /// 示例：/admin/article/contribute/view/10
        /// </summary>
        [HttpGet("view/{top}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleContribute", ActionType.View, "channelId")]
        public async Task<IActionResult> GetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<ArticleContributesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<ArticleContributesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据库列表
            var resultFrom = await _articleContributeService.QueryListAsync<ArticleContributes>(top,
                x => string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword)),
                null,
                searchParam.OrderBy ?? "Status,-Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<ArticleContributesDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取分页列表
        /// 示例：/admin/article/contribute/1?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleContribute", ActionType.View, "channelId")]
        public async Task<IActionResult> GetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam, [FromRoute] long channelId)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<ArticleContributesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<ArticleContributesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表，如果ID大于0则查询该用户下所有的列表
            var list = await _articleContributeService.QueryPageAsync<ArticleContributes>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => x.ChannelId == channelId
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword)))
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.UserName != null && x.UserName.Contains(searchParam.Keyword))),
                null,
                searchParam.OrderBy ?? "Status,-Id");

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
            var resultDto = _mapper.Map<IEnumerable<ArticleContributesDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/article/contribute/1
        /// </summary>
        [HttpPost("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleContribute", ActionType.Add, "channelId")]
        public async Task<IActionResult> Add([FromBody] ArticleContributesAddDto modelDto)
        {
            //验证数据是否合法
            if (!TryValidateModel(modelDto))
            {
                return ValidationProblem(ModelState);
            }
            var siteChannel = await _articleContributeService.QueryAsync<SiteChannels>(x => x.Id == modelDto.ChannelId);
            //检查频道是否存在
            if (siteChannel == null)
            {
                throw new ResponseException("频道不存在或已删除");
            }
            //检查是否可以投稿
            if (siteChannel.IsContribute == 0)
            {
                throw new ResponseException("该频道不允许投稿");
            }
            //检查站点是否存在
            modelDto.SiteId = siteChannel.SiteId;
            if (!await _articleContributeService.ExistsAsync<Sites>(x => x.Id == modelDto.SiteId))
            {
                throw new ResponseException("站点不存在或已删除");
            }

            //映射成实体
            var model = _mapper.Map<ArticleContributes>(modelDto);
            //获取当前用户名
            model.UserId = _userService.GetUserId();
            model.UserName = _userService.GetUserName();
            model.AddTime = DateTime.Now;
            //内容摘要提取内容前255个字符
            if (!string.IsNullOrWhiteSpace(model.Content))
            {
                model.Zhaiyao = HtmlHelper.CutString(model.Content, 250);
            }
            //写入数据库
            var mapModel = await _articleContributeService.AddAsync(model);
            //映射成DTO再返回，否则出错
            var result = _mapper.Map<ArticleContributesDto>(mapModel);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/admin/article/contribute/1/1
        /// </summary>
        [HttpPut("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleContribute", ActionType.Edit, "channelId")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ArticleContributesEditDto modelDto)
        {
            //验证数据是否合法
            if (!TryValidateModel(modelDto))
            {
                return ValidationProblem(ModelState);
            }
            var user = await _userService.GetUserAsync() ?? throw new ResponseException("用户未登录或已超时");
            modelDto.UpdateBy = user.UserName;
            modelDto.UpdateTime = DateTime.Now;
            //内容摘要提取内容前255个字符
            if (!string.IsNullOrWhiteSpace(modelDto.Content))
            {
                modelDto.Zhaiyao = HtmlHelper.CutString(modelDto.Content, 250);
            }
            await _articleContributeService.UpdateAsync(id, modelDto);
            return NoContent();
        }

        /// <summary>
        /// 局部更新一条记录
        /// 示例：/admin/article/contribute/1/1
        /// Body：[{"op":"replace","path":"/title","value":"new title"}]
        /// </summary>
        [HttpPatch("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleContribute", ActionType.Edit, "channelId")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] JsonPatchDocument<ArticleContributesEditDto> patchDocument)
        {
            var model = await _articleContributeService.QueryAsync<ArticleContributes>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            var modelToPatch = _mapper.Map<ArticleContributesEditDto>(model);
            patchDocument.ApplyTo(modelToPatch, ModelState);
            //验证数据是否合法
            if (!TryValidateModel(modelToPatch))
            {
                return ValidationProblem(ModelState);
            }
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelToPatch, model);
            await _articleContributeService.SaveAsync();

            return NoContent();
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/article/contribute/1/1
        /// </summary>
        [HttpDelete("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleContribute", ActionType.Delete, "channelId")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!await _articleContributeService.ExistsAsync<ArticleContributes>(x => x.Id == id))
            {
                throw new ResponseException($"数据{id}不存在或已删除");
            }
            var result = await _articleContributeService.DeleteAsync<ArticleContributes>(x => x.Id == id);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录
        /// 示例：/admin/article/contribute/1?ids=1,2,3
        /// </summary>
        [HttpDelete("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleContribute", ActionType.Delete, "channelId")]
        public async Task<IActionResult> DeleteByIds([FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var arrIds = Ids.ToIEnumerable<int>();
            if (arrIds == null)
            {
                throw new ResponseException("传输参数不符合规范");
            }
            //执行批量删除操作
            await _articleContributeService.DeleteAsync<ArticleContributes>(x => arrIds.Contains(x.Id));

            return NoContent();
        }
        #endregion

        #region 普通账户调用接口========================
        /// <summary>
        /// 根据ID获取数据
        /// 示例：/account/article/contribute/1
        /// </summary>
        [HttpGet("/account/article/contribute/{id}")]
        [Authorize]
        public async Task<IActionResult> ClientGetById([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<ArticleContributesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            var userId = _userService.GetUserId();
            //查询数据库获取实体
            var model = await _articleContributeService.QueryAsync<ArticleContributes>(x => x.Id == id && x.UserId == userId);
            if (model == null)
            {
                throw new ResponseException($"数据[{id}]不存在或已删除");
            }
            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<ArticleContributesDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取指定数量列表
        /// 示例：/account/article/contribute/view/10
        /// </summary>
        [HttpGet("/account/article/contribute/view/{top}")]
        [Authorize]
        public async Task<IActionResult> ClientGetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<ArticleContributesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<ArticleContributesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            var userId = _userService.GetUserId();
            //获取数据库列表
            var resultFrom = await _articleContributeService.QueryListAsync<ArticleContributes>(top,
                x => x.UserId == userId
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                null,
                searchParam.OrderBy ?? "Status,-Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<ArticleContributesDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取分页列表
        /// 示例：/account/article/contribute?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet("/account/article/contribute")]
        [Authorize]
        public async Task<IActionResult> ClientGetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<ArticleContributesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<ArticleContributesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            var userId = _userService.GetUserId();

            //获取数据列表，如果ID大于0则查询该用户下所有的列表
            var list = await _articleContributeService.QueryPageAsync<ArticleContributes>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => x.UserId == userId
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                null,
                searchParam.OrderBy ?? "Status,-Id");

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
            var resultDto = _mapper.Map<IEnumerable<ArticleContributesDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/account/article/contribute
        /// </summary>
        [HttpPost("/account/article/contribute")]
        [Authorize]
        public async Task<IActionResult> ClientAdd([FromBody] ArticleContributesAddDto modelDto)
        {
            //验证数据是否合法
            if (!TryValidateModel(modelDto))
            {
                return ValidationProblem(ModelState);
            }
            var siteChannel = await _articleContributeService.QueryAsync<SiteChannels>(x => x.Id == modelDto.ChannelId);
            //检查频道是否存在
            if (siteChannel == null)
            {
                throw new ResponseException("频道不存在或已删除");
            }
            //检查是否可以投稿
            if (siteChannel.IsContribute == 0)
            {
                throw new ResponseException("该频道不允许投稿");
            }
            //检查站点是否存在
            modelDto.SiteId = siteChannel.SiteId;
            if (!await _articleContributeService.ExistsAsync<Sites>(x => x.Id == modelDto.SiteId))
            {
                throw new ResponseException("站点不存在或已删除");
            }

            //映射成实体
            var model = _mapper.Map<ArticleContributes>(modelDto);
            //获取当前用户名
            model.UserId = _userService.GetUserId();
            model.UserName = _userService.GetUserName();
            model.AddTime = DateTime.Now;
            //内容摘要提取内容前255个字符
            if (!string.IsNullOrWhiteSpace(model.Content))
            {
                model.Zhaiyao = HtmlHelper.CutString(model.Content, 250);
            }
            //写入数据库
            var mapModel = await _articleContributeService.AddAsync(model);
            //映射成DTO再返回，否则出错
            var result = _mapper.Map<ArticleContributesDto>(mapModel);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/account/article/contribute/1
        /// </summary>
        [HttpPut("/account/article/contribute/{id}")]
        [Authorize]
        public async Task<IActionResult> ClientUpdate([FromRoute] int id, [FromBody] ArticleContributesEditDto modelDto)
        {
            //验证数据是否合法
            if (!TryValidateModel(modelDto))
            {
                return ValidationProblem(ModelState);
            }
            var user = await _userService.GetUserAsync()
                ?? throw new ResponseException($"用户未登录或已超时");
            

            modelDto.UserId = user.Id;
            modelDto.UserName = user.UserName;
            modelDto.UpdateBy = user.UserName;
            //内容摘要提取内容前250个字符
            if (!string.IsNullOrWhiteSpace(modelDto.Content))
            {
                modelDto.Zhaiyao = HtmlHelper.CutString(modelDto.Content, 250);
            }
            await _articleContributeService.UserUpdateAsync(id, modelDto);
            return NoContent();
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/account/article/contribute/1
        /// </summary>
        [HttpDelete("/account/article/contribute/{id}")]
        [Authorize]
        public async Task<IActionResult> ClientDelete([FromRoute] int id)
        {
            var userId = _userService.GetUserId(); //获取当前用户ID
            if (!await _articleContributeService.ExistsAsync<ArticleContributes>(x => x.Id == id && x.UserId == userId))
            {
                throw new ResponseException($"数据[{id}]不存在或已删除");
            }

            await _articleContributeService.DeleteAsync<ArticleContributes>(x => x.Id == id);
            return NoContent();
        }

        /// <summary>
        /// 批量删除记录
        /// 示例：/account/article/contribute?ids=1,2,3
        /// </summary>
        [HttpDelete("/account/article/contribute")]
        [Authorize]
        public async Task<IActionResult> ClientDeleteByIds([FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            var userId = _userService.GetUserId();//获取当前用户ID
            //将ID列表转换成IEnumerable
            var arrIds = Ids.ToIEnumerable<int>() ?? throw new ResponseException("传输参数不符合规范");
            //执行批量删除操作
            await _articleContributeService.DeleteAsync<ArticleContributes>(x => arrIds.Contains(x.Id) && x.UserId == userId);
            return NoContent();
        }
        #endregion
    }
}