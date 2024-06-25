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
    /// 频道接口
    /// </summary>
    [Route("admin/channel")]
    [ApiController]
    public class SiteChannelController(ISiteChannelService channelService, IMapper mapper) : ControllerBase
    {
        private readonly ISiteChannelService _channelService = channelService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取频道
        /// 示例：/admin/channel/1
        /// </summary>
        [HttpGet("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Channel", ActionType.View)]
        public async Task<IActionResult> GetById([FromRoute] int channelId, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<SiteChannelsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _channelService.QueryAsync<SiteChannels>(x => x.Id == channelId,
                query => query.Include(c => c.Fields), WriteRoRead.Write)
                ?? throw new ResponseException($"频道[{channelId}]不存在或已删除");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<SiteChannelsDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取指定数量列表
        /// 示例：/admin/channel/view/0
        /// </summary>
        [HttpGet("view/{top}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Channel", ActionType.View)]
        public async Task<IActionResult> GetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<SiteChannelsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<SiteChannelsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据库列表
            var list = await _channelService.QueryListAsync<SiteChannels>(top,
                x => (searchParam.SiteId <= 0 || x.SiteId == searchParam.SiteId)
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                query => query.Include(c => c.Fields),
                searchParam.OrderBy ?? "SortId,AddTime");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<SiteChannelsDto>>(list).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取频道分页列表
        /// 示例：/admin/channel?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Channel", ActionType.View)]
        public async Task<IActionResult> GetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.TrimStart('-').IsPropertyExists<SiteChannelsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<SiteChannelsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表
            var list = await _channelService.QueryPageAsync<SiteChannels>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => (searchParam.SiteId <= 0 || x.SiteId == searchParam.SiteId)
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                query => query.Include(c => c.Fields),
                searchParam.OrderBy ?? "SortId,AddTime");

            //x-pagination
            var paginationMetadata = new
            {
                totalCount = list.TotalCount,
                pageSize = list.PageSize,
                pageIndex = list.PageIndex,
                totalPages = list.TotalPages
            };
            Response.Headers.Append("x-pagination", JsonHelper.ToJson(paginationMetadata));

            //映射成DTO
            var resultDto = _mapper.Map<IEnumerable<SiteChannelsDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/channel/
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Channel", ActionType.Add)]
        public async Task<IActionResult> Add([FromBody] SiteChannelsEditDto modelDto)
        {
            var model = await _channelService.AddAsync(modelDto);
            var result = _mapper.Map<SiteChannelsDto>(model);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/admin/channel/1
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Channel", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SiteChannelsEditDto modelDto)
        {
            await _channelService.UpdateAsync(id, modelDto);
            return NoContent();
        }

        /// <summary>
        /// 局部更新一条记录
        /// 示例：/admin/channel/1
        /// Body：[{"op":"replace","path":"/title","value":"new title"}]
        /// </summary>
        [HttpPatch("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Channel", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] JsonPatchDocument<SiteChannelsEditDto> patchDocument)
        {
            //注意：要使用写的数据库进行查询，才能正确写入数据主库
            var model = await _channelService.QueryAsync<SiteChannels>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"频道{id}不存在或已删除");
            var modelToPatch = _mapper.Map<SiteChannelsEditDto>(model);
            patchDocument.ApplyTo(modelToPatch, ModelState);
            //验证数据是否合法
            if (!TryValidateModel(modelToPatch))
            {
                return ValidationProblem(ModelState);
            }
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelToPatch, model);
            await _channelService.SaveAsync();

            //手动删除缓存
            await _channelService.RemoveCacheAsync<SiteChannels>(true);

            return NoContent();
        }

        /// <summary>
        /// 删除一条记录(级联数据)
        /// 示例：/admin/channel/1
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Channel", ActionType.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            //应还要检查频道下是否有文章，有则删除失败
            var result = await _channelService.DeleteAsync(x => x.Id == id);
            return NoContent();
        }

        /// <summary>
        /// 批量删除记录(级联数据)
        /// 示例：/admin/channel?ids=1,2,3
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Channel", ActionType.Delete)]
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
            //执行删除操作
            var result = await _channelService.DeleteAsync(x => arrIds.Contains(x.Id));
            return NoContent();
        }

        /// <summary>
        /// 获取频道扩展字段
        /// 示例：/admin/channel/1/field
        /// </summary>
        [HttpGet("{id}/field")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Channel", ActionType.View)]
        public async Task<IActionResult> GetFieldList([FromRoute] int id, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<SiteChannelFieldsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<SiteChannelFieldsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据库列表
            var resultFrom = await _channelService.QueryListAsync<SiteChannelFields>(0,
                x => x.ChannelId == id,
                null,
                searchParam.OrderBy ?? "SortId,Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<SiteChannelFieldsDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }
        #endregion

        #region 前台调用接口============================
        /// <summary>
        /// 根据ID获取频道(缓存)
        /// 示例：/client/channel/1
        /// </summary>
        [HttpGet("/client/channel/{channelId}")]
        public async Task<IActionResult> ClientGetById([FromRoute] int channelId, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<SiteChannelsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //查询数据库获取实体
            var model = await _channelService.QueryAsync<SiteChannels>(channelId.ToString(), x => x.Id == channelId)
                ?? throw new ResponseException($"频道{channelId}不存在或已删除");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<SiteChannelsDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取指定数量列表(缓存)
        /// 示例：/client/channel/view/0
        /// </summary>
        [HttpGet("/client/channel/view/{top}")]
        public async Task<IActionResult> ClientGetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<SiteChannelsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<SiteChannelsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取缓存Key
            var cacheKey = $"{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据库列表
            var resultFrom = await _channelService.QueryListAsync<SiteChannels>(cacheKey, top,
                x => (searchParam.SiteId <= 0 || x.SiteId == searchParam.SiteId)
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                null,
                searchParam.OrderBy ?? "SortId,AddTime");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<SiteChannelsDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取频道扩展字段(缓存)
        /// 示例：/client/channel/1/field
        /// </summary>
        [HttpGet("/client/channel/{id}/field")]
        public async Task<IActionResult> ClientGetFieldList([FromRoute] int id, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<SiteChannelFieldsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<SiteChannelFieldsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取缓存Key
            var cacheKey = $"{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据库列表
            var resultFrom = await _channelService.QueryListAsync<SiteChannelFields>(cacheKey, 0,
                x => x.ChannelId == id,
                null,
                searchParam.OrderBy ?? "SortId,Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<SiteChannelFieldsDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }
        #endregion
    }
}