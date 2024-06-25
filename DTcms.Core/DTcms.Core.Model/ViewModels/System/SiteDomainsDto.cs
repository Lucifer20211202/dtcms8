using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    public class SiteDomainsDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
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
        public string? Domain { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        public string? Remark { get; set; }
    }
}
