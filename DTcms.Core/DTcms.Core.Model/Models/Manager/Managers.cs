using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 管理员信息
    /// </summary>
    public class Managers
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 所属用户
        /// </summary>
        [Display(Name = "所属用户")]
        public int UserId { get; set; }

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
        [StringLength(30)]
        public string? RealName { get; set; }

        /// <summary>
        /// 启用发布审核
        /// </summary>
        [Display(Name = "启用发布审核")]
        public byte IsAudit { get; set; } = 0;

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
