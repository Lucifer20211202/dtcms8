﻿using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 第三方开放平台(显示)
    /// </summary>
    public class SiteOAuthsDto: SiteOAuthsEditDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "创建人")]
        [StringLength(128)]
        public string? AddBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime? AddTime { get; set; }


        /// <summary>
        /// 站点信息
        /// </summary>
        public SitesDto? Site { get; set; }
    }

    /// <summary>
    /// 第三方开放平台(编辑)
    /// </summary>
    public class SiteOAuthsEditDto
    {
        /// <summary>
        /// 所属站点ID
        /// </summary>
        [Display(Name = "所属站点")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int SiteId { get; set; }

        /// <summary>
        /// 平台标识
        /// qq|wechat
        /// </summary>
        [Display(Name = "平台标识")]
        [Required(ErrorMessage = "{0}不可为空")]
        [StringLength(20)]
        public string? Provider { get; set; }

        /// <summary>
        /// 接口类型
        /// web(网站)|mp(小程序)|app
        /// </summary>
        [Display(Name = "接口类型")]
        [Required(ErrorMessage = "{0}不可为空")]
        [StringLength(20)]
        public string? Type { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        [Required(ErrorMessage = "{0}不可为空")]
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
        [Required(ErrorMessage = "{0}不可为空")]
        [StringLength(512)]
        public string? ClientId { get; set; }

        /// <summary>
        /// 开放平台提供的AppKey
        /// </summary>
        [Display(Name = "AppKey")]
        [Required(ErrorMessage = "{0}不可为空")]
        [StringLength(512)]
        public string? ClientSecret { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        public int SortId { get; set; } = 99;

        /// <summary>
        /// 状态(0启用1关闭)
        /// </summary>
        [Display(Name = "状态")]
        public byte Status { get; set; } = 0;
    }
}
