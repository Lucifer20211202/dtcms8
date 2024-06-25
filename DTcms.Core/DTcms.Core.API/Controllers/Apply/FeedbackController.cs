﻿using AutoMapper;
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
    /// 在线留言
    /// </summary>
    [Route("admin/feedback")]
    [ApiController]
    public class FeedbacksController(IFeedbackService feedbackService, IUserService userService, IMapper mapper) : ControllerBase
    {
        private readonly IFeedbackService _feedbackService = feedbackService;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        #region 管理员调用接口==========================
        /// <summary>
        /// 根据ID获取数据
        /// 示例：/admin/feedback/1
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Feedback", ActionType.View)]
        public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] BaseParameter param)
        {
            //检测参数是否合法
            if (!param.Fields.IsPropertyExists<FeedbacksDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }
            //查询数据库获取实体
            var model = await _feedbackService.QueryAsync<Feedbacks>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据{id}不存在或已删除");

            //使用AutoMapper转换成ViewModel，根据字段进行塑形
            var result = _mapper.Map<FeedbacksDto>(model).ShapeData(param.Fields);
            return Ok(result);
        }

        /// <summary>
        /// 获取留言列表
        /// 示例：/admin/feedback?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Feedback", ActionType.View)]
        public async Task<IActionResult> GetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null && !searchParam.OrderBy.TrimStart('-').IsPropertyExists<FeedbacksDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<FeedbacksDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取数据列表
            var list = await _feedbackService.QueryPageAsync<Feedbacks>(
                pageParam.PageSize,
                pageParam.PageIndex,
                x => (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Content != null && x.Content.Contains(searchParam.Keyword))),
                null,
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
            var resultDto = _mapper.Map<IEnumerable<FeedbacksDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/admin/feedback
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Feedback", ActionType.Add)]
        public async Task<IActionResult> Add([FromBody] FeedbacksEditDto modelDto)
        {
            //映射成实体
            var model = _mapper.Map<Feedbacks>(modelDto);
            //获取当前用户名
            model.AddBy = _userService.GetUserName();
            model.AddTime = DateTime.Now;
            //写入数据库
            await _feedbackService.AddAsync<Feedbacks>(model);
            //映射成DTO再返回，否则出错
            var result = _mapper.Map<FeedbacksDto>(model);
            return Ok(result);
        }

        /// <summary>
        /// 修改一条记录
        /// 示例：/admin/feedback/1
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Feedback", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] FeedbacksEditDto modelDto)
        {
            //查找记录
            var model = await _feedbackService.QueryAsync<Feedbacks>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"记录[{id}]不存在或已删除");

            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelDto, model);
            //如果已答复,则更新答复时间
            if (model.ReplyContent != null)
            {
                model.ReplyBy = _userService.GetUserName();
                model.ReplyTime = DateTime.Now;
            }
            var result = await _feedbackService.SaveAsync();

            //由于没有调用方法，手动清空缓存
            if (result)
            {
                await _feedbackService.RemoveCacheAsync<Feedbacks>(true);
            }

            return NoContent();
        }

        /// <summary>
        /// 局部更新一条记录
        /// 示例：/admin/feedback/1
        /// Body：[{"op":"replace","path":"/title","value":"new title"}]
        /// </summary>
        [HttpPatch("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Feedback", ActionType.Edit)]
        public async Task<IActionResult> Update([FromRoute] long id, [FromBody] JsonPatchDocument<FeedbacksEditDto> patchDocument)
        {
            var model = await _feedbackService.QueryAsync<Feedbacks>(x => x.Id == id, null, WriteRoRead.Write)
                ?? throw new ResponseException($"数据{id}不存在或已删除");

            var modelToPatch = _mapper.Map<FeedbacksEditDto>(model);
            patchDocument.ApplyTo(modelToPatch, ModelState);
            //验证数据是否合法
            if (!TryValidateModel(modelToPatch))
            {
                return ValidationProblem(ModelState);
            }
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelToPatch, model);
            var result = await _feedbackService.SaveAsync();

            //由于没有调用方法，手动清空缓存
            if (result)
            {
                await _feedbackService.RemoveCacheAsync<Feedbacks>(true);
            }

            return NoContent();
        }

        /// <summary>
        /// 批量审核记录
        /// 示例：/admin/feedback?ids=1,2,3
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Feedback", ActionType.Audit)]
        public async Task<IActionResult> AuditByIds([FromQuery] string Ids)
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
            //找出符合条件的记录
            var list = await _feedbackService.QueryListAsync<Feedbacks>(1000, x => x.Status == 0 && arrIds.Contains(x.Id), null, null, WriteRoRead.Write);
            if (list == null || list.Count() == 0)
            {
                throw new ResponseException("暂无需要审核的记录");
            }
            foreach (var item in list)
            {
                item.Status = 1;
            }
            //保存到数据库
            var result = await _feedbackService.SaveAsync();

            //由于没有调用方法，手动清空缓存
            if (result)
            {
                await _feedbackService.RemoveCacheAsync<Feedbacks>(true);
            }

            return NoContent();
        }

        /// <summary>
        /// 删除一条记录
        /// 示例：/admin/feedback/1
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Feedback", ActionType.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!await _feedbackService.ExistsAsync<Feedbacks>(x => x.Id == id, WriteRoRead.Write))
            {
                throw new ResponseException($"记录[{id}]不存在或已删除");
            }
            var result = await _feedbackService.DeleteAsync<Feedbacks>(x => x.Id == id);

            return NoContent();
        }

        /// <summary>
        /// 批量删除记录(级联数据)
        /// 示例：/admin/feedback?ids=1,2,3
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [AuthorizeFilter("Feedback", ActionType.Delete)]
        public async Task<IActionResult> DeleteByIds([FromQuery] string Ids)
        {
            if (Ids == null)
            {
                throw new ResponseException("传输参数不可为空");
            }
            //将ID列表转换成IEnumerable
            var ids = Ids.ToIEnumerable<int>() ?? throw new ResponseException("传输参数不符合规范");
            //执行批量删除操作
            await _feedbackService.DeleteAsync<Feedbacks>(x => ids.Contains(x.Id));

            return NoContent();
        }
        #endregion

        #region 前台调用接口============================
        /// <summary>
        /// 获取指定数量列表(缓存)
        /// 示例：/client/feedback/view/10
        /// </summary>
        [HttpGet("/client/feedback/view/{top}")]
        public async Task<IActionResult> ClientGetList([FromRoute] int top, [FromQuery] BaseParameter searchParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<FeedbacksDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<FeedbacksDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取缓存Key
            var cacheKey = $"{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取数据库列表
            var resultFrom = await _feedbackService.QueryListAsync<Feedbacks>(cacheKey, top,
                x => x.Status == 1
                && (searchParam.SiteId < 0 || x.SiteId == searchParam.SiteId)
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Content != null && x.Content.Contains(searchParam.Keyword))),
                null,
                searchParam.OrderBy ?? "-Id");

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<FeedbacksDto>>(resultFrom).ShapeData(searchParam.Fields);
            //返回成功200
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取分页列表(缓存)
        /// 示例：/client/feedback?pageSize=10&pageIndex=1
        /// </summary>
        [HttpGet("/client/feedback")]
        public async Task<IActionResult> ClientGetList([FromQuery] BaseParameter searchParam, [FromQuery] PageParamater pageParam)
        {
            //检测参数是否合法
            if (searchParam.OrderBy != null
                && !searchParam.OrderBy.Replace("-", "").IsPropertyExists<FeedbacksDto>())
            {
                throw new ResponseException("请输入正确的排序参数");
            }
            if (!searchParam.Fields.IsPropertyExists<FeedbacksDto>())
            {
                throw new ResponseException("请输入正确的属性参数");
            }

            //获取缓存Key
            var cacheKey = $"{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //获取所有的列表
            var list = await _feedbackService.QueryPageAsync<Feedbacks>(
                cacheKey,
                pageParam.PageSize,
                pageParam.PageIndex,
                x => x.Status == 1
                && (searchParam.SiteId < 0 || x.SiteId == searchParam.SiteId)
                && (string.IsNullOrWhiteSpace(searchParam.Keyword) || (x.Content != null && x.Content.Contains(searchParam.Keyword))),
                null,
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

            //映射成DTO，根据字段进行塑形
            var resultDto = _mapper.Map<IEnumerable<FeedbacksDto>>(list.Items).ShapeData(searchParam.Fields);
            return Ok(resultDto);
        }

        /// <summary>
        /// 添加一条记录
        /// 示例：/client/feedback
        /// </summary>
        [HttpPost("/client/feedback")]
        public async Task<IActionResult> ClientAdd([FromBody] FeedbacksClientDto modelDto)
        {
            //检查验证码
            var code = MemoryHelper.Get(modelDto.CodeKey);
            if (code == null)
            {
                throw new ResponseException("验证码已过期，请重试");
            }
            if (code?.ToString()?.ToLower() != modelDto?.CodeValue?.ToLower())
            {
                throw new ResponseException("验证码有误，请重试");
            }
            //验证完毕，删除验证码
            MemoryHelper.Remove(modelDto?.CodeKey);
            //映射成实体
            var model = _mapper.Map<Feedbacks>(modelDto);
            //获取当前用户名
            model.AddBy = _userService.GetUserName();
            model.AddTime = DateTime.Now;
            //写入数据库
            await _feedbackService.AddAsync(model);
            //映射成DTO再返回，否则出错
            var result = _mapper.Map<FeedbacksDto>(model);
            return Ok(result);
        }
        #endregion
    }
}