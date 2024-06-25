﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 文章类别
    /// </summary>
    public class ArticleCategorys
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
        /// 所属频道ID
        /// </summary>
        [Display(Name = "所属频道")]
        public int ChannelId { get; set; }

        /// <summary>
        /// 所属父类
        /// </summary>
        [Display(Name = "所属父类")]
        public int ParentId { get; set; } = 0;

        /// <summary>
        /// 调用别名
        /// </summary>
        [Display(Name = "调用标识")]
        [StringLength(128)]
        public string? CallIndex { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        [StringLength(128)]
        public string? Title { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        [Display(Name = "图片地址")]
        [StringLength(512)]
        public string? ImgUrl { get; set; }

        /// <summary>
        /// 外部链接
        /// </summary>
        [Display(Name = "外部链接")]
        [StringLength(512)]
        public string? LinkUrl { get; set; }

        /// <summary>
        /// 内容介绍
        /// </summary>
        [Display(Name = "内容介绍")]
        [StringLength(1024)]
        public string? Content { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        public int SortId { get; set; } = 99;

        /// <summary>
        /// SEO标题
        /// </summary>
        [Display(Name = "SEO标题")]
        [StringLength(256)]
        public string? SeoTitle { get; set; }

        /// <summary>
        /// SEO关健字
        /// </summary>
        [Display(Name = "SEO关健字")]
        [StringLength(512)]
        public string? SeoKeyword { get; set; }

        /// <summary>
        /// SEO描述
        /// </summary>
        [Display(Name = "SEO描述")]
        [StringLength(512)]
        public string? SeoDescription { get; set; }

        /// <summary>
        /// 状态(0正常1禁用)
        /// </summary>
        [Display(Name = "状态")]
        [Range(0, 9)]
        public byte Status { get; set; } = 0;

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
        public DateTime AddTime { get; set; } = DateTime.Now;


        /// <summary>
        /// 类别关联列表
        /// </summary>
        public ICollection<ArticleCategoryRelations>? CategoryRelations { get; set; }
    }
}
