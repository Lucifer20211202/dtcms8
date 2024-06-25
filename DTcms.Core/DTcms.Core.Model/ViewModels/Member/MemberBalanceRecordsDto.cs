using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 余额记录(显示)
    /// </summary>
    public class MemberBalanceRecordsDto : MemberBalanceRecordsEditDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        [StringLength(128)]
        public string? UserName { get; set; }

        /// <summary>
        /// 当前金额
        /// </summary>
        [Display(Name = "当前金额")]
        public decimal CurrAmount { get; set; } = 0;
    }

    /// <summary>
    /// 余额记录(编辑)
    /// </summary>
    public class MemberBalanceRecordsEditDto
    {
        /// <summary>
        /// 所属用户ID
        /// </summary>
        [Display(Name = "所属用户")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int UserId { get; set; }

        /// <summary>
        /// 增减金额
        /// </summary>
        [Display(Name = "增减金额")]
        [Required(ErrorMessage = "{0}不可为空")]
        public decimal Value { get; set; } = 0;

        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        [StringLength(512)]
        [Required(ErrorMessage = "{0}不可为空")]
        public string? Description { get; set; }
    }
}
