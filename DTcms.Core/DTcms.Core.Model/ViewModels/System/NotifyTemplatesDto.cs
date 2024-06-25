using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 系统通知模板(显示)
    /// </summary>
    public class NotifyTemplatesDto: NotifyTemplatesEditDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 系统默认(0否1是)
        /// </summary>
        [Display(Name = "系统默认")]
        public byte IsSystem { get; set; } = 0;

        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        public DateTime? UpdateTime { get; set; }
    }

    /// <summary>
    /// 系统通知模板(编辑)
    /// </summary>
    public class NotifyTemplatesEditDto
    {
        /// <summary>
        /// 模板类型1邮件2短信3微信
        /// </summary>
        [Display(Name = "模板类型")]
        [Range(1, 3, ErrorMessage = "{0}只能选择1-3其中一项")]
        public byte Type { get; set; } = 0;

        /// <summary>
        /// 调用标识
        /// </summary>
        [Display(Name = "调用标识")]
        [Required(ErrorMessage = "{0}不可为空")]
        [MaxLength(128, ErrorMessage = "{0}不可超出{1}字符")]
        public string? CallIndex { get; set; }

        /// <summary>
        /// 模板标题
        /// </summary>
        [Display(Name = "模板标题")]
        [MaxLength(512, ErrorMessage = "{0}不可超出{1}字符")]
        public string? Title { get; set; }

        /// <summary>
        /// 模板标识
        /// </summary>
        [Display(Name = "模板标识")]
        [MaxLength(128, ErrorMessage = "{0}不可超出{1}字符")]
        public string? TemplateId { get; set; }

        /// <summary>
        /// 模板内容
        /// </summary>
        [Display(Name = "模板内容")]
        [Required(ErrorMessage = "{0}不可为空")]
        public string? Content { get; set; }
    }
}
