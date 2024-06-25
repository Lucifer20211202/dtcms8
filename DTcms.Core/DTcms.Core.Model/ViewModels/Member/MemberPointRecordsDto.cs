using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 积分记录(显示)
    /// </summary>
    public class MemberPointRecordsDto : MemberPointRecordsEditDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        public string? UserName { get; set; }

        /// <summary>
        /// 当前积分
        /// </summary>
        [Display(Name = "当前积分")]
        public int CurrPoint { get; set; }
    }

    /// <summary>
    /// 积分记录(编辑)
    /// </summary>
    public class MemberPointRecordsEditDto
    {
        /// <summary>
        /// 所属用户ID
        /// </summary>
        [Display(Name = "所属用户")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int UserId { get; set; }

        /// <summary>
        /// 增减积分
        /// </summary>
        [Display(Name = "增减积分")]
        [Required(ErrorMessage = "{0}不可为空")]
        public int Value { get; set; } = 0;

        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        [StringLength(512)]
        public string? Description { get; set; }
    }
}
