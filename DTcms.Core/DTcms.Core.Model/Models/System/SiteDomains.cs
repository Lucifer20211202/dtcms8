using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 站点域名
    /// </summary>
    public class SiteDomains
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
        /// 站点域名
        /// </summary>
        [Display(Name = "站点域名")]
        [Required]
        [StringLength(128)]
        public string? Domain { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        [StringLength(512)]
        public string? Remark { get; set; }


        /// <summary>
        /// 站点信息
        /// </summary>
        [ForeignKey("SiteId")]
        public Sites? Site { get; set; }
    }
}
