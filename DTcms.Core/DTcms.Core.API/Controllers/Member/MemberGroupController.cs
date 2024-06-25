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
    /// 会员组别
    /// </summary>
    [Route("admin/member/group")]
    [ApiController]
    public class MemberGroupController(IMemberGroupService memberGroupService, IUserService userService, IMapper mapper) : ControllerBase
    {
        private readonly IMemberGroupService _memberGroupService = memberGroupService;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取数据
        /// 示例：/admin/member/group/1
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberGroup", ActionType.View)]
        public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<MemberGroupsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _memberGroupService.QueryAsync<MemberGroups>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<MemberGroupsDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取指定数量列表
        /// 示例：/admin/member/group/view/0
        /// </summary>
        [HttpGet("view/{top}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberGroup", ActionType.View)]
        public async Task<IActionResult> GetList([FromRoute] int top, [FromQuery] GroupParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<MemberGroupsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<MemberGroupsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据库列表
            var list = await _memberGroupService.QueryListAsync<MemberGroups>(top,
                x => (searchParam.Status < 0 || x.Status == searchParam.Status)
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                null,
                searchParam.OrderBy ?? "Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<MemberGroupsDto>>(list).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取分页列表
        /// 示例：/admin/member/group?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberGroup", ActionType.View)]
        public async Task<IActionResult> GetList([FromQuery] GroupParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<MemberGroupsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<MemberGroupsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表，如果ID大于0则查询该用户下所有的列表
            var list = await _memberGroupService.QueryPageAsync<MemberGroups>(
                pageParam.PageSize,
                pageParam.PageIndex,
                 x => (searchParam.Status < 0 || x.Status == searchParam.Status)
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
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
            var resultDto = _mapper.Map<IEnumerable<MemberGroupsDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/member/group
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberGroup", ActionType.Add)]
        public async Task<IActionResult> Add([FromBody] MemberGroupsEditDto modelDto)
        {
            //映射成实体
            var model = _mapper.Map<MemberGroups>(modelDto);
            //获取当前用户名
            model.AddBy = _userService.GetUserName();
            model.AddTime = DateTime.Now;
            //写入数据库
            await _memberGroupService.AddAsync(model);
            //映射成DTO再返回，否则出错
            var result = _mapper.Map<MemberGroupsDto>(model);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/admin/member/group/1
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberGroup", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] MemberGroupsEditDto modelDto)
        {
            //查找记录
            var model = await _memberGroupService.QueryAsync<MemberGroups>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据{id}不存在或已删除");

            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelDto, model);
            var result = await _memberGroupService.SaveAsync();
            return NoContent();
        }

        // <summary>
        /// 局部更新一条记录
        /// 示例：/admin/member/group/1
        /// Body：[{"op":"replace","path":"/title","value":"new title"}]
        /// </summary>
        [HttpPatch("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberGroup", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] JsonPatchDocument<MemberGroupsEditDto> patchDocument)
        {
            //检查记录是否存在
            var model = await _memberGroupService.QueryAsync<MemberGroups>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            var modelToPatch = _mapper.Map<MemberGroupsEditDto>(model);
            patchDocument.ApplyTo(modelToPatch, ModelState);
            //验证数据是否合法
            if (!TryValidateModel(modelToPatch))
            {
                return ValidationProblem(ModelState);
            }

            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelToPatch, model);
            await _memberGroupService.SaveAsync();
            return NoContent();
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/member/group/1
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberGroup", ActionType.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            //检查参数是否正确
            if (!await _memberGroupService.ExistsAsync<MemberGroups>(x => x.Id == id))
            {
                throw new ResponseException($"数据{id}不存在或已删除");
            }
            var result = await _memberGroupService.DeleteAsync<MemberGroups>(x => x.Id == id);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录
        /// 示例：/admin/member/group?ids=1,2,3
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("MemberGroup", ActionType.Delete)]
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
            await _memberGroupService.DeleteAsync<MemberGroups>(x => listIds.Contains(x.Id));

            return NoContent();
        }
        #endregion

        #region 前台调用接口============================
        /// <summary>
        /// 获取指定数量列表(缓存)
        /// 示例：/client/member/group/view/0
        /// </summary>
        [HttpGet("/client/member/group/view/{top}")]
        public async Task<IActionResult> ClientGetList([FromRoute] int top, [FromQuery] GroupParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<MemberGroupsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<MemberGroupsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取缓存Key
            var cacheKey = $"{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据库列表
            var list = await _memberGroupService.QueryListAsync<MemberGroups>(cacheKey, top,
                x => x.Status == 0
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Title != null && x.Title.Contains(searchParam.Keyword))),
                null,
                searchParam.OrderBy ?? "Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<MemberGroupsDto>>(list).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }
        #endregion
    }
}