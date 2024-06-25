using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 站点菜单
    /// </summary>
    public class SiteMenus
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 父导航ID
        /// </summary>
        [Display(Name = "所属父级")]
        public int ParentId { get; set; }

        /// <summary>
        /// 菜单标题
        /// </summary>
        [Display(Name = "菜单标题")]
        [StringLength(128)]
        public string? Title { get; set; }

        /// <summary>
        /// 菜单副标题
        /// </summary>
        [Display(Name = "副标题")]
        [StringLength(512)]
        public string? SubTitle { get; set; }

        /// <summary>
        /// 图标地址
        /// </summary>
        [Display(Name = "图标地址")]
        [StringLength(512)]
        public string? IconUrl { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        [Display(Name = "链接地址")]
        [StringLength(512)]
        public string? LinkUrl { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        public int SortId { get; set; } = 99;

        /// <summary>
        /// 状态0正常1禁用
        /// </summary>
        [Display(Name = "状态")]
        public byte Status { get; set; } = 0;

        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        [StringLength(512)]
        public string? Remark { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "创建人")]
        [StringLength(30)]
        public string? AddBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;
    }
}
