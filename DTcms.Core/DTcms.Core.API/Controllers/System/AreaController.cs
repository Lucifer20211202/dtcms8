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
    /// 省市区接口
    /// </summary>
    [Route("admin/area")]
    [ApiController]
    public class AreaController(IAreaService areaService, IMapper mapper) : ControllerBase
    {
        private readonly IAreaService _areaService = areaService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取地区
        /// 示例：/admin/area/1
        /// </summary>
        [HttpGet("{areaId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Area", ActionType.View)]
        public async Task<IActionResult> GetById([FromRoute] int areaId, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<AreasDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _areaService.QueryAsync<Areas>(x => x.Id == areaId, null, WriteRoRead.Write);
            if (model == null)
            {
                throw new ResponseException($"数据[{areaId}]不存在或已删除");
            }
            //使用AutoMapper转换成ViewModel
            //根据字段进行塑形
            var result = _mapper.Map<AreasDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 根据父节点获取一级列表
        /// 示例：/admin/area/view/0
        /// </summary>
        [HttpGet("view/{top}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Area", ActionType.View)]
        public async Task<IActionResult> GetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<AreasDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<AreasDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //获取一条数据
            var parentId = 0;
            if (searchParam.Keyword.IsNotNullOrEmpty())
            {
                var model = await _areaService.QueryAsync<Areas>(x => x.Title != null && x.Title.Equals(searchParam.Keyword));
                parentId = model != null ? model.Id : -1;
            }
            //获取数据库列表
            var resultFrom = await _areaService.QueryListAsync<Areas>(top,
                x => x.ParentId == parentId,
                null,
                searchParam.OrderBy ?? "SortId,Id");
            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<AreasDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取地区树目录列表
        /// 示例：/admin/area/
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Area", ActionType.View)]
        public async Task<IActionResult> GetList([FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<AreasDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //如果有查询关健字
            var parentId = 0; //父节点ID
            if (!string.IsNullOrWhiteSpace(param.Keyword))
            {
                var model = await _areaService.QueryAsync<Areas>(x => (x.Title != null && x.Title.Contains(param.Keyword))) ?? throw new ResponseException("暂无查询记录");
                parentId = model.Id;
            }
            //获取数据库列表
            var resultFrom = await _areaService.QueryListAsync(parentId);
            //使用AutoMapper转换成ViewModel
            //根据字段进行塑形
            var resultDto = resultFrom.AsEnumerable().ShapeData(param.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/area/
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Area", ActionType.Add)]
        public async Task<IActionResult> Add([FromBody] AreasEditDto modelDto)
        {
            //映射成实体
            var model = _mapper.Map<Areas>(modelDto);
            //写入数据库
            await _areaService.AddAsync(model);
            //映射成DTO再返回，否则出错
            var result = _mapper.Map<AreasDto>(model);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/admin/area/1
        /// </summary>
        [HttpPut("{areaId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Area", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int areaId, [FromBody] AreasEditDto modelDto)
        {
            //查找记录
            var model = await _areaService.QueryAsync<Areas>(x => x.Id == areaId, null, WriteRoRead.Write)
                ?? throw new ResponseException($"地区{areaId}不存在或已删除");
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelDto, model);
            var result = await _areaService.SaveAsync();
            //由于没有调用方法，手动清空缓存
            if (result)
            {
                await _areaService.RemoveCacheAsync<Areas>(true);
            }
            return NoContent();
        }

        /// <summary>
        /// 局部更新一条记录
        /// 示例：/admin/area/1
        /// Body：[{"op":"replace","path":"/title","value":"new title"}]
        /// </summary>
        [HttpPatch("{areaId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Area", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int areaId, [FromBody] JsonPatchDocument<AreasEditDto> patchDocument)
        {
            var model = await _areaService.QueryAsync<Areas>(x => x.Id == areaId, null, WriteRoRead.Write);
            if (model == null)
            {
                throw new ResponseException($"数据{areaId}不存在或已删除");
            }

            var modelToPatch = _mapper.Map<AreasEditDto>(model);
            patchDocument.ApplyTo(modelToPatch, ModelState);
            //验证数据是否合法
            if (!TryValidateModel(modelToPatch))
            {
                return ValidationProblem(ModelState);
            }
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelToPatch, model);
            var result = await _areaService.SaveAsync();

            //由于没有调用方法，手动清空缓存
            if (result)
            {
                await _areaService.RemoveCacheAsync<Areas>(true);
            }

            return NoContent();
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/area/1
        /// </summary>
        [HttpDelete("{areaId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Area", ActionType.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int areaId)
        {
            if (!await _areaService.ExistsAsync<Areas>(x => x.Id == areaId))
            {
                throw new ResponseException($"数据[{areaId}]不存在或已删除");
            }
            var result = await _areaService.DeleteAsync(x => x.Id == areaId);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录(级联数据)
        /// 示例：/admin/area?ids=1,2,3
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Area", ActionType.Delete)]
        public async Task<IActionResult> DeleteByIds([FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var areaIds = Ids.ToIEnumerable<int>();
            if (areaIds == null)
            {
                throw new ResponseException("传输参数不符合规范");
            }
            //执行批量删除操作
            await _areaService.DeleteAsync(x => areaIds.Contains(x.Id));

            return NoContent();
        }

        /// <summary>
        /// 批量导入数据
        /// 示例：/admin/area/import
        /// </summary>
        [HttpPost("import")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Area", ActionType.Add)]
        public async Task<IActionResult> Import([FromBody] AreasImportEditDto modelDto)
        {
            var list = JsonHelper.ToJson<List<AreasImportDto>>(modelDto.JsonData) ?? throw new ResponseException("数据格式有误，请检查重试");
            await _areaService.ImportAsync(list);
            return NoContent();
        }
        #endregion

        #region 前台调用接口============================
        /// <summary>
        /// 根据父节点获取一级列表(缓存)
        /// 示例：/client/area/view/0
        /// </summary>
        [HttpGet("/client/area/view/{top}")]
        public async Task<IActionResult> ClientGetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<AreasDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<AreasDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取缓存Key
            var cacheKey = $"{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取一条数据
            var parentId = 0;
            if (searchParam.Keyword.IsNotNullOrEmpty())
            {
                var model = await _areaService.QueryAsync<Areas>(x => x.Title != null && x.Title.Equals(searchParam.Keyword));
                parentId = model != null ? model.Id : -1;
            }
            //获取数据库列表
            var resultFrom = await _areaService.QueryListAsync<Areas>(cacheKey, top,
                x => x.ParentId == parentId,
                null,
                searchParam.OrderBy ?? "SortId,Id");
            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<AreasDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取地区树目录列表(缓存)
        /// 示例：/client/area
        /// </summary>
        [HttpGet("/client/area")]
        public async Task<IActionResult> ClientGetList([FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<AreasDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //如果有查询关健字
            var parentId = 0; //父节点ID
            if (!string.IsNullOrWhiteSpace(param.Keyword))
            {
                var model = await _areaService.QueryAsync<Areas>(x => (x.Title != null && x.Title.Contains(param.Keyword)))
                    ?? throw new ResponseException("暂无查询记录");
                parentId = model.Id;
            }

            //获取缓存Key
            var cacheKey = $"{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据库列表
            var resultFrom = await _areaService.QueryListAsync(cacheKey, parentId);
            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var resultDto = resultFrom.AsEnumerable().ShapeData(param.Fields);
            //返回成功200
            return Ok(resultDto);
        }
        #endregion
    }
}