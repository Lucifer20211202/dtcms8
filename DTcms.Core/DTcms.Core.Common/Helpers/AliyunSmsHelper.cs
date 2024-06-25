using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Dysmsapi20170525;
using AlibabaCloud.SDK.Dysmsapi20170525.Models;

namespace DTcms.Core.Common.Helpers
{
    /// <summary>
    /// 阿里云短信帮助类
    /// </summary>
    public class AliyunSmsHelper
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        public static string Send(string accessKey, string secretKey, string signName, string phoneNumbers, string templateCode, string templateParam)
        {
            // 初始化客户端
            var client = new Client(new Config
            {
                AccessKeyId = accessKey,
                AccessKeySecret = secretKey
            });

            // 创建发送短信请求
            var request = new SendSmsRequest
            {
                PhoneNumbers = phoneNumbers,
                SignName = signName,
                TemplateCode = templateCode,
                TemplateParam = templateParam, // 根据你的模板替换相应的参数
                OutId = Guid.NewGuid().ToString() // 可选的，用于保证请求的幂等性
            };

            try
            {
                // 发送短信
                var response = client.SendSms(request);

                // 处理响应
                if (response.Body.Code == "OK")
                {
                    //短信发送成功
                    //Console.WriteLine("RequestId: " + response.Body.RequestId);
                    return response.Body.RequestId;
                }
                else
                {
                    //短信发送失败
                    //Console.WriteLine("ErrorCode: " + response.Body.Code);
                    //Console.WriteLine("ErrorMessage: " + response.Body.Message);
                    throw new ResponseException("短信发送失败：" + response.Body.Message, Emums.ErrorCode.Error, 500);
                }
            }
            catch (Exception ex)
            {
                throw new ResponseException("发送短信时异常：" + ex.Message, Emums.ErrorCode.Error, 500);
            }
        }
    }
}
