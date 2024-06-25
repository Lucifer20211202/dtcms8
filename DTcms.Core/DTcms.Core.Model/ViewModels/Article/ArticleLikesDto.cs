using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 文章点赞(显示)
    /// </summary>
    public class ArticleLikesDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 所属文章ID
        /// </summary>
        [Display(Name = "所属文章")]
        public long ArticleId { get; set; }

        /// <summary>
        /// 所属用户
        /// </summary>
        [Display(Name = "所属用户")]
        public int UserId { get; set; }

        /// <summary>
        /// 点赞时间
        /// </summary>
        [Display(Name = "点赞时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;
    }
}
