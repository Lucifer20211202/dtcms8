using DTcms.Core.Common.Emums;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DTcms.Core.Common.Helpers
{
    /// <summary>
    /// 请求响应统一消息
    /// </summary>
    public class ResponseMessage
    {
        /// <summary>
        ///错误码
        /// </summary>
        public ErrorCode Code { get; set; }

        /// <summary>
        ///错误信息
        /// </summary>
        public object? Message { get; set; }

        /// <summary>
        ///请求地址
        /// </summary>
        public string? Request { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ResponseMessage() { }
        /// <summary>
        /// 有参构造函数
        /// </summary>
        public ResponseMessage(ErrorCode errorCode)
        {
            Code = errorCode;
        }

        /// <summary>
        /// 自定义信息
        /// </summary>
        public ResponseMessage(ErrorCode errorCode, object message)
        {
            Code = errorCode;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        /// <summary>
        /// 自定义信息
        /// </summary>
        public ResponseMessage(ErrorCode errorCode, object message, HttpContext httpContext)
        {
            Code = errorCode;
            Message = message;
            Request = httpContext.Request.Method + " " + httpContext.Request.Path;
        }

        /// <summary>
        /// 操作成功
        /// </summary>
        public static ResponseMessage Success(string message = "操作成功")
        {
            return new ResponseMessage(ErrorCode.Success, message);
        }

        /// <summary>
        /// 操作失败
        /// </summary>
        public static ResponseMessage Error(string message = "操作失败")
        {
            return new ResponseMessage(ErrorCode.Fail, message);
        }

        /// <summary>
        /// 重写ToString方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            });
        }
    }
}
