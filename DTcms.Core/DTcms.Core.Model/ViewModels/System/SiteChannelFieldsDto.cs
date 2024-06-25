using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 频道字段DTO
    /// </summary>
    public class SiteChannelFieldsDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 所属频道ID
        /// </summary>
        [Display(Name = "所属频道")]
        public int ChannelId { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        [Display(Name = "字段名")]
        [Required(ErrorMessage = "{0}不可为空")]
        [StringLength(128)]
        public string? Name { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        [Required(ErrorMessage = "{0}不可为空")]
        [StringLength(128)]
        public string? Title { get; set; }

        /// <summary>
        /// 控件类型
        /// </summary>
        [Display(Name = "控件类型")]
        [Required(ErrorMessage = "{0}不可为空")]
        [StringLength(128)]
        public string? ControlType { get; set; }

        /// <summary>
        /// 选项列表
        /// </summary>
        [Display(Name = "选项列表")]
        public string? ItemOption { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        [Display(Name = "默认值")]
        [StringLength(512)]
        public string? DefaultValue { get; set; }

        /// <summary>
        /// 是否密码框
        /// </summary>
        [Display(Name = "是否密码框")]
        [Range(0, 9)]
        public byte IsPassword { get; set; } = 0;

        /// <summary>
        /// 是否必填(0非必填1必填)
        /// </summary>
        [Display(Name = "是否必填")]
        [Range(0, 9)]
        public byte IsRequired { get; set; } = 0;

        /// <summary>
        /// 编辑器(0标准型1简洁型)
        /// </summary>
        [Display(Name = "编辑器")]
        [Range(0, 9)]
        public byte EditorType { get; set; } = 0;

        /// <summary>
        /// 验证提示信息
        /// </summary>
        [Display(Name = "验证提示信息")]
        [StringLength(255)]
        public string? ValidTipMsg { get; set; }

        /// <summary>
        /// 验证失败提示信息
        /// </summary>
        [Display(Name = "验证失败提示信息")]
        [StringLength(255)]
        public string? ValidErrorMsg { get; set; }

        /// <summary>
        /// 验证正则表达式
        /// </summary>
        [Display(Name = "验证正则表达式")]
        [StringLength(255)]
        public string? ValidPattern { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        public int SortId { get; set; } = 99;


        /// <summary>
        /// 多选项
        /// </summary>
        public object? Options { get; set; }

        /// <summary>
        /// 选项值
        /// </summary>
        public object? FieldValue { get; set; }
    }
}
