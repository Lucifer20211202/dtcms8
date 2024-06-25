using AutoMapper;
using DTcms.Core.API.Filters;
using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Extensions;
using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 订单收款
    /// </summary>
    [Route("admin/order/payment")]
    [ApiController]
    public class OrderPaymentController(IOrderPaymentService orderPaymentService, IUserService userService, IMapper mapper) : ControllerBase
    {
        private readonly IOrderPaymentService _orderPaymentService = orderPaymentService;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取数据
        /// 示例：/admin/order/payment/1
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("OrderPayment", ActionType.View)]
        public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<OrderPaymentsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _orderPaymentService.QueryAsync<OrderPayments>(x => x.Id == id,
                query => query.Include(x => x.User), 
                WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<OrderPaymentsDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取指定数量列表
        /// 示例：/admin/order/payment/view/10
        /// </summary>
        [HttpGet("view/{top}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("OrderPayment", ActionType.View)]
        public async Task<IActionResult> GetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<OrderPaymentsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<OrderPaymentsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据库列表
            var list = await _orderPaymentService.QueryListAsync<OrderPayments>(top,
                x => (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.TradeNo != null && x.TradeNo.Contains(searchParam.Keyword))),
                null,
                searchParam.OrderBy ?? "-AddTime,-Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<OrderPaymentsDto>>(list).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取分页列表
        /// 示例：/admin/order/payment?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("OrderPayment", ActionType.View)]
        public async Task<IActionResult> GetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<OrderPaymentsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<OrderPaymentsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表，如果ID大于0则查询该用户下所有的列表
            var list = await _orderPaymentService.QueryPageAsync<OrderPayments>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.TradeNo != null && x.TradeNo.Contains(searchParam.Keyword))),
                null,
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

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<OrderPaymentsDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/order/payment/1
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("OrderPayment", ActionType.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!await _orderPaymentService.ExistsAsync<OrderPayments>(x => x.Id == id))
            {
                throw new ResponseException($"数据[{id}]不存在或已删除");
            }
            var result = await _orderPaymentService.DeleteAsync(x => x.Id == id);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录(级联数据)
        /// 示例：/admin/order/payment?ids=1,2,3
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("OrderPayment", ActionType.Delete)]
        public async Task<IActionResult> DeleteByIds([FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var arrIds = Ids.ToIEnumerable<int>() ?? throw new ResponseException("传输参数不符合规范");
            //执行批量删除操作
            await _orderPaymentService.DeleteAsync(x => arrIds.Contains(x.Id));

            return NoContent();
        }
        #endregion

        #region 当前账户调用接口========================
        /// <summary>
        /// 根据交易号获取数据
        /// 示例：/account/order/payment/RN2021031...
        /// </summary>
        [HttpGet("/account/order/payment/{tradeNo}")]
        [Authorize]
        public async Task<IActionResult> AccountGetByNo([FromRoute] string tradeNo, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<OrderPaymentsDto>())
            {
                throw new ResponseException("请输入正确的塑性参数");
            }
            //获取登录用户ID
            int userId = _userService.GetUserId();
            if (userId == 0)
            {
                throw new ResponseException("用户尚未登录");
            }

            //查询数据库获取实体
            var model = await _orderPaymentService.QueryAsync<OrderPayments>(
                x => x.UserId == userId
                && x.TradeNo == tradeNo,
                null, WriteRoRead.Write)
                ?? throw new ResponseException($"收款单[{tradeNo}]未开始或已失效");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<OrderPaymentsDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 取消付款
        /// 示例：/account/order/payment/cancel/RN2021031...
        /// </summary>
        [HttpPut("/account/order/payment/cancel/{tradeNo}")]
        [Authorize]
        public async Task<IActionResult> AccountCancel([FromRoute] string tradeNo)
        {
            //获取登录用户ID
            int userId = _userService.GetUserId();
            if (userId == 0)
            {
                throw new ResponseException("用户尚未登录");
            }
            //保存数据
            var result = await _orderPaymentService.CancelAsync(x => x.UserId == userId && x.TradeNo == tradeNo);
            if (result)
            {
                return NoContent();
            }
            throw new ResponseException("保存过程中发生了错误");
        }

        /// <summary>
        /// 提交更改支付方式(客户)
        /// 示例：/account/order/payment/edit
        /// </summary>
        [HttpPut("/account/order/payment/edit")]
        [Authorize]
        public async Task<IActionResult> AccountPayment([FromBody] OrderPaymentsEditDto modelDto)
        {
            //获取当前登录用户
            modelDto.UserId = _userService.GetUserId();
            var model = await _orderPaymentService.PayAsync(modelDto);
            var result = _mapper.Map<OrderPaymentsDto>(model);
            return Ok(result);
        }
        #endregion
    }
}
