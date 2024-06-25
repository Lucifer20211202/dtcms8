using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 文章扩展字段值
    /// </summary>
    public class ArticleFieldValues
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
        /// 所属字段ID
        /// </summary>
        [Display(Name = "所属字段")]
        public int FieldId { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        [Display(Name = "字段名")]
        [StringLength(128)]
        public string? FieldName { get; set; }

        /// <summary>
        /// 字段值
        /// </summary>
        [Display(Name = "字段值")]
        public string? FieldValue { get; set; }


        /// <summary>
        /// 文章信息
        /// </summary>
        [ForeignKey("ArticleId")]
        public Articles? Article { get; set; }
    }
}
