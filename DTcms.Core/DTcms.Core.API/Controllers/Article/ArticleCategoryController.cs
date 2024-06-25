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
    /// 文章类别
    /// </summary>
    [Route("admin/article/category")]
    [ApiController]
    public class ArticleCategoryController(IArticleCategoryService categoryService, IUserService userService, IMapper mapper) : ControllerBase
    {
        private readonly IArticleCategoryService _categoryService = categoryService;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取数据
        /// 示例：/admin/article/category/1/1
        /// </summary>
        [HttpGet("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleCategory", ActionType.View, "channelId")]
        public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<ArticleCategorysDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _categoryService.QueryAsync<ArticleCategorys>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<ArticleCategorysDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取指定数量列表
        /// 示例：/admin/article/category/view/1/1/0
        /// </summary>
        [HttpGet("view/{channelId}/{parentId}/{top}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleCategory", ActionType.View, "channelId")]
        public async Task<IActionResult> GetList([FromRoute] int parentId, [FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (!searchParam.Fields.IsPropertyExists<ArticleCategorysDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据库列表
            var resultFrom = await _categoryService.QueryListAsync(top, parentId);
            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<ArticleCategorysDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取树目录列表
        /// 示例：/admin/article/category/1/1
        /// </summary>
        [HttpGet("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleCategory", ActionType.View, "channelId")]
        public async Task<IActionResult> GetList([FromQuery] BaseParameter param, [FromRoute] int channelId)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<ArticleCategorysDto>() || channelId <= 0)
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据库列表
            var resultFrom = await _categoryService.QueryListAsync(channelId, 0);
            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var resultDto = resultFrom.AsEnumerable().ShapeData(param.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/article/category/1
        /// </summary>
        [HttpPost("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleCategory", ActionType.Add, "channelId")]
        public async Task<IActionResult> Add([FromBody] ArticleCategorysEditDto modelDto)
        {
            //验证数据是否合法
            if (!TryValidateModel(modelDto))
            {
                return ValidationProblem(ModelState);
            }
            //检查频道是否存在
            var channelModel = await _categoryService.QueryAsync<SiteChannels>(x => x.Id.Equals(modelDto.ChannelId));
            if (channelModel == null)
            {
                throw new ResponseException("频道不存在或已删除");
            }
            
            //映射成实体
            var model = _mapper.Map<ArticleCategorys>(modelDto);
            model.SiteId = channelModel.SiteId;
            //获取当前用户名
            model.AddBy = _userService.GetUserName();
            model.AddTime = DateTime.Now;
            //写入数据库
            var mapModel = await _categoryService.AddAsync(model);
            //映射成DTO再返回，否则出错
            var result = _mapper.Map<ArticleCategorysDto>(mapModel);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/admin/article/category/1/1
        /// </summary>
        [HttpPut("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleCategory", ActionType.Edit, "channelId")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ArticleCategorysEditDto modelDto)
        {
            //验证数据是否合法
            if (!TryValidateModel(modelDto))
            {
                return ValidationProblem(ModelState);
            }
            if (id == modelDto.ParentId)
            {
                throw new ResponseException($"不能把自己设置为父类，请重试");
            }
            await _categoryService.UpdateAsync(id, modelDto);
            return NoContent();
        }

        /// <summary>
        /// 局部更新一条记录
        /// 示例：/admin/article/category/1/1
        /// Body：[{"op":"replace","path":"/title","value":"new title"}]
        /// </summary>
        [HttpPatch("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleCategory", ActionType.Edit, "channelId")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] JsonPatchDocument<ArticleCategorysEditDto> patchDocument)
        {
            //注意：要使用写的数据库进行查询，才能正确写入数据主库
            var model = await _categoryService.QueryAsync<ArticleCategorys>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据{id}不存在或已删除");

            var modelToPatch = _mapper.Map<ArticleCategorysEditDto>(model);
            patchDocument.ApplyTo(modelToPatch, ModelState);
            //验证数据是否合法
            if (!TryValidateModel(modelToPatch))
            {
                return ValidationProblem(ModelState);
            }
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelToPatch, model);
            var result = await _categoryService.SaveAsync();

            //由于没有调用方法，手动清空缓存
            if (result)
            {
                await _categoryService.RemoveCacheAsync<ArticleCategorys>(true);
            }

            return NoContent();
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/article/category/1/1
        /// </summary>
        [HttpDelete("{channelId}/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleCategory", ActionType.Delete, "channelId")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!await _categoryService.ExistsAsync<ArticleCategorys>(x => x.Id == id))
            {
                throw new ResponseException($"数据{id}不存在或已删除");
            }
            var result = await _categoryService.DeleteAsync(x => x.Id == id);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录(级联数据)
        /// 示例：/admin/article/category/1?ids=1,2,3
        /// </summary>
        [HttpDelete("{channelId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("ArticleCategory", ActionType.Delete, "channelId")]
        public async Task<IActionResult> DeleteByIds([FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var arrIds = Ids.ToIEnumerable<int>() ?? throw new ResponseException("传输参数不符合规范");
            //执行批量删除操作
            await _categoryService.DeleteAsync(x => arrIds.Contains(x.Id));

            return NoContent();
        }
        #endregion

        #region 前台调用接口============================
        /// <summary>
        /// 根据ID获取数据(缓存)
        /// 示例：/client/article/category/1
        /// </summary>
        [HttpGet("/client/article/category/{id}")]
        public async Task<IActionResult> ClientGetById([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<ArticleCategorysDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _categoryService.QueryAsync<ArticleCategorys>(id.ToString(), x => x.Id == id)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<ArticleCategorysDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取指定数量列表(缓存)
        /// 示例：/client/article/category/channel/news/view/1/0?siteId=1
        /// </summary>
        [HttpGet("/client/article/category/channel/{channelKey}/view/{parentId}/{top}")]
        public async Task<IActionResult> ClientGetList([FromRoute] string channelKey, [FromRoute] int parentId, [FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (!searchParam.Fields.IsPropertyExists<ArticleCategorysDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            if (searchParam.SiteId == 0)
            {
                throw new ResponseException("所属站点不能为空");
            }

            //检查是否频道ID或者名称
            SiteChannels? channelModel = null;
            if (int.TryParse(channelKey, out int channelId))
            {
                channelModel = await _categoryService.QueryAsync<SiteChannels>(x => x.Id.Equals(channelId) && x.SiteId.Equals(searchParam.SiteId));
            }
            if (channelModel == null)
            {
                channelModel = await _categoryService.QueryAsync<SiteChannels>(x => x.Name != null && x.Name.Equals(channelKey) && x.SiteId.Equals(searchParam.SiteId));
            }
            if (channelModel == null)
            {
                throw new ResponseException("频道不存在或已删除");
            }

            //获取缓存Key
            var cacheKey = $"{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据库列表
            var resultFrom = await _categoryService.QueryListByCacheAsync(cacheKey, top, channelModel.Id, parentId);
            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<ArticleCategorysDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 根据频道ID或名称获取树目录列表(缓存)
        /// 示例：/client/article/category/channel/news?siteId=1
        /// </summary>
        [HttpGet("/client/article/category/channel/{channelKey}")]
        public async Task<IActionResult> ClientGetList([FromRoute] string channelKey, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (!searchParam.Fields.IsPropertyExists<ArticleCategorysDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            if (searchParam.SiteId == 0)
            {
                throw new ResponseException("所属站点不能为空");
            }

            //检查是否频道ID或者名称
            SiteChannels? channelModel = null;
            if (int.TryParse(channelKey, out int channelId))
            {
                channelModel = await _categoryService.QueryAsync<SiteChannels>(x => x.Id.Equals(channelId) && x.SiteId.Equals(searchParam.SiteId));
            }
            if (channelModel == null)
            {
                channelModel = await _categoryService.QueryAsync<SiteChannels>(x => x.Name != null && x.Name.Equals(channelKey) && x.SiteId.Equals(searchParam.SiteId));
            }
            if (channelModel == null)
            {
                throw new ResponseException("频道不存在或已删除");
            }
            //如果有查询关健字
            int parentId = 0; //父节点ID
            if (!string.IsNullOrWhiteSpace(searchParam.Keyword))
            {
                var model = await _categoryService.QueryAsync<ArticleCategorys>(x => x.Title != null && x.Title.Contains(searchParam.Keyword))
                    ?? throw new ResponseException("暂无查询记录");
                parentId = model.Id;
            }

            //获取缓存Key
            var cacheKey = $"{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据库列表
            var resultFrom = await _categoryService.QueryListByCacheAsync(cacheKey, channelModel.Id, parentId);
            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var resultDto = resultFrom.AsEnumerable().ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }
        #endregion
    }
}
