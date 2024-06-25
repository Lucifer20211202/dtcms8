using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Sms.V20210111;
using TencentCloud.Sms.V20210111.Models;


namespace DTcms.Core.Common.Helpers
{
    /// <summary>
    /// 腾讯云发送短信
    /// </summary>
    public class TencentSmsHelper
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        public static string Send(string accessKey, string secretKey, string? sdkAppId, string signName, string[] phoneNumbers, string templateId, string[] templateParam)
        {
            try
            {
                /* 必要步骤：
                 * 实例化一个认证对象，入参需要传入腾讯云账户密钥对secretId，secretKey
                 * SecretId、SecretKey 查询: https://console.cloud.tencent.com/cam/capi */
                Credential cred = new()
                {
                    SecretId = accessKey,
                    SecretKey = secretKey
                };

                /* 非必要步骤:
                 * 实例化一个客户端配置对象，可以指定超时时间等配置 */
                ClientProfile clientProfile = new();
                 /* 实例化一个客户端配置对象，可以指定接入地域域名 */
                HttpProfile httpProfile = new();
                clientProfile.HttpProfile = httpProfile;
                /* 实例化要请求产品(以sms为例)的client对象
                 * 第二个参数是地域信息，可以直接填写字符串ap-guangzhou，支持的地域列表参考 https://cloud.tencent.com/document/api/382/52071#.E5.9C.B0.E5.9F.9F.E5.88.97.E8.A1.A8 */
                SmsClient client = new SmsClient(cred, "ap-guangzhou", clientProfile);
                // 实例化一个请求对象，根据调用的接口和实际情况，可以进一步设置请求参数
                SendSmsRequest req = new()
                {
                    // 短信签名内容: 使用 UTF-8 编码，必须填写已审核通过的签名
                    SignName = signName,
                    // 模板 ID: 必须填写已审核通过的模板ID
                    TemplateId = templateId,
                    /* 下发手机号码，采用 E.164 标准，+[国家或地区码][手机号]
                     * 示例如：+8613711112222， 其中前面有一个+号 ，86为国家码，13711112222为手机号，最多不要超过200个手机号*/
                    PhoneNumberSet = phoneNumbers,
                    /* 模板参数: 模板参数的个数需要与 TemplateId 对应模板的变量个数保持一致，若无模板参数，则设置为空 */
                    TemplateParamSet = templateParam,
                    //短信 SdkAppId，在 短信控制台 添加应用后生成的实际 SdkAppId，示例如1400006666。
                    SmsSdkAppId = sdkAppId
                };

                SendSmsResponse resp = client.SendSmsSync(req);

                return resp.RequestId;
            }
            catch (Exception ex)
            {
                throw new ResponseException("发送短信时异常：" + ex.Message, Emums.ErrorCode.Error, 500);
            }

        }
    }
}
