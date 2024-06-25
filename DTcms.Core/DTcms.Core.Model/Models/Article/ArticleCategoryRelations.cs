using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 文章类别关联
    /// </summary>
    public class ArticleCategoryRelations
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
        /// 所属分类ID
        /// </summary>
        [Display(Name = "所属分类")]
        public int CategoryId { get; set; }


        /// <summary>
        /// 文章信息
        /// </summary>
        [ForeignKey("ArticleId")]
        public Articles? Article { get; set; }

        /// <summary>
        /// 类别信息
        /// </summary>
        [ForeignKey("CategoryId")]
        public ArticleCategorys? Category { get; set; }
    }
}
