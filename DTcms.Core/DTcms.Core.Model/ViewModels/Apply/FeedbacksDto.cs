using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 留言反馈(显示)
    /// </summary>
    public class FeedbacksDto: FeedbacksEditDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

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
        /// 回复人
        /// </summary>
        [Display(Name = "回复人")]
        [StringLength(128)]
        public string? ReplyBy { get; set; }

        /// <summary>
        /// 回复时间
        /// </summary>
        [Display(Name = "回复时间")]
        public DateTime? ReplyTime { get; set; }
    }

    /// <summary>
    /// 留言反馈(编辑)
    /// </summary>
    public class FeedbacksEditDto
    {
        /// <summary>
        /// 站点ID
        /// </summary>
        [Display(Name = "所属站点")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int SiteId { get; set; }

        /// <summary>
        /// 留言内容
        /// </summary>
        [Display(Name = "留言内容")]
        [Required(ErrorMessage = "{0}不可为空")]
        public string? Content { get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        [Display(Name = "回复内容")]
        public string? ReplyContent { get; set; }

        /// <summary>
        /// 状态(0未审核1正常)
        /// </summary>
        [Display(Name = "状态")]
        public byte Status { get; set; } = 0;
    }

    /// <summary>
    /// 留言反馈(前端)
    /// </summary>
    public class FeedbacksClientDto : VerifyCode
    {
        /// <summary>
        /// 站点ID
        /// </summary>
        [Display(Name = "所属站点")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int SiteId { get; set; }

        /// <summary>
        /// 留言内容
        /// </summary>
        [Display(Name = "留言内容")]
        [Required(ErrorMessage = "{0}不可为空")]
        public string? Content { get; set; }
    }
}
