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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.API.Controllers
{
    /// <summary>
    /// 会员充值记录
    /// </summary>
    [Route("admin/member/recharge")]
    [ApiController]
    public class MemberRechargeController(IMemberRechargeService memberRechargeService, IOrderPaymentService orderPaymentService,
        IUserService userService, IMapper mapper) : ControllerBase
    {
        private readonly IMemberRechargeService _memberRechargeService = memberRechargeService;
        private readonly IOrderPaymentService _orderPaymentService = orderPaymentService;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取数据
        /// 示例：/admin/member/recharge/1
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberRecharge", ActionType.View)]
        public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<MemberRechargesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _memberRechargeService.QueryAsync<MemberRecharges>(x => x.Id == id, query=>query.Include(x=>x.OrderPayments), WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //根据字段进行塑形
            var result = _mapper.Map<MemberRechargesDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取总记录数量
        /// 示例：/admin/member/recharge/view/count
        /// </summary>
        [HttpGet("view/count")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberRecharge", ActionType.View)]
        public async Task<IActionResult> GetCount([FromQuery] BaseParameter searchParam)
        {
            var result = await _memberRechargeService.CountAsync<MemberRecharges>(x => searchParam.Status < 0 || x.Status == searchParam.Status);
            //返回成功200
            return Ok(result);
        }

        /// <summary>
        /// 获取总记录金额
        /// 示例：/admin/member/recharge/view/amount
        /// </summary>
        [HttpGet("view/amount")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberRecharge", ActionType.View)]
        public async Task<IActionResult> GetAmount([FromQuery] ReportParameter searchParam)
        {
            if (searchParam.StartTime == null)
            {
                searchParam.StartTime = DateTime.Now.AddHours(-DateTime.Now.Hour + 1);
            }
            var result = await _memberRechargeService.QueryAmountAsync(
                x => (searchParam.Status <= -1 || x.Status == searchParam.Status)
                && !searchParam.StartTime.HasValue || x.AddTime >= searchParam.StartTime.Value);
            //返回成功200
            return Ok(result);
        }

        /// <summary>
        /// 获取分页列表
        /// 示例：/admin/member/recharge?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberRecharge", ActionType.View)]
        public async Task<IActionResult> GetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<MemberRechargesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<MemberRechargesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表，如果ID大于0则查询该用户下所有的列表
            var list = await _memberRechargeService.QueryPageAsync<MemberRecharges>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.OrderPayments != null && x.OrderPayments.Any(x => x.TradeNo!.Contains(searchParam.Keyword)))),
                query => query.Include(x => x.OrderPayments),
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
            var resultDto = _mapper.Map<IEnumerable<MemberRechargesDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/member/recharge
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberRecharge", ActionType.Add)]
        public async Task<IActionResult> Add([FromBody] MemberRechargesEditDto modelDto)
        {
            if (modelDto.UserId == 0)
            {
                throw new ResponseException("请选择充值的账户");
            }
            //保存充值记录
            var model = await _memberRechargeService.AddAsync(modelDto);
            //映射成DTO再返回，否则出错
            var result = _mapper.Map<OrderPaymentsDto>(model);
            return Ok(result);
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/member/recharge/1
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberRecharge", ActionType.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            //检查记录是否存在
            if (!await _memberRechargeService.ExistsAsync<MemberRecharges>(x => x.Id == id))
            {
                throw new ResponseException($"数据[{id}]不存在或已删除");
            }
            var result = await _memberRechargeService.DeleteAsync(x => x.Id == id);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录
        /// 示例：/admin/member/recharge?ids=1,2,3
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberRecharge", ActionType.Delete)]
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
            await _memberRechargeService.DeleteAsync(x => listIds.Contains(x.Id));
            return NoContent();
        }

        /// <summary>
        /// 完成充值订单
        /// 示例：/admin/member/recharge?ids=1,2,3
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberRecharge", ActionType.Complete)]
        public async Task<IActionResult> Complete([FromQuery] string Ids)
        {
            //检查参数是否为空
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var listIds = Ids.ToIEnumerable<int>() ?? throw new ResponseException("传输参数不符合规范");

            //将符合条件的一次查询出来
            var list = await _memberRechargeService.QueryListAsync<MemberRecharges>(0, x => x.Status == 0 && listIds.Contains(x.Id),
                query => query.Include(x => x.OrderPayments));
            //执行批量删除操作
            foreach (var modelt in list)
            {
                var tradeNo = modelt.OrderPayments?.FirstOrDefault()?.TradeNo;
                if (tradeNo != null)
                {
                    await _orderPaymentService.ConfirmAsync(tradeNo);
                }
            }
            return NoContent();
        }
        #endregion

        #region 当前用户调用接口========================
        /// <summary>
        /// 获取指定数量列表
        /// 示例：/account/member/recharge/view/10
        /// </summary>
        [HttpGet("/account/member/recharge/view/{top}")]
        [Authorize]
        public async Task<IActionResult> AccountList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy!=null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<MemberRechargesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<MemberRechargesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //获取登录用户ID
            int userId = _userService.GetUserId();

            //获取数据库列表
            var list = await _memberRechargeService.QueryListAsync<MemberRecharges>(top,
                x => x.UserId == userId
                && (searchParam.Status <= 0 || x.Status == searchParam.Status)
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.OrderPayments != null && x.OrderPayments.Any(x => x.TradeNo!.Contains(searchParam.Keyword)))),
                query => query.Include(x => x.OrderPayments),
                searchParam.OrderBy ?? "-Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<MemberRechargesDto>>(list).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取分页列表
        /// 示例：/account/member/recharge?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet("/account/member/recharge")]
        [Authorize]
        public async Task<IActionResult> AccountList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy!=null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<MemberRechargesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<MemberRechargesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //获取登录用户ID
            int userId = _userService.GetUserId();

            //获取数据列表，如果ID大于0则查询该用户下所有的列表
            var list = await _memberRechargeService.QueryPageAsync<MemberRecharges>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => x.UserId == userId
                && (searchParam.Status <= 0 || x.Status == searchParam.Status)
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.OrderPayments != null && x.OrderPayments.Any(x => x.TradeNo!.Contains(searchParam.Keyword)))),
                query => query.Include(x => x.OrderPayments),
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
            var resultDto = _mapper.Map<IEnumerable<MemberRechargesDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 账户充值
        /// 示例：/account/member/recharge
        /// </summary>
        [HttpPost("/account/member/recharge")]
        [Authorize]
        public async Task<IActionResult> AccountAdd([FromBody] MemberRechargesEditDto modelDto)
        {
            //获取登录用户ID
            modelDto.UserId = _userService.GetUserId();
            if (modelDto.UserId == 0)
            {
                throw new ResponseException("用户尚未登录");
            }
            //保存数据
            var model = await _memberRechargeService.AddAsync(modelDto);
            //映射成DTO再返回，否则出错
            var result = _mapper.Map<OrderPaymentsDto>(model);
            return Ok(result);
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/account/member/recharge/1
        /// </summary>
        [HttpDelete("/account/member/recharge/{id}")]
        [Authorize]
        public async Task<IActionResult> AccountDelete([FromRoute] int id)
        {
            //获取登录用户ID
            int userId = _userService.GetUserId();
            if (userId == 0)
            {
                throw new ResponseException("用户尚未登录");
            }
            //检查记录是否存在
            if (!await _memberRechargeService.ExistsAsync<MemberRecharges>(x => x.Id == id && x.UserId == userId))
            {
                throw new ResponseException($"数据不存在或已删除");
            }
            var result = await _memberRechargeService.DeleteAsync(x => x.Id == id && x.UserId == userId && x.Status != 1);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录
        /// 示例：/account/member/recharge?ids=1,2,3
        /// </summary>
        [HttpDelete("/account/member/recharge")]
        [Authorize]
        public async Task<IActionResult> AccountDeleteByIds([FromQuery] string Ids)
        {
            //检查参数是否为空
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //获取登录用户ID
            int userId = _userService.GetUserId();
            if (userId == 0)
            {
                throw new ResponseException("用户尚未登录");
            }
            //将ID列表转换成IEnumerable
            var listIds = Ids.ToIEnumerable<int>();
            if (listIds == null)
            {
                throw new ResponseException("传输参数不符合规范");
            }
            //执行批量删除操作
            await _memberRechargeService.DeleteAsync(x => listIds.Contains(x.Id) && x.UserId == userId && x.Status != 1);
            return NoContent();
        }
        #endregion
    }
}
