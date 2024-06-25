using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 文章会员组权限
    /// </summary>
    public class ArticleGroups
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
        /// 所属会员组ID
        /// </summary>
        [Display(Name = "所属会员组")]
        public int GroupId { get; set; }


        /// <summary>
        /// 文章信息
        /// </summary>
        [ForeignKey("ArticleId")]
        public Articles? Article { get; set; }

        /// <summary>
        /// 会员组信息
        /// </summary>
        [ForeignKey("GroupId")]
        public MemberGroups? MemberGroup { get; set; }
    }
}
