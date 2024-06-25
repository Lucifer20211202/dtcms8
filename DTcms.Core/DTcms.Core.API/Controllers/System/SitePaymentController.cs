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
    /// 站点支付方式接口
    /// </summary>
    [Route("admin/site/payment")]
    [ApiController]
    public class SitePaymentsController(ISitePaymentService sitePaymentService, IUserService userService, IMapper mapper) : ControllerBase
    {
        private readonly ISitePaymentService _sitePaymentService = sitePaymentService;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取数据
        /// 示例：/admin/site/payment/1
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Payment", ActionType.View)]
        public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] PaymentParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<SitePaymentsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _sitePaymentService.QueryAsync<SitePayments>(x => x.Id == id,
                query => query.Include(p => p.Payment).Include(p => p.Site),
                WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");
            //根据字段进行塑形
            var result = _mapper.Map<SitePaymentsDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取列表
        /// 示例：/admin/site/payment/view/10
        /// </summary>
        [HttpGet("view/{top}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Payment", ActionType.View)]
        public async Task<IActionResult> GetList([FromRoute] int top, [FromQuery] PaymentParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<SitePaymentsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<SitePaymentsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //将接口类型转换成IEnumerable
            var listTypes = searchParam.Types.ToIEnumerable<string>();

            //获取数据库列表
            var list = await _sitePaymentService.QueryListAsync<SitePayments>(top,
                x => (searchParam.SiteId <= 0 || x.SiteId == searchParam.SiteId)
                && (listTypes == null || listTypes.Contains(x.Type))
                && (searchParam.Status < 0 || (x.Payment != null && x.Payment.Status == searchParam.Status))
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                query => query.Include(p => p.Payment).Include(p => p.Site),
                searchParam.OrderBy ?? "AddTime");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<SitePaymentsDto>>(list).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取分页列表
        /// 示例：/admin/site/payment?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Payment", ActionType.View)]
        public async Task<IActionResult> GetList([FromQuery] PaymentParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.TrimStart('-').IsPropertyExists<SitePaymentsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<SitePaymentsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //将接口类型转换成IEnumerable
            var listTypes = searchParam.Types.ToIEnumerable<string>();

            //获取数据列表，如果站点ID大于0则查询该站点下所有的列表
            var list = await _sitePaymentService.QueryPageAsync<SitePayments>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => (searchParam.SiteId <= 0 || x.SiteId == searchParam.SiteId)
                && (listTypes == null || listTypes.Contains(x.Type))
                && (searchParam.Status < 0 || (x.Payment != null && x.Payment.Status == searchParam.Status))
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                query => query.Include(p => p.Payment).Include(p => p.Site),
                searchParam.OrderBy ?? "-AddTime,-Id");

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
            var resultDto = _mapper.Map<IEnumerable<SitePaymentsDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/site/payment
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Payment", ActionType.Add)]
        public async Task<IActionResult> Add([FromBody] SitePaymentsEditDto modelDto)
        {
            //映射成实体
            var model = _mapper.Map<SitePayments>(modelDto);
            //查找用户名
            model.AddBy = _userService.GetUserName();
            model.AddTime = DateTime.Now;
            //写入数据库
            await _sitePaymentService.AddAsync(model);
            //映射成DTO再返回，否则出错
            var result = _mapper.Map<SitePaymentsDto>(model);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/admin/site/payment/1
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Payment", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SitePaymentsEditDto modelDto)
        {
            //查找记录
            var model = await _sitePaymentService.QueryAsync<SitePayments>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelDto, model);
            var result = await _sitePaymentService.SaveAsync();

            //因为没有调用方法，所以要手动删除缓存
            if (result)
            {
                await _sitePaymentService.RemoveCacheAsync<SitePayments>(true);
            }

            return NoContent();
        }

        /// <summary>
        /// 局部更新一条记录
        /// 示例：/admin/site/payment/1
        /// Body：[{"op":"replace","path":"/title","value":"new title"}]
        /// </summary>
        [HttpPatch("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Payment", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] JsonPatchDocument<SitePaymentsEditDto> patchDocument)
        {
            var model = await _sitePaymentService.QueryAsync<SitePayments>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            var modelToPatch = _mapper.Map<SitePaymentsEditDto>(model);
            patchDocument.ApplyTo(modelToPatch, ModelState);
            //验证数据是否合法
            if (!TryValidateModel(modelToPatch))
            {
                return ValidationProblem(ModelState);
            }
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelToPatch, model);
            var result = await _sitePaymentService.SaveAsync();

            //因为没有调用方法，所以要手动删除缓存
            if (result)
            {
                await _sitePaymentService.RemoveCacheAsync<SitePayments>(true);
            }

            return NoContent();
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/site/payment/1
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Payment", ActionType.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            //检查参数是否正确
            if (!await _sitePaymentService.ExistsAsync<SitePayments>(x => x.Id == id))
            {
                throw new ResponseException($"数据[{id}]不存在或无权删除");
            }
            var result = await _sitePaymentService.DeleteAsync<SitePayments>(x => x.Id == id);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录
        /// 示例：//admin/site/payment?ids=1,2,3
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Payment", ActionType.Delete)]
        public async Task<IActionResult> DeleteByIds([FromQuery] string Ids)
        {
            //检查参数是否为空
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var listIds = Ids.ToIEnumerable<int>() ?? throw new ResponseException("传输参数不符合规范");
            //执行批量删除操作
            await _sitePaymentService.DeleteAsync<SitePayments>(x => listIds.Contains(x.Id));
            return NoContent();
        }
        #endregion

        #region 前台调用接口============================
        /// <summary>
        /// 获取列表(缓存)
        /// 示例：/client/payment/view/10
        /// </summary>
        [HttpGet("/client/payment/view/{top}")]
        public async Task<IActionResult> ClientGetList([FromRoute] int top, [FromQuery] PaymentParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<SitePaymentsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<SitePaymentsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //将接口类型转换成IEnumerable
            var listTypes = searchParam.Types.ToIEnumerable<string>();

            //获取缓存Key
            var cacheKey = $"{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据库列表
            var resultFrom = await _sitePaymentService.QueryListAsync<SitePayments>(cacheKey, top,
                x => x.Payment != null
                && x.Payment.Status == 0
                && (searchParam.SiteId <= 0 || x.SiteId == searchParam.SiteId)
                && (listTypes == null || listTypes.Contains(x.Type))
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                query => query.Include(p => p.Payment).Include(p => p.Site),
                searchParam.OrderBy ?? "AddTime");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<SitePaymentsClientDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }
        #endregion
    }
}