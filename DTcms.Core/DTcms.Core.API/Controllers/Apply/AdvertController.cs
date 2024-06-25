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
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 广告位
    /// </summary>
    [Route("admin/advert")]
    [ApiController]
    public class AdvertController(IAdvertService advertService, IUserService userService, IMapper mapper) : ControllerBase
    {
        private readonly IAdvertService _advertService = advertService;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取数据
        /// 示例：/admin/advert/1
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Advert", ActionType.View)]
        public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<AdvertsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            //查询数据库获取实体
            var model = await _advertService.QueryAsync<Adverts>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<AdvertsDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取指定数量列表
        /// 示例：/admin/advert/view/0
        /// </summary>
        [HttpGet("view/{top}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Advert", ActionType.View)]
        public async Task<IActionResult> GetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.TrimStart('-').IsPropertyExists<AdvertsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<AdvertsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据库列表
            var list = await _advertService.QueryListAsync<Adverts>(top,
                x => (string.IsNullOrEmpty(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                null,
                searchParam.OrderBy ?? "SortId,Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<AdvertsDto>>(list).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取数据列表
        /// 示例：/admin/advert?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Advert", ActionType.View)]
        public async Task<IActionResult> GetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.TrimStart('-').IsPropertyExists<AdvertsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<AdvertsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表
            var list = await _advertService.QueryPageAsync<Adverts>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => (string.IsNullOrEmpty(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                null,
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

            //映射成DTO
            var resultDto = _mapper.Map<IEnumerable<AdvertsDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/advert
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Advert", ActionType.Add)]
        public async Task<IActionResult> Add([FromBody] AdvertsEditDto modelDto)
        {
            //映射成实体
            var model = _mapper.Map<Adverts>(modelDto);
            //获取当前用户名
            model.AddBy = _userService.GetUserName();
            model.AddTime = DateTime.Now;
            //写入数据库
            await _advertService.AddAsync<Adverts>(model);
            //映射成DTO再返回，否则出错
            var result = _mapper.Map<AdvertsDto>(model);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/admin/advert/1
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Advert", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] AdvertsEditDto modelDto)
        {
            //查找记录
            var model = await _advertService.QueryAsync<Adverts>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"记录[{id}]不存在或已删除");

            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelDto, model);
            var result = await _advertService.SaveAsync();

            //由于没有调用方法，手动清空缓存
            if (result)
            {
                await _advertService.RemoveCacheAsync<Adverts>(true);
            }

            return NoContent();
        }

        /// <summary>
        /// 局部更新一条记录
        /// 示例：/admin/advert/1
        /// Body：[{"op":"replace","path":"/title","value":"new title"}]
        /// </summary>
        [HttpPatch("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Advert", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] long id, [FromBody] JsonPatchDocument<AdvertsEditDto> patchDocument)
        {
            var model = await _advertService.QueryAsync<Adverts>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"记录[{id}]不存在或已删除");

            var modelToPatch = _mapper.Map<AdvertsEditDto>(model);
            patchDocument.ApplyTo(modelToPatch, ModelState);
            //验证数据是否合法
            if (!TryValidateModel(modelToPatch))
            {
                return ValidationProblem(ModelState);
            }
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelToPatch, model);
            var result = await _advertService.SaveAsync();

            //由于没有调用方法，手动清空缓存
            if (result)
            {
                await _advertService.RemoveCacheAsync<Adverts>(true);
            }

            return NoContent();
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/advert/1
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Advert", ActionType.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!await _advertService.ExistsAsync<Adverts>(x => x.Id == id, WriteRoRead.Write))
            {
                throw new ResponseException($"记录[{id}]不存在或已删除");
            }
            var result = await _advertService.DeleteAsync<Adverts>(x => x.Id == id);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录(级联数据)
        /// 示例：/admin/advert?ids=1,2,3
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Advert", ActionType.Delete)]
        public async Task<IActionResult> DeleteByIds([FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var ids = Ids.ToIEnumerable<int>();
            if (ids == null)
            {
                throw new ResponseException("传输参数不符合规范");
            }
            //执行批量删除操作
            await _advertService.DeleteAsync<Adverts>(x => ids.Contains(x.Id));

            return NoContent();
        }
        #endregion

        #region 前台调用接口============================
        /// <summary>
        /// 根据ID或标识获取数据(缓存)
        /// 示例：/client/advert/1
        /// </summary>
        [HttpGet("/client/advert/{indexKey}")]
        public async Task<IActionResult> ClientGetById([FromRoute] string indexKey, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<AdvertsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            Adverts? model = null;
            if (int.TryParse(indexKey, out int advertId))
            {
                model = await _advertService.QueryAsync(indexKey,
                    x => x.Id == advertId,
                    b => b.Status == 1
                    && b.StartTime != null
                    && b.EndTime != null
                    && DateTime.Compare(b.StartTime.Value, DateTime.Now) <= 0
                    && DateTime.Compare(b.EndTime.Value, DateTime.Now) >= 0);
            }
            if (model == null)
            {
                model = await _advertService.QueryAsync(indexKey,
                    x => x.CallIndex == indexKey,
                    b => b.Status == 1
                    && b.StartTime != null
                    && b.EndTime != null
                    && DateTime.Compare(b.StartTime.Value, DateTime.Now) <= 0
                    && DateTime.Compare(b.EndTime.Value, DateTime.Now) >= 0);
            }
            if (model == null)
            {
                throw new ResponseException("广告已失效或不存在", ErrorCode.NotFound);
            }
            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<AdvertsDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取指定数量列表(缓存)
        /// 示例：/client/advert/view/10
        /// </summary>
        [HttpGet("/client/advert/view/{top}")]
        public async Task<IActionResult> ClientGetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<AdvertsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<AdvertsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取缓存Key
            var cacheKey = $"{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据库列表
            var resultFrom = await _advertService.QueryListAsync<Adverts>(cacheKey, top,
                //判断条件
                x => (x.SiteId <= 0 || x.SiteId == searchParam.SiteId)
                && (string.IsNullOrEmpty(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword)))
                && x.Banners.Any(b => b.Status == 1
                && b.StartTime != null
                && b.EndTime != null
                && DateTime.Compare(b.StartTime.Value, DateTime.Now) <= 0
                && DateTime.Compare(b.EndTime.Value, DateTime.Now) >= 0),
                //加载子集合
                query => query.Include(x => x.Banners),
                searchParam.OrderBy ?? "SortId,-Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<AdvertsDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }
        #endregion
    }
}