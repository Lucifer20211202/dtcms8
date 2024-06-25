using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 第三方开放平台
    /// </summary>
    public class SiteOAuths
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 所属站点
        /// </summary>
        [Display(Name = "所属站点")]
        public int SiteId { get; set; }

        /// <summary>
        /// 平台标识
        /// qq|wechat
        /// </summary>
        [Display(Name = "平台标识")]
        [StringLength(128)]
        public string? Provider { get; set; }

        /// <summary>
        /// 接口类型
        /// web(网站)|mp(小程序)|app
        /// </summary>
        [Display(Name = "接口类型")]
        [StringLength(128)]
        public string? Type { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        [StringLength(128)]
        public string? Title { get; set; }

        /// <summary>
        /// 显示图标
        /// </summary>
        [Display(Name = "显示图标")]
        [StringLength(512)]
        public string? ImgUrl { get; set; }

        /// <summary>
        /// 开放平台提供的AppId
        /// </summary>
        [Display(Name = "AppId")]
        [StringLength(512)]
        public string? ClientId { get; set; }

        /// <summary>
        /// 开放平台提供的AppKey
        /// </summary>
        [Display(Name = "AppKey")]
        [StringLength(512)]
        public string? ClientSecret { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        public int SortId { get; set; } = 99;

        /// <summary>
        /// 状态0启用1关闭
        /// </summary>
        [Display(Name = "状态")]
        public byte Status { get; set; } = 0;

        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "创建人")]
        [StringLength(30)]
        public string? AddBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;


        /// <summary>
        /// 站点信息
        /// </summary>
        [ForeignKey("SiteId")]
        public Sites? Site { get; set; }
    }
}
