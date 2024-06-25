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
    /// 会员站内消息
    /// </summary>
    [Route("admin/member/message")]
    [ApiController]
    public class MemberMessageController(IMemberMessageService memberMessageService, IUserService userService, IMapper mapper) : ControllerBase
    {
        private readonly IMemberMessageService _memberMessageService = memberMessageService;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取数据
        /// 示例：/admin/member/message/1
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberMessage", ActionType.View)]
        public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<MemberMessagesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _memberMessageService.QueryAsync<MemberMessages>(x => x.Id == id, query => query.Include(x => x.User), WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<MemberMessagesDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取指定数量列表
        /// 示例：/admin/member/message/view/0
        /// </summary>
        [HttpGet("view/{top}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<MemberMessagesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<MemberMessagesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据库列表
            var list = await _memberMessageService.QueryListAsync<MemberMessages>(top,
                x => string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.User != null && x.User.UserName != null && x.User.UserName.Contains(searchParam.Keyword)),
                query => query.Include(x => x.User),
                searchParam.OrderBy ?? "-AddTime,-Id");

            //映射成DTO，根据字段进行塑形
            var result = _mapper.Map<IEnumerable<MemberMessagesDto>>(list).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(result);
        }

        /// <summary>
        /// 获取分页列表
        /// 示例：/admin/member/message?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberMessage", ActionType.View)]
        public async Task<IActionResult> GetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<MemberMessagesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<MemberMessagesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表，如果ID大于0则查询该用户下所有的列表
            var list = await _memberMessageService.QueryPageAsync<MemberMessages>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.User != null && x.User.UserName != null && x.User.UserName.Contains(searchParam.Keyword)),
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
            var result = _mapper.Map<IEnumerable<MemberMessagesDto>>(list.Items).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(result);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/member/message
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberMessage", ActionType.Add)]
        public async Task<IActionResult> Add([FromBody] MemberMessagesEditDto modelDto)
        {
            //检查会员是否存在
            if (!await _memberMessageService.ExistsAsync<Members>(x => x.UserId == modelDto.UserId))
            {
                throw new ResponseException($"会员ID[{modelDto.UserId}]不存在");
            }
            //映射成实体
            var model = _mapper.Map<MemberMessages>(modelDto);
            //写入数据库
            await _memberMessageService.AddAsync<MemberMessages>(model);

            //使用AutoMapper转换成ViewModel
            var result = _mapper.Map<MemberMessagesDto>(model);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/admin/member/message/1
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberMessage", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] MemberMessagesEditDto modelDto)
        {
            //查找记录
            var model = await _memberMessageService.QueryAsync<MemberMessages>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据{id}不存在或已删除");

            //因为只有一次查询，更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelDto, model);
            await _memberMessageService.SaveAsync();
            return NoContent();
        }

        /// <summary>
        /// 局部更新一条记录
        /// 示例：/admin/member/message/1
        /// Body：[{"op":"replace","path":"/title","value":"new title"}]
        /// </summary>
        [HttpPatch("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberMessage", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] JsonPatchDocument<MemberMessagesEditDto> patchDocument)
        {
            //检查记录是否存在
            var model = await _memberMessageService.QueryAsync<MemberMessages>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            var modelToPatch = _mapper.Map<MemberMessagesEditDto>(model);
            patchDocument.ApplyTo(modelToPatch, ModelState);
            //验证数据是否合法
            if (!TryValidateModel(modelToPatch))
            {
                return ValidationProblem(ModelState);
            }
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelToPatch, model);
            await _memberMessageService.SaveAsync();
            return NoContent();
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/member/message/1
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberMessage", ActionType.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            //检查参数是否正确
            if (!await _memberMessageService.ExistsAsync<MemberMessages>(x => x.Id == id))
            {
                throw new ResponseException($"数据[{id}]不存在或已删除");
            }
            var result = await _memberMessageService.DeleteAsync<MemberMessages>(x => x.Id == id);
            return NoContent();
        }

        /// <summary>
        /// 批量删除记录
        /// 示例：/admin/member/message?ids=1,2,3
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberMessage", ActionType.Delete)]
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
            await _memberMessageService.DeleteAsync<MemberMessages>(x => listIds.Contains(x.Id));
            return NoContent();
        }
        #endregion

        #region 当前用户调用接口========================
        /// <summary>
        /// 根据ID获取用户一条数据
        /// 示例：/account/member/message/1
        /// </summary>
        [HttpGet("/account/member/message/{id}")]
        [Authorize]
        public async Task<IActionResult> AccountGetById([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<MemberMessagesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //获取登录用户ID
            int userId = _userService.GetUserId();
            //查询数据库获取实体
            var model = await _memberMessageService.QueryAsync<MemberMessages>(x => x.UserId == userId && x.Id == id)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<MemberMessagesDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取指定数量列表
        /// 示例：/account/member/message/view/0
        /// </summary>
        [HttpGet("/account/member/message/view/{top}")]
        [Authorize]
        public async Task<IActionResult> AccountGetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<MemberMessagesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<MemberMessagesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //获取登录用户ID
            int userId = _userService.GetUserId();
            //获取数据库列表
            var list = await _memberMessageService.QueryListAsync<MemberMessages>(top,
                x => x.UserId == userId
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.User != null && x.User.UserName != null && x.User.UserName.Contains(searchParam.Keyword))),
                query => query.Include(x => x.User),
                searchParam.OrderBy ?? "-AddTime,-Id");

            //映射成DTO，根据字段进行塑形
            var result = _mapper.Map<IEnumerable<MemberMessagesDto>>(list).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(result);
        }

        /// <summary>
        /// 获取分页列表
        /// 示例：/account/member/message/list?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet("/account/member/message/list")]
        [Authorize]
        public async Task<IActionResult> AccountGetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<MemberMessagesDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<MemberMessagesDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //获取登录用户ID
            int userId = _userService.GetUserId();
            //获取数据列表，如果ID大于0则查询该用户下所有的列表
            var list = await _memberMessageService.QueryPageAsync<MemberMessages>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => x.UserId == userId
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.User != null && x.User.UserName != null && x.User.UserName.Contains(searchParam.Keyword))),
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
            var result = _mapper.Map<IEnumerable<MemberMessagesDto>>(list.Items).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(result);
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/account/member/message/1
        /// </summary>
        [HttpDelete("/account/member/message/{id}")]
        [Authorize]
        [AuthorizeFilter("MemberMessage", ActionType.Delete)]
        public async Task<IActionResult> AccountDelete([FromRoute] int id)
        {
            //获取登录用户ID
            int userId = _userService.GetUserId();
            if (userId == 0)
            {
                throw new ResponseException("用户尚未登录");
            }
            //检查参数是否正确
            if (!await _memberMessageService.ExistsAsync<MemberMessages>(x => x.UserId == userId && x.Id == id))
            {
                throw new ResponseException($"数据[{id}]不存在或已删除");
            }
            var result = await _memberMessageService.DeleteAsync<MemberMessages>(x => x.UserId == userId && x.Id == id);
            return NoContent();
        }

        /// <summary>
        /// 批量删除记录
        /// 示例：/account/member/message?ids=1,2,3
        /// </summary>
        [HttpDelete("/account/member/message")]
        [Authorize]
        [AuthorizeFilter("MemberMessage", ActionType.Delete)]
        public async Task<IActionResult> AccountDeleteByIds([FromQuery] string Ids)
        {
            //获取登录用户ID
            int userId = _userService.GetUserId();
            if (userId == 0)
            {
                throw new ResponseException("用户尚未登录");
            }
            //检查参数是否为空
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var listIds = Ids.ToIEnumerable<int>() ?? throw new ResponseException("传输参数不符合规范");

            //执行批量删除操作
            await _memberMessageService.DeleteAsync<MemberMessages>(x => x.UserId == userId && listIds.Contains(x.Id));
            return NoContent();
        }
        #endregion
    }
}