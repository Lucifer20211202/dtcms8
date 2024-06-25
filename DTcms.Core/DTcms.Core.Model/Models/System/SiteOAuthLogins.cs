using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 开放平台授权
    /// </summary>
    public class SiteOAuthLogins
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 所属开放平台
        /// </summary>
        [Display(Name = "所属平台")]
        public int OAuthId { get; set; }

        /// <summary>
        /// 所属用户ID
        /// </summary>
        [Display(Name = "所属用户")]
        public int UserId { get; set; }

        /// <summary>
        /// 平台标识
        /// qq|wechat
        /// </summary>
        [Display(Name = "平台标识")]
        [StringLength(20)]
        public string? Provider { get; set; }

        /// <summary>
        /// 开放平台用户OpenId
        /// </summary>
        [Display(Name = "OpenId")]
        [StringLength(512)]
        public string? OpenId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;


        /// <summary>
        /// 开放平台信息
        /// </summary>
        [ForeignKey("OAuthId")]
        public SiteOAuths? OAuth { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
