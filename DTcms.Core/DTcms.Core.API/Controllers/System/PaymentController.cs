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
    /// 支付方式接口
    /// </summary>
    [Route("admin/payment")]
    [ApiController]
    public class PaymentController(IPaymentService paymentService, IMapper mapper) : ControllerBase
    {
        private readonly IPaymentService _paymentService = paymentService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 获取列表
        /// 示例：/admin/payment/view/10
        /// </summary>
        [HttpGet("view/{top}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Payment", ActionType.View)]
        public async Task<IActionResult> GetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<PaymentsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<PaymentsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //获取数据库列表
            var resultFrom = await _paymentService.QueryListAsync<Payments>(top,
                x => x.Status == 0
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                null,
                searchParam.OrderBy ?? "SortId,Id");
            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<PaymentsDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取分页列表
        /// 示例：/admin/payment?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Payment", ActionType.View)]
        public async Task<IActionResult> GetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.TrimStart('-').IsPropertyExists<PaymentsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<PaymentsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表
            var list = await _paymentService.QueryPageAsync<Payments>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword)),
                null,
                searchParam.OrderBy ?? "Id");

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
            var resultDto = _mapper.Map<IEnumerable<PaymentsDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 根据ID获取数据
        /// 示例：/admin/payment/1
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Payment", ActionType.View)]
        public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<PaymentsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _paymentService.QueryAsync<Payments>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<PaymentsDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/payment/
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Payment", ActionType.Add)]
        public async Task<IActionResult> Add([FromBody] PaymentsEditDto modelDto)
        {
            //映射成实体
            var model = _mapper.Map<Payments>(modelDto);
            //写入数据库
            await _paymentService.AddAsync(model);
            //映射成DTO再返回，否则出错
            var result = _mapper.Map<PaymentsDto>(model);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/admin/payment/1
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Payment", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PaymentsEditDto modelDto)
        {
            //查找记录
            var model = await _paymentService.QueryAsync<Payments>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据{id}不存在或已删除");

            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelDto, model);
            var result = await _paymentService.SaveAsync();

            //手动删除缓存
            if (result)
            {
                await _paymentService.RemoveCacheAsync<Payments>(true);
            }

            return NoContent();
        }

        /// <summary>
        /// 局部更新一条记录
        /// 示例：/admin/payment/1
        /// Body：[{"op":"replace","path":"/title","value":"new title"}]
        /// </summary>
        [HttpPatch("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Payment", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] JsonPatchDocument<PaymentsEditDto> patchDocument)
        {
            var model = await _paymentService.QueryAsync<Payments>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据{id}不存在或已删除");

            var modelToPatch = _mapper.Map<PaymentsEditDto>(model);
            patchDocument.ApplyTo(modelToPatch, ModelState);
            //验证数据是否合法
            if (!TryValidateModel(modelToPatch))
            {
                return ValidationProblem(ModelState);
            }
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelToPatch, model);
            var result = await _paymentService.SaveAsync();

            //手动删除缓存
            if (result)
            {
                await _paymentService.RemoveCacheAsync<Payments>(true);
            }

            return NoContent();
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/payment/1
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Payment", ActionType.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!await _paymentService.ExistsAsync<Payments>(x => x.Id == id))
            {
                throw new ResponseException($"数据[{id}]不存在或已删除");
            }
            var result = await _paymentService.DeleteAsync(x => x.Id == id);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录(级联数据)
        /// 示例：/admin/payment?ids=1,2,3
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Payment", ActionType.Delete)]
        public async Task<IActionResult> DeleteByIds([FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var listIds = Ids.ToIEnumerable<int>() ?? throw new ResponseException("传输参数不符合规范");
            //执行批量删除操作
            await _paymentService.DeleteAsync(x => listIds.Contains(x.Id));

            return NoContent();
        }
        #endregion
    }
}