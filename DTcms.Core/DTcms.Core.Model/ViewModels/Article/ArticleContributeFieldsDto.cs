using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 文章投稿扩展字段(显示)
    /// </summary>
    public class ArticleContributeFieldsDto
    {
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

    }
}