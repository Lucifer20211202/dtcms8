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

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 文章标签
    /// </summary>
    [Route("admin/article/label")]
    [ApiController]
    public class ArticleLabelController(IArticleLabelService articleLabelService, IUserService userService, IMapper mapper) : ControllerBase
    {
        private readonly IArticleLabelService _articleLabelService = articleLabelService;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取数据
        /// 示例：/admin/article/label/1/1
        /// </summary>
        [HttpGet("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleLabel", ActionType.View, "channelId")]
        public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<ArticleLabelsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _articleLabelService.QueryAsync<ArticleLabels>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<ArticleLabelsDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取指定数量列表
        /// 示例：/admin/article/label/view/1/10
        /// </summary>
        [HttpGet("view/{channelId}/{top}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleLabel", ActionType.View, "channelId")]
        public async Task<IActionResult> GetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<ArticleLabelsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<ArticleLabelsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据库列表
            var resultFrom = await _articleLabelService.QueryListAsync<ArticleLabels>(top,
                x => x.Status == searchParam.Status && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                null,
                searchParam.OrderBy ?? "SortId,Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<ArticleLabelsDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取分页列表
        /// 示例：/admin/article/label/1?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleLabel", ActionType.View, "channelId")]
        public async Task<IActionResult> GetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<ArticleLabelsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<ArticleLabelsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表，如果ID大于0则查询该用户下所有的列表
            var list = await _articleLabelService.QueryPageAsync<ArticleLabels>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                null,
                searchParam.OrderBy ?? "SortId,Id");

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
            var resultDto = _mapper.Map<IEnumerable<ArticleLabelsDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/article/label/1
        /// </summary>
        [HttpPost("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleLabel", ActionType.Add, "channelId")]
        public async Task<IActionResult> Add([FromBody] ArticleLabelsEditDto modelDto)
        {
            //映射成实体
            var model = _mapper.Map<ArticleLabels>(modelDto);
            //获取当前用户名
            model.AddBy = _userService.GetUserName();
            model.AddTime = DateTime.Now;
            //写入数据库
            await _articleLabelService.AddAsync(model);
            //映射成DTO再返回，否则出错
            var result = _mapper.Map<ArticleLabelsDto>(model);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/admin/article/label/1/1
        /// </summary>
        [HttpPut("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleLabel", ActionType.Edit, "channelId")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ArticleLabelsEditDto modelDto)
        {
            //查找记录
            var model = await _articleLabelService.QueryAsync<ArticleLabels>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelDto, model);
            var result = await _articleLabelService.SaveAsync();

            //由于没有调用方法，手动清空缓存
            if (result)
            {
                await _articleLabelService.RemoveCacheAsync<ArticleLabels>(true);
            }

            return NoContent();
        }

        /// <summary>
        /// 局部更新一条记录
        /// 示例：/admin/article/label/1/1
        /// Body：[{"op":"replace","path":"/title","value":"new title"}]
        /// </summary>
        [HttpPatch("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleLabel", ActionType.Edit, "channelId")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] JsonPatchDocument<ArticleLabelsEditDto> patchDocument)
        {
            var model = await _articleLabelService.QueryAsync<ArticleLabels>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            var modelToPatch = _mapper.Map<ArticleLabelsEditDto>(model);
            patchDocument.ApplyTo(modelToPatch, ModelState);
            //验证数据是否合法
            if (!TryValidateModel(modelToPatch))
            {
                return ValidationProblem(ModelState);
            }
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelToPatch, model);
            var result = await _articleLabelService.SaveAsync();

            //由于没有调用方法，手动清空缓存
            if (result)
            {
                await _articleLabelService.RemoveCacheAsync<ArticleLabels>(true);
            }

            return NoContent();
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/article/label/1/1
        /// </summary>
        [HttpDelete("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleLabel", ActionType.Delete, "channelId")]
        public async Task<IActionResult> Delete([FromRoute] long id)
        {
            if (!await _articleLabelService.ExistsAsync<ArticleLabels>(x => x.Id == id))
            {
                throw new ResponseException($"数据[{id}]不存在或已删除");
            }
            var result = await _articleLabelService.DeleteAsync<ArticleLabels>(x => x.Id == id);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录(级联数据)
        /// 示例：/admin/article/label/1?ids=1,2,3
        /// </summary>
        [HttpDelete("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleLabel", ActionType.Delete, "channelId")]
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
            await _articleLabelService.DeleteAsync<ArticleLabels>(x => arrIds.Contains(x.Id));

            return NoContent();
        }
        #endregion

        #region 前台调用接口============================
        /// <summary>
        /// 获取指定数量列表(缓存)
        /// 示例：/client/article/label/view/10
        /// </summary>
        [HttpGet("/client/article/label/view/{top}")]
        public async Task<IActionResult> ClientGetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<ArticleLabelsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<ArticleLabelsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取缓存Key
            var cacheKey = $"{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据库列表
            var resultFrom = await _articleLabelService.QueryListAsync<ArticleLabels>(cacheKey, top,
                x => string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword)),
                null,
                searchParam.OrderBy ?? "SortId,Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<ArticleLabelsDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }
        #endregion

    }
}