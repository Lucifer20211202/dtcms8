using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 会员信息
    /// </summary>
    public class Members
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 所属站点ID
        /// </summary>
        [Display(Name = "所属站点")]
        public int SiteId { get; set; }

        /// <summary>
        /// 所属用户ID
        /// </summary>
        [Display(Name = "所属用户")]
        public int UserId { get; set; }

        /// <summary>
        /// 推荐用户ID
        /// </summary>
        [Display(Name = "推荐用户")]
        public int ReferrerId { get; set; }

        /// <summary>
        /// 所属会员组ID
        /// </summary>
        [Display(Name = "会员组")]
        public int GroupId { get; set; }

        /// <summary>
        /// 会员头像
        /// </summary>
        [Display(Name = "会员头像")]
        [StringLength(512)]
        public string? Avatar { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        [StringLength(20)]
        public string? RealName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        [StringLength(20)]
        public string? Sex { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [Display(Name = "生日")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        [Display(Name = "余额")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; } = 0;

        /// <summary>
        /// 积分
        /// </summary>
        [Display(Name = "积分")]
        public int Point { get; set; } = 0;

        /// <summary>
        /// 经验值
        /// </summary>
        [Display(Name = "经验值")]
        public int Exp { get; set; } = 0;

        /// <summary>
        /// 状态(0正常1待审2黑名单)
        /// </summary>
        [Display(Name = "状态")]
        public byte Status { get; set; } = 0;

        /// <summary>
        /// 注册IP
        /// </summary>
        [Display(Name = "注册IP")]
        [StringLength(128)]
        public string? RegIp { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        [Display(Name = "注册时间")]
        public DateTime RegTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 是否分销商
        /// </summary>
        [Display(Name = "是否分销商")]
        public byte IsReseller { get; set; } = 0;

        /// <summary>
        /// 消费总金额
        /// </summary>
        [Display(Name = "消费总金额")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal OrderAmount { get; set; } = 0M;

        /// <summary>
        /// 佣金总金额
        /// </summary>
        [Display(Name = "佣金总金额")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CommAmount { get; set; } = 0M;

        /// <summary>
        /// 到期时间(付费会员到期时间)
        /// </summary>
        [Display(Name = "到期时间")]
        public DateTime? ExpiryTime { get; set; }


        /// <summary>
        /// 用户信息
        /// </summary>
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        /// <summary>
        /// 会员组
        /// </summary>
        [ForeignKey("GroupId")]
        public MemberGroups? Group { get; set; }

        /// <summary>
        /// 站点信息
        /// </summary>
        [ForeignKey("SiteId")]
        public Sites? Site { get; set; }
    }
}
