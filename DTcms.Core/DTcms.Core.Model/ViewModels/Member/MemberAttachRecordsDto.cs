using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 附件下载记录(显示)
    /// </summary>
    public class MemberAttachRecordsDto : MemberAttachRecordsEditDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 下载时间
        /// </summary>
        [Display(Name = "下载时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 所属用户
        /// </summary>
        [Display(Name = "所属用户")]
        public int UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        [StringLength(128)]
        public string? UserName { get; set; }
    }

    /// <summary>
    /// 附件下载记录(编辑)
    /// </summary>
    public class MemberAttachRecordsEditDto
    {
        /// <summary>
        /// 附件ID
        /// </summary>
        [Display(Name = "附件ID")]
        [Required(ErrorMessage = "{0}不可为空")]
        public long AttachId { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [Display(Name = "文件名称")]
        [Required(ErrorMessage = "{0}不可为空")]
        [StringLength(255)]
        public string? FileName { get; set; }
    }
}
