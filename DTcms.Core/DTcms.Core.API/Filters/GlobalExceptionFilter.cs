using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DTcms.Core.API.Filters
{
    /// <summary>
    /// 全局异常过滤器
    /// </summary>
    public class GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment environment) : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger = logger;
        private readonly IWebHostEnvironment _environment = environment;

        /// <summary>
        /// 实现过滤器方法
        /// </summary>
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ResponseException cmsException)
            {
                ResponseMessage warnResponse = new ResponseMessage(cmsException.GetErrorCode(), cmsException.Message, context.HttpContext);
                _logger.LogWarning(JsonHelper.ToJson(warnResponse));
                HandlerException(context, warnResponse, cmsException.GetCode());
                return;
            }
            string error = "异常信息：";
            void ReadException(Exception ex)
            {
                error += $"{ex.Message} | {ex.StackTrace} | {ex.InnerException}";
                if (ex.InnerException != null)
                {
                    ReadException(ex.InnerException);
                }
            }
            ReadException(context.Exception);

            _logger.LogError(error);//记录错误日志

            ResponseMessage apiResponse = new(ErrorCode.Error, _environment.IsDevelopment() ? error : "服务器正忙，请稍后再试.", context.HttpContext);
            HandlerException(context, apiResponse, StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// 返回响应字符串
        /// </summary>
        private static void HandlerException(ExceptionContext context, ResponseMessage apiResponse, int statusCode)
        {
            context.Result = new JsonResult(apiResponse)
            {
                StatusCode = statusCode,
                ContentType = "application/json",
            };
            context.ExceptionHandled = true;
        }
    }
}
