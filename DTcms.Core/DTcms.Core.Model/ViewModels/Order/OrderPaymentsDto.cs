using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 支付收款单(显示)
    /// </summary>
    public class OrderPaymentsDto : OrderPaymentsListDto
    {
        /// <summary>
        /// 会员充值信息
        /// </summary>
        public MemberRechargesDto? MemberRecharge { get; set; }
    }

    /// <summary>
    /// 支付收款单(列表)
    /// </summary>
    public class OrderPaymentsListDto
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
        /// 所属充值ID
        /// </summary>
        [Display(Name = "所属充值")]
        public int? RechargeId { get; set; }

        /// <summary>
        /// 交易单号
        /// </summary>
        [Display(Name = "交易单号")]
        public string? TradeNo { get; set; }

        /// <summary>
        /// 所属支付方式ID
        /// </summary>
        [Display(Name = "支付方式")]
        public int? PaymentId { get; set; }

        /// <summary>
        /// 交易类型(枚举:0.商品购买1.会员充值)
        /// </summary>
        [Display(Name = "交易类型")]
        public byte TradeType { get; set; } = 0;

        /// <summary>
        /// 支付类型(0.在线支付1.货到付款)
        /// </summary>
        [Display(Name = "收款类型")]
        public byte? PaymentType { get; set; }

        /// <summary>
        /// 支付方式名称
        /// </summary>
        [Display(Name = "支付名称")]
        [StringLength(128)]
        public string? PaymentTitle { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        [Display(Name = "支付金额")]
        public decimal PaymentAmount { get; set; } = 0;

        /// <summary>
        /// 收款状态(1.待支付2.已支付3.已退款4.已取消)
        /// </summary>
        [Display(Name = "收款状态")]
        public byte Status { get; set; } = 1;

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 失效时间
        /// </summary>
        [Display(Name = "失效时间")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        [Display(Name = "完成时间")]
        public DateTime? CompleteTime { get; set; }
    }

    /// <summary>
    /// 支付收款单(新增)
    /// </summary>
    public class OrderPaymentsAddDto
    {
        /// <summary>
        /// 所属用户ID
        /// </summary>
        [Display(Name = "所属用户")]
        public int UserId { get; set; }

        /// <summary>
        /// 所属充值ID
        /// </summary>
        [Display(Name = "所属充值")]
        public int? RechargeId { get; set; }

        /// <summary>
        /// 交易单号
        /// </summary>
        [Display(Name = "交易单号")]
        public string? TradeNo { get; set; }

        /// <summary>
        /// 交易类型(枚举:0.商品购买1.会员充值)
        /// </summary>
        [Display(Name = "交易类型")]
        public byte TradeType { get; set; } = 0;

        /// <summary>
        /// 所属支付方式ID
        /// </summary>
        [Display(Name = "支付方式")]
        public int? PaymentId { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        [Display(Name = "支付金额")]
        public decimal PaymentAmount { get; set; } = 0;
    }

    /// <summary>
    /// 支付收款单(编辑)
    /// </summary>
    public class OrderPaymentsEditDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Display(Name = "所属用户")]
        public int? UserId { get; set; }

        /// <summary>
        /// 所属支付方式ID
        /// </summary>
        [Display(Name = "支付方式")]
        public int PaymentId { get; set; }
    }
}
