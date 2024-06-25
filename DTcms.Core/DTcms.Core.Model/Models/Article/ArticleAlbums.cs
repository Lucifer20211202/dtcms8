﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 文章相册
    /// </summary>
    public class ArticleAlbums
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 所属文章ID
        /// </summary>
        [Display(Name = "所属文章")]
        public long ArticleId { get; set; }

        /// <summary>
        /// 缩略图
        /// </summary>
        [Display(Name = "缩略图")]
        [StringLength(512)]
        public string? ThumbPath { get; set; }

        /// <summary>
        /// 原图
        /// </summary>
        [Display(Name = "原图")]
        [StringLength(512)]
        public string? OriginalPath { get; set; }

        /// <summary>
        /// 图片描述
        /// </summary>
        [Display(Name = "图片描述")]
        [StringLength(512)]
        public string? Remark { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        public int SortId { get; set; } = 99;

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;


        /// <summary>
        /// 文章信息
        /// </summary>
        [ForeignKey("ArticleId")]
        public Articles? Article { get; set; }

    }
}
