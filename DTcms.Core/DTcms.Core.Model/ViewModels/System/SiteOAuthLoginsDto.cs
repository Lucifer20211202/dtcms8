using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 开放平台授权(显示)
    /// </summary>
    public class SiteOAuthLoginsDto: SiteOAuthLoginsEditDto
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
        public DateTime? AddTime { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        public string? UserName { get; set; }

        /// <summary>
        /// 授权标题
        /// </summary>
        [Display(Name = "授权标题")]
        public string? OAuthTitle { get; set; }
    }

    /// <summary>
    /// 开放平台授权(编辑)
    /// </summary>
    public class SiteOAuthLoginsEditDto
    {
        /// <summary>
        /// 所属开放平台
        /// </summary>
        [Display(Name = "所属平台")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int OAuthId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Display(Name = "所属用户")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int UserId { get; set; }

        /// <summary>
        /// 平台标识
        /// qq|wechat
        /// </summary>
        [Display(Name = "平台标识")]
        [Required(ErrorMessage = "{0}不可为空")]
        [StringLength(20)]
        public string? Provider { get; set; }

        /// <summary>
        /// 开放平台用户OpenId
        /// </summary>
        [Display(Name = "OpenId")]
        [Required(ErrorMessage = "{0}不可为空")]
        [StringLength(512)]
        public string? OpenId { get; set; }
    }
}
