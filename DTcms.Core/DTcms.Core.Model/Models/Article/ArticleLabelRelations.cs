using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 文章标签关联
    /// </summary>
    public class ArticleLabelRelations
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
        /// 所属标签ID
        /// </summary>
        [Display(Name = "所属标签")]
        public int LabelId { get; set; }

        /// <summary>
        /// 文章信息
        /// </summary>
        [ForeignKey("ArticleId")]
        public Articles? Article { get; set; }

        /// <summary>
        /// 标签信息
        /// </summary>
        [ForeignKey("LabelId")]
        public ArticleLabels? Label { get; set; }
    }
}
