using System;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 手机短信内容实体
    /// </summary>
    public class SmsMessageDto
    {
        /// <summary>
        /// 手机号，多个号码以,逗号分隔开
        /// </summary>
        public string? PhoneNumbers { get; set; }

        /// <summary>
        /// 已审核的模板标识
        /// </summary>
        public string? TemplateId { get; set; }

        /// <summary>
        /// 模板的参数
        /// </summary>
        public string? TemplateParam { get; set; }
    }
}
