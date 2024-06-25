using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 文章相册
    /// </summary>
    public class ArticleAlbumsDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public long Id { get; set; }

        /// <summary>
        /// 所属文章
        /// </summary>
        [Display(Name = "所属文章")]
        public long ArticleId { get; set; }

        /// <summary>
        /// 缩略图
        /// </summary>
        [Display(Name = "缩略图")]
        public string? ThumbPath { get; set; }

        /// <summary>
        /// 原图
        /// </summary>
        [Display(Name = "原图")]
        public string? OriginalPath { get; set; }

        /// <summary>
        /// 图片描述
        /// </summary>
        [Display(Name = "图片描述")]
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
    }
}
