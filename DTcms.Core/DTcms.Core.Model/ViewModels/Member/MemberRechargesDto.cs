using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 会员充值(显示)
    /// </summary>
    public class MemberRechargesDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 所属用户ID
        /// </summary>
        [Display(Name = "所属用户")]
        public int UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        [StringLength(128)]
        public string? UserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 充值金额
        /// </summary>
        [Display(Name = "充值金额")]
        public decimal Amount { get; set; } = 0;

        /// <summary>
        /// 充值状态(0待办1完成2取消)
        /// </summary>
        [Display(Name = "充值状态")]
        public byte Status { get; set; } = 0;

        /// <summary>
        /// 交易单号
        /// </summary>
        [Display(Name = "交易单号")]
        [StringLength(128)]
        public string? TradeNo { get; set; }

        /// <summary>
        /// 所属支付方式ID
        /// </summary>
        [Display(Name = "支付方式")]
        public int? PaymentId { get; set; }

        /// <summary>
        /// 支付方式名称
        /// </summary>
        [Display(Name = "支付方式名称")]
        [StringLength(128)]
        public string? PaymentTitle { get; set; }
    }

    /// <summary>
    /// 会员充值(编辑)
    /// </summary>
    public class MemberRechargesEditDto
    {
        /// <summary>
        /// 充值用户ID
        /// </summary>
        [Display(Name = "充值用户")]
        public int? UserId { get; set; }

        /// <summary>
        /// 充值金额
        /// </summary>
        [Display(Name = "充值金额")]
        [Required(ErrorMessage = "{0}不可为空")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 支付方式ID
        /// </summary>
        [Display(Name = "支付方式")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int PaymentId { get; set; }
    }
}
