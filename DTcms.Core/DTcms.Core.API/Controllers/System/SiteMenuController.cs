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
    /// 站点菜单
    /// </summary>
    [Route("admin/site/menu")]
    [ApiController]
    public class SiteMenuController(ISiteMenuService siteMenuService, IUserService userService, IMapper mapper) : ControllerBase
    {
        private readonly ISiteMenuService _siteMenuService = siteMenuService;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取数据
        /// 示例：/admin/site/menu/1
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("SiteMenu", ActionType.View)]
        public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<SiteMenusDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _siteMenuService.QueryAsync<SiteMenus>(x => x.Id == id, null, WriteRoRead.Write);
            if (model == null)
            {
                throw new ResponseException($"数据[{id}]不存在或已删除");
            }
            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<SiteMenusDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取树目录列表
        /// 示例：/admin/site/menu
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("SiteMenu", ActionType.View)]
        public async Task<IActionResult> GetList([FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<SiteMenusDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //如果有查询关健字
            int parentId = 0; //父节点ID
            if (param.SiteId > 0 || !string.IsNullOrWhiteSpace(param.Keyword))
            {
                var model = await _siteMenuService.QueryAsync<SiteMenus>(x => 
                    string.IsNullOrWhiteSpace(param.Keyword) || (x.Title != null && x.Title.Contains(param.Keyword)))
                    ?? throw new ResponseException("暂无查询记录");
                parentId = model.Id;
            }
            //获取数据库列表
            var resultFrom = await _siteMenuService.QueryListAsync(parentId);
            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var resultDto = resultFrom.AsEnumerable().ShapeData(param.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/site/menu
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("SiteMenu", ActionType.Add)]
        public async Task<IActionResult> Add([FromBody] SiteMenusEditDto modelDto)
        {
            //映射成实体
            var model = _mapper.Map<SiteMenus>(modelDto);
            //获取当前用户名
            model.AddBy = _userService.GetUserName();
            model.AddTime = DateTime.Now;
            //写入数据库
            await _siteMenuService.AddAsync(model);
            //映射成DTO再返回，否则出错
            var result = _mapper.Map<SiteMenusDto>(model);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/admin/site/menu/1
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("SiteMenu", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SiteMenusEditDto modelDto)
        {
            //查找记录
            var model = await _siteMenuService.QueryAsync<SiteMenus>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelDto, model);
            var result = await _siteMenuService.SaveAsync();

            //手动删除缓存
            await _siteMenuService.RemoveCacheAsync<SiteMenus>(true);

            return NoContent();
        }

        /// <summary>
        /// 局部更新一条记录
        /// 示例：/admin/site/menu/1
        /// Body：[{"op":"replace","path":"/title","value":"new title"}]
        /// </summary>
        [HttpPatch("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("SiteMenu", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] JsonPatchDocument<SiteMenusEditDto> patchDocument)
        {
            var model = await _siteMenuService.QueryAsync<SiteMenus>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            var modelToPatch = _mapper.Map<SiteMenusEditDto>(model);
            patchDocument.ApplyTo(modelToPatch, ModelState);
            //验证数据是否合法
            if (!TryValidateModel(modelToPatch))
            {
                return ValidationProblem(ModelState);
            }
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelToPatch, model);
            await _siteMenuService.SaveAsync();

            //手动删除缓存
            await _siteMenuService.RemoveCacheAsync<SiteMenus>(true);

            return NoContent();
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/site/menu/1
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("SiteMenu", ActionType.Delete)]
        public async Task<IActionResult> Delete([FromRoute] long id)
        {
            if (!await _siteMenuService.ExistsAsync<SiteMenus>(x => x.Id == id))
            {
                throw new ResponseException($"数据[{id}]不存在或已删除");
            }
            var result = await _siteMenuService.DeleteAsync(x => x.Id == id);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录(级联数据)
        /// 示例：/admin/site/menu?ids=1,2,3
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("SiteMenu", ActionType.Delete)]
        public async Task<IActionResult> DeleteByIds([FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var arrIds = Ids.ToIEnumerable<int>();
            if (arrIds == null)
            {
                throw new ResponseException("传输参数不符合规范");
            }
            //执行批量删除操作
            await _siteMenuService.DeleteAsync(x => arrIds.Contains(x.Id));

            return NoContent();
        }
        #endregion

        #region 前台调用接口============================
        /// <summary>
        /// 获取树目录列表
        /// 示例：/client/site/menu/1
        /// </summary>
        [HttpGet("/client/site/menu/{parentId}")]
        public async Task<IActionResult> ClientGetList([FromRoute] int parentId, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<SiteMenusDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            
            //获取缓存Key
            var cacheKey = $"{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据库列表
            var resultFrom = await _siteMenuService.QueryListAsync(cacheKey, parentId);
            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var resultDto = resultFrom.AsEnumerable().ShapeData(param.Fields);
            //返回成功200
            return Ok(resultDto);
        }
        #endregion
    }
}
