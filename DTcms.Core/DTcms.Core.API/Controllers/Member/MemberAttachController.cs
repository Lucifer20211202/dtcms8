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
    /// 会员附件下载记录
    /// </summary>
    [Route("member/attach")]
    [ApiController]
    public class MemberAttachController(IMemberAttachRecordService memberAttachRecordService, IUserService userService, IMapper mapper) : ControllerBase
    {
        private readonly IMemberAttachRecordService _memberAttachRecordService = memberAttachRecordService;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取数据
        /// 示例：/member/attach/1
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberAttach", ActionType.View)]
        public async Task<IActionResult> GetById([FromRoute] long id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<MemberAttachRecordsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _memberAttachRecordService.QueryAsync<MemberAttachRecords>(x => x.Id == id, query => query.Include(x => x.User), WriteRoRead.Write)
                ?? throw new ResponseException($"数据{id}不存在或已删除");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<MemberAttachRecordsDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取分页列表
        /// 示例：/member/attach?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberAttach", ActionType.View)]
        public async Task<IActionResult> GetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<MemberAttachRecordsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<MemberAttachRecordsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表，如果ID大于0则查询该用户下所有的列表
            var list = await _memberAttachRecordService.QueryPageAsync<MemberAttachRecords>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.UserName != null && x.UserName.Contains(searchParam.Keyword))),
                query => query.Include(x => x.User),
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
            var result = _mapper.Map<IEnumerable<MemberAttachRecordsDto>>(list).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(result);
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/member/attach/1
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberAttach", ActionType.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            //检查记录是否存在
            if (!await _memberAttachRecordService.ExistsAsync<MemberAttachRecords>(x => x.Id == id))
            {
                throw new ResponseException($"数据[{id}]不存在或已删除");
            }
            var result = await _memberAttachRecordService.DeleteAsync<MemberAttachRecords>(x => x.Id == id);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录
        /// 示例：/member/attach?ids=1,2,3
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberAttach", ActionType.Delete)]
        public async Task<IActionResult> DeleteByIds([FromQuery] string Ids)
        {
            //检查参数是否为空
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
            await _memberAttachRecordService.DeleteAsync<MemberAttachRecords>(x => listIds.Contains(x.Id));

            return NoContent();
        }
        #endregion

        #region 当前用户调用接口========================
        /// <summary>
        /// 获取指定数量列表
        /// 示例：/account/member/attach/view/0
        /// </summary>
        [HttpGet("/account/member/attach/view/{top}")]
        [Authorize]
        public async Task<IActionResult> AccountGetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<MemberAttachRecordsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<MemberAttachRecordsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //获取登录用户ID
            int userId = _userService.GetUserId();
            //获取数据库列表
            var list = await _memberAttachRecordService.QueryListAsync<MemberAttachRecords>(top,
                x => x.UserId == userId
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.UserName != null && x.UserName.Contains(searchParam.Keyword))),
                query => query.Include(x => x.User),
                searchParam.OrderBy ?? "-AddTime,-Id");

            //映射成DTO，根据字段进行塑形
            var result = _mapper.Map<IEnumerable<MemberAttachRecordsDto>>(list).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(result);
        }

        /// <summary>
        /// 获取分页列表
        /// 示例：/account/member/attach/list?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet("/account/member/attach/list")]
        [Authorize]
        public async Task<IActionResult> AccountGetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<MemberAttachRecordsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<MemberAttachRecordsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //获取登录用户ID
            int userId = _userService.GetUserId();
            //获取数据列表，如果ID大于0则查询该用户下所有的列表
            var list = await _memberAttachRecordService.QueryPageAsync<MemberAttachRecords>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => x.UserId == userId
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.UserName != null && x.UserName.Contains(searchParam.Keyword))),
                query => query.Include(x => x.User),
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
            var result = _mapper.Map<IEnumerable<MemberAttachRecordsDto>>(list.Items).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(result);
        }
        #endregion
    }
}