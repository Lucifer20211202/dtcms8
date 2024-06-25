using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 会员组
    /// </summary>
    public class MemberGroups
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 组名称
        /// </summary>
        [Display(Name = "组名称")]
        [StringLength(128)]
        public string? Title { get; set; }

        /// <summary>
        /// 等级排序(按顺序升级)
        /// </summary>
        [Display(Name = "等级排序")]
        public int Grade { get; set; } = 1;

        /// <summary>
        /// 升级所需预存款
        /// </summary>
        [Display(Name = "升级所需预存款")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; } = 0;

        /// <summary>
        /// 升级所需经验值
        /// </summary>
        [Display(Name = "升级所需经验值")]
        public int Exp { get; set; } = 0;

        /// <summary>
        /// 是否参与升级(0否1是)
        /// </summary>
        [Display(Name = "是否参与升级")]
        public byte IsUpgrade { get; set; } = 0;

        /// <summary>
        /// 是否默认(0否1是)
        /// </summary>
        [Display(Name = "是否默认")]
        public byte IsDefault { get; set; } = 0;

        /// <summary>
        /// 状态(0正常1不启用)
        /// </summary>
        [Display(Name = "状态")]
        public byte Status { get; set; } = 0;

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
