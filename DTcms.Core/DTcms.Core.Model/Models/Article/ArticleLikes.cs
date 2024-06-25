using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 文章点赞
    /// </summary>
    public class ArticleLikes
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 所属文章ID
        /// </summary>
        [Display(Name = "所属文章")]
        public long ArticleId { get; set; }

        /// <summary>
        /// 所属用户ID
        /// </summary>
        [Display(Name = "所属用户")]
        public int UserId { get; set; }

        /// <summary>
        /// 点赞时间
        /// </summary>
        [Display(Name = "点赞时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;


        /// <summary>
        /// 文章信息
        /// </summary>
        [ForeignKey("ArticleId")]
        public Articles? Article { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
