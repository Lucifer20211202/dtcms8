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

namespace DTcms.Core.API.Controllers.Application
{
    /// <summary>
    /// 授权记录
    /// </summary>
    [Route("admin/site/oauth/login")]
    [ApiController]
    public class SiteOAuthLoginController(ISiteOAuthLoginService siteOAuthLoginService, IMapper mapper) : ControllerBase
    {
        private readonly ISiteOAuthLoginService _siteOAuthLoginService = siteOAuthLoginService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 获取分页列表
        /// 示例：/admin/site/oauth/login?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("OAuth", ActionType.View)]
        public async Task<IActionResult> GetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy!=null
                && !searchParam.OrderBy.TrimStart('-').IsPropertyExists<SiteOAuthLoginsDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<SiteOAuthLoginsDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表
            var list = await _siteOAuthLoginService.QueryPageAsync<SiteOAuthLogins>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => string.IsNullOrWhiteSpace(searchParam.Keyword)
                || (x.Provider != null && x.Provider.Contains(searchParam.Keyword)) || (x.User != null && x.User.UserName != null && x.User.UserName.Contains(searchParam.Keyword)),
                query => query.Include(o => o.User).Include(o => o.OAuth),
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
            var resultDto = _mapper.Map<IEnumerable<SiteOAuthLoginsDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/site/oauth/login/1
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("OAuth", ActionType.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!await _siteOAuthLoginService.ExistsAsync<SiteOAuthLogins>(x => x.Id == id))
            {
                throw new ResponseException($"数据{id}不存在或已删除");
            }
            var result = await _siteOAuthLoginService.DeleteAsync<SiteOAuthLogins>(x => x.Id == id);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录(级联数据)
        /// 示例：/admin/site/oauth/login?ids=1,2,3
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("OAuth", ActionType.Delete)]
        public async Task<IActionResult> DeleteByIds([FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var listIds = Ids.ToIEnumerable<int>() ?? throw new ResponseException("传输参数不符合规范");
            //执行批量删除操作
            await _siteOAuthLoginService.DeleteAsync<SiteOAuthLogins>(x => listIds.Contains(x.Id));

            return NoContent();
        }
        #endregion
    }
}
