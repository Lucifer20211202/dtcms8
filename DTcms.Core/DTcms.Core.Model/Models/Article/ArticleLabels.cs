using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 文章标签
    /// </summary>
    public class ArticleLabels
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 标签标题
        /// </summary>
        [Display(Name = "标签标题")]
        [StringLength(128)]
        public string? Title { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        public int SortId { get; set; } = 99;

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
        [StringLength(30)]
        public string? AddBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;


        /// <summary>
        /// 标签关联列表
        /// </summary>
        public ICollection<ArticleLabelRelations>? LabelRelations { get; set; }
    }
}
