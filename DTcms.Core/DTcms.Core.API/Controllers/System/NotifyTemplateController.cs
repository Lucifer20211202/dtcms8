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
    /// 消息模板
    /// </summary>
    [Route("admin/notify/template")]
    [ApiController]
    public class NotifyTemplateController(INotifyTemplateService notifyTemplateService, IMapper mapper) : ControllerBase
    {
        private readonly INotifyTemplateService _notifyTemplateService = notifyTemplateService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 获取分页列表
        /// 示例：/admin/notify/template?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("NotifyTemplate", ActionType.View)]
        public async Task<IActionResult> GetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.TrimStart('-').IsPropertyExists<NotifyTemplatesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<NotifyTemplatesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表
            var list = await _notifyTemplateService.QueryPageAsync<NotifyTemplates>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                null,
                searchParam.OrderBy ?? "-Id");

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
            var resultDto = _mapper.Map<IEnumerable<NotifyTemplatesDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 根据ID获取数据
        /// 示例：/admin/notify/template/1
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("NotifyTemplate", ActionType.View)]
        public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<NotifyTemplatesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _notifyTemplateService.QueryAsync<NotifyTemplates>(x => x.Id == id, null, WriteRoRead.Write);
            if (model == null)
            {
                throw new ResponseException($"数据[{id}]不存在或已删除");
            }
            //使用AutoMapper转换成ViewModel
            //根据字段进行塑形
            var result = _mapper.Map<NotifyTemplatesDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 根据别名获取数据
        /// </summary>
        [HttpGet("{type}/{callIndex}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("NotifyTemplate", ActionType.View)]
        public async Task<IActionResult> GetCallIndex([FromRoute] int type, [FromRoute] string callIndex, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<NotifyTemplatesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _notifyTemplateService.QueryAsync<NotifyTemplates>(
                x => x.Type == type
                && x.CallIndex != null
                && x.CallIndex.Equals(callIndex, StringComparison.CurrentCultureIgnoreCase),
                null,
                WriteRoRead.Write);
            if (model == null)
            {
                throw new ResponseException($"数据{callIndex}不存在或已删除");
            }
            //根据字段进行塑形
            var result = _mapper.Map<NotifyTemplatesDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/notify/template
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("NotifyTemplate", ActionType.Add)]
        public async Task<IActionResult> Add([FromBody] NotifyTemplatesEditDto modelDto)
        {
            //映射成实体
            var model = _mapper.Map<NotifyTemplates>(modelDto);
            //写入数据库
            await _notifyTemplateService.AddAsync(model);
            //映射成DTO再返回，否则出错
            var result = _mapper.Map<NotifyTemplatesDto>(model);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/admin/notify/template/1
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("NotifyTemplate", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] NotifyTemplatesEditDto modelDto)
        {
            //查找记录
            var model = await _notifyTemplateService.QueryAsync<NotifyTemplates>(x => x.Id == id)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            model.UpdateTime = DateTime.Now; //更新时间
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelDto, model);
            var result = await _notifyTemplateService.SaveAsync();
            //由于没有调用方法，手动清空缓存
            if (result)
            {
                await _notifyTemplateService.RemoveCacheAsync<NotifyTemplates>(true);
            }
            return NoContent();
        }

        /// <summary>
        /// 局部更新一条记录
        /// 示例：/admin/notify/template/1
        /// Body：[{"op":"replace","path":"/title","value":"new title"}]
        /// </summary>
        [HttpPatch("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("NotifyTemplate", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] JsonPatchDocument<NotifyTemplatesEditDto> patchDocument)
        {
            var model = await _notifyTemplateService.QueryAsync<NotifyTemplates>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");
            var modelToPatch = _mapper.Map<NotifyTemplatesEditDto>(model);
            patchDocument.ApplyTo(modelToPatch, ModelState);
            //验证数据是否合法
            if (!TryValidateModel(modelToPatch))
            {
                return ValidationProblem(ModelState);
            }
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelToPatch, model);
            var result = await _notifyTemplateService.SaveAsync();
            //由于没有调用方法，手动清空缓存
            if (result)
            {
                await _notifyTemplateService.RemoveCacheAsync<NotifyTemplates>(true);
            }
            return NoContent();
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/notify/template/1
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("NotifyTemplate", ActionType.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            //系统默认不允许删除
            if (!await _notifyTemplateService.ExistsAsync<NotifyTemplates>(x => x.Id == id && x.IsSystem == 0))
            {
                throw new ResponseException($"数据[{id}]不存在或无权删除");
            }
            var result = await _notifyTemplateService.DeleteAsync<NotifyTemplates>(x => x.Id == id);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录(级联数据)
        /// 示例：/admin/notify/template?ids=1,2,3
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("NotifyTemplate", ActionType.Delete)]
        public async Task<IActionResult> DeleteByIds([FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var listIds = Ids.ToIEnumerable<int>();
            if (listIds == null)
            {
                throw new ResponseException("传输参数不符合规范");
            }
            //执行批量删除操作
            await _notifyTemplateService.DeleteAsync<NotifyTemplates>(x => listIds.Contains(x.Id) && x.IsSystem == 0);

            return NoContent();
        }
        #endregion
    }
}