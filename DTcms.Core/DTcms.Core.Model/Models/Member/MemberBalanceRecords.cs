using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 会员余额记录
    /// </summary>
    public class MemberBalanceRecords
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
        /// 所属用户名
        /// </summary>
        [Display(Name = "用户名")]
        [StringLength(128)]
        public string? UserName { get; set; }

        /// <summary>
        /// 当前金额
        /// </summary>
        [Display(Name = "当前金额")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CurrAmount { get; set; }

        /// <summary>
        /// 增减金额
        /// </summary>
        [Display(Name = "增减金额")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Value { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        [StringLength(512)]
        public string? Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;


        /// <summary>
        /// 用户信息
        /// </summary>
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
