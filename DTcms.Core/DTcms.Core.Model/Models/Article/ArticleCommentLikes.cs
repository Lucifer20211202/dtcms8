using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 评论点赞
    /// </summary>
    public class ArticleCommentLikes
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 所属评论ID
        /// </summary>
        [Display(Name = "所属评论")]
        public int CommentId { get; set; }

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

        /// <summary>
        /// 评论信息
        /// </summary>
        [ForeignKey("CommentId")]
        public ArticleComments? Comment { get; set; }
    }
}
