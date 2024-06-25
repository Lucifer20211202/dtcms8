using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace DTcms.Core.API.Filters
{
    /// <summary>
    /// 管理日志过滤器
    /// </summary>
    public class GlobalRequestFilter(IManagerLogService managerLogService, IHttpContextAccessor httpContextAccessor) : IAsyncResourceFilter
    {
        private readonly IManagerLogService _managerLogService = managerLogService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        /// <summary>
        /// 实现方法
        /// </summary>
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            await next();
            var method = context.HttpContext.Request.Method;
            var path = context.HttpContext.Request.Path.ToString();
            var userName = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;

            //记录日志，只记录管理员接口请求
            if (!string.IsNullOrEmpty(path)
                && path.ToLower().StartsWith("/admin/")
                && userName != null
                && (method.ToLower().Equals("post")
                || method.ToLower().Equals("put")
                || method.ToLower().Equals("patch")
                || method.ToLower().Equals("delete")))
            {
                ManagerLogs model = new()
                {
                    UserName = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value,
                    Method = method,
                    Path = path,
                    Query = context.HttpContext.Request.QueryString.ToString(),
                    StatusCode = context.HttpContext.Response.StatusCode.ToString(),
                    AddTime = DateTime.Now
                };
                await _managerLogService.AddAsync<ManagerLogs>(model);
            }
        }
    }
}
