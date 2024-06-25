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
    /// 站点接口
    /// </summary>
    [Route("admin/site")]
    [ApiController]
    public class SiteController(ISiteService siteService, IMapper mapper) : ControllerBase
    {
        private readonly ISiteService _siteService = siteService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取站点
        /// 示例：/admin/site/1
        /// </summary>
        [HttpGet("{siteId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Site", ActionType.View)]
        public async Task<IActionResult> GetById([FromRoute] int siteId, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<SitesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _siteService.QueryAsync<Sites>(x => x.Id == siteId,
                query => query.Include(s => s.Domains), WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{siteId}]不存在或已删除");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<SitesDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取总记录数量
        /// 示例：/admin/site/view/count
        /// </summary>
        [HttpGet("view/count")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Site", ActionType.View)]
        public async Task<IActionResult> GetCount([FromQuery] BaseParameter searchParam)
        {
            var result = await _siteService.CountAsync<Sites>(x => searchParam.Status < 0 || x.Status == searchParam.Status);
            //返回成功200
            return Ok(result);
        }

        /// <summary>
        /// 获取指定数量列表
        /// 示例：/admin/site/view/0
        /// </summary>
        [HttpGet("view/{top}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Site", ActionType.View)]
        public async Task<IActionResult> GetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<SitesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<SitesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据库列表
            var resultFrom = await _siteService.QueryListAsync<Sites>(top,
                x => (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                query => query.Include(s => s.Domains),
                searchParam.OrderBy ?? "SortId,Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<SitesDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取站点列表
        /// 示例：/admin/site?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Site", ActionType.View)]
        public async Task<IActionResult> GetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null && !searchParam.OrderBy.TrimStart('-').IsPropertyExists<SitesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<SitesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表
            var list = await _siteService.QueryPageAsync<Sites>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => (string.IsNullOrWhiteSpace(searchParam.Keyword) || x.Title != null && x.Title.Contains(searchParam.Keyword)),
                query => query.Include(s => s.Domains),
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

            //映射成DTO
            var resultDto = _mapper.Map<IEnumerable<SitesDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/site/
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Site", ActionType.Add)]
        public async Task<IActionResult> Add([FromBody] SitesEditDto modelDto)
        {
            var model = await _siteService.AddAsync(modelDto);
            var mapModel = _mapper.Map<SitesDto>(model);
            return Ok(mapModel);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/admin/site/1
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Site", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SitesEditDto modelDto)
        {
            await _siteService.UpdateAsync(id, modelDto);
            return NoContent();
        }

        /// <summary>
        /// 局部更新一条记录
        /// 示例：/admin/site/1
        /// Body：[{"op":"replace","path":"/title","value":"new title"}]
        /// </summary>
        [HttpPatch("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Site", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] JsonPatchDocument<SitesEditDto> patchDocument)
        {
            //注意：要使用写的数据库进行查询，才能正确写入数据主库
            var model = await _siteService.QueryAsync<Sites>(x => x.Id == id,
                query => query.Include(s => s.Domains),
                WriteRoRead.Write) ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            var modelToPatch = _mapper.Map<SitesEditDto>(model);
            patchDocument.ApplyTo(modelToPatch, ModelState);

            //验证数据是否合法
            if (!TryValidateModel(modelToPatch))
            {
                return ValidationProblem(ModelState);
            }
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelToPatch, model);
            await _siteService.SaveAsync();

            //删除缓存(列表和详情)
            await _siteService.RemoveCacheAsync<Sites>(true);

            return NoContent();
        }

        /// <summary>
        /// 删除一条记录(级联数据)
        /// 示例：/admin/site/1
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Site", ActionType.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(!await _siteService.ExistsAsync<Sites>(x => x.Id == id))
            {
                throw new ResponseException($"数据[{id}]不存在或已删除");
            }

            await _siteService.DeleteAsync(x => x.Id == id);
            return NoContent();
        }

        /// <summary>
        /// 批量删除记录(级联数据)
        /// 示例：/admin/site?ids=1,2,3
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Site", ActionType.Delete)]
        public async Task<IActionResult> DeleteByIds([FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var arrIds = Ids.ToIEnumerable<int>() ?? throw new ResponseException("传输参数不符合规范");

            //执行删除操作
            var result = await _siteService.DeleteAsync(x => arrIds.Contains(x.Id));
            return NoContent();
        }
        #endregion

        #region 前台调用接口============================
        /// <summary>
        /// 根据ID或名称获取站点信息(缓存)
        /// 示例：/client/site/1
        /// </summary>
        [HttpGet("/client/site/{siteKey}")]
        public async Task<IActionResult> GetClientById([FromRoute] string siteKey, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<SitesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //查询数据库获取实体
            Sites? model = null;
            if (int.TryParse(siteKey, out int siteId))
            {
                model = await _siteService.QueryAsync<Sites>(siteKey,
                    x => x.Id == siteId,
                    query => query.Include(s => s.Domains));
            }
            if (model == null)
            {
                model = await _siteService.QueryAsync<Sites>(siteKey,
                    x => x.Name == siteKey,
                    query => query.Include(s => s.Domains));
            }
            //查询数据库获取实体
            if (model == null)
            {
                throw new ResponseException($"数据{siteKey}不存在或已删除");
            }
            //使用AutoMapper转换成ViewModel
            //根据字段进行塑形
            var result = _mapper.Map<SitesDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 根据域名获取站点信息(缓存)
        /// 示例：/client/site/domain?host=域名
        /// </summary>
        [HttpGet("/client/site/domain")]
        public async Task<IActionResult> GetClientByDomain([FromQuery] string host, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<SitesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //查询数据库获取实体
            Sites? model = await _siteService.QueryAsync<Sites>(host,
                    x => x.Domains.Any(d => d.Domain != null && d.Domain.Equals(host)) || x.IsDefault == 1,
                    query => query.Include(s => s.Domains));
            //查询数据库获取实体
            if (model == null)
            {
                throw new ResponseException($"站点{host}不存在或已删除");
            }
            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<SitesDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取指定数量列表(缓存)
        /// 示例：/client/site/view/0
        /// </summary>
        [HttpGet("/client/site/view/{top}")]
        public async Task<IActionResult> GetClientList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<SitesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<SitesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取缓存Key
            var cacheKey = $"{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据库列表
            var resultFrom = await _siteService.QueryListAsync<Sites>(cacheKey, top,
                x => (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                query => query.Include(s => s.Domains),
                searchParam.OrderBy ?? "SortId,Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<SitesDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }
        #endregion

    }
}