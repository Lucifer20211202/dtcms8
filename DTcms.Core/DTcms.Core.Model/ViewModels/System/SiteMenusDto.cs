using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 站点菜单(显示)
    /// </summary>
    public class SiteMenusDto : SiteMenusEditDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "创建人")]
        public string? AddBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; }


        /// <summary>
        /// 子类列表
        /// </summary>
        public List<SiteMenusDto> Children { get; set; } = [];
    }

    /// <summary>
    /// 站点菜单(编辑)
    /// </summary>
    public class SiteMenusEditDto
    {
        /// <summary>
        /// 所属父类ID
        /// </summary>
        [Display(Name = "所属父类")]
        public int ParentId { get; set; } = 0;

        /// <summary>
        /// 菜单标题
        /// </summary>
        [Display(Name = "菜单标题")]
        [Required(ErrorMessage = "{0}不可为空")]
        [MaxLength(128, ErrorMessage = "{0}不可超出{1}字符")]
        [MinLength(1, ErrorMessage = "{0}不可小于{1}字符")]
        public string? Title { get; set; }

        /// <summary>
        /// 菜单副标题
        /// </summary>
        [Display(Name = "菜单副标题")]
        [MaxLength(512, ErrorMessage = "{0}不可超出{1}字符")]
        public string? SubTitle { get; set; }

        /// <summary>
        /// 图标地址
        /// </summary>
        [Display(Name = "图标地址")]
        [MaxLength(512, ErrorMessage = "{0}不可超出{1}字符")]
        public string? IconUrl { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        [Display(Name = "链接地址")]
        [MaxLength(512, ErrorMessage = "{0}不可超出{1}字符")]
        public string? LinkUrl { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        public int SortId { get; set; } = 99;

        /// <summary>
        /// 状态(0正常1禁用)
        /// </summary>
        [Display(Name = "状态")]
        public byte Status { get; set; } = 0;

        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        [MaxLength(512, ErrorMessage = "{0}不可超出{1}字符")]
        public string? Remark { get; set; }
    }
}
