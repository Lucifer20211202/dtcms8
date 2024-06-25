using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 会员组别(显示)
    /// </summary>
    public class MemberGroupsDto : MemberGroupsEditDto
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
        [StringLength(128)]
        public string? AddBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// 会员组别(编辑)
    /// </summary>
    public class MemberGroupsEditDto
    {
        /// <summary>
        /// 组名称
        /// </summary>
        [Display(Name = "组名称")]
        [Required(ErrorMessage = "{0}不可为空")]
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
        /// 是否参与升级
        /// </summary>
        [Display(Name = "是否参与升级")]
        public byte IsUpgrade { get; set; } = 1;

        /// <summary>
        /// 是否默认
        /// </summary>
        [Display(Name = "是否默认")]
        [Range(0, 9)]
        public byte IsDefault { get; set; } = 0;

        /// <summary>
        /// 状态(0正常1不启用)
        /// </summary>
        [Display(Name = "状态")]
        [Range(0, 9)]
        public byte Status { get; set; } = 0;
    }
}
