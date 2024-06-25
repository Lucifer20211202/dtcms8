using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 会员充值
    /// </summary>
    public class MemberRecharges
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        /// 充值金额
        /// </summary>
        [Display(Name = "充值金额")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; } = 0;

        /// <summary>
        /// 充值状态(0待办1完成2取消)
        /// </summary>
        [Display(Name = "状态")]
        public byte Status { get; set; } = 0;

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;


        /// <summary>
        /// 订单支付列表
        /// </summary>
        public ICollection<OrderPayments>? OrderPayments { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
