using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 广告内容(显示)
    /// </summary>
    public class AdvertBannersDto: AdvertBannersEditDto
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
        public string? AddBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 广告位名称
        /// </summary>
        public string? AdvertTitle { get; set; }
    }

    /// <summary>
    /// 广告内容(编辑)
    /// </summary>
    public class AdvertBannersEditDto
    {
        /// <summary>
        /// 所属广告位
        /// </summary>
        [Display(Name = "所属广告位")]
        public int AdvertId { get; set; }

        /// <summary>
        /// 广告名称
        /// </summary>
        [Display(Name = "广告名称")]
        [Required(ErrorMessage = "{0}不可为空")]
        [MaxLength(128, ErrorMessage = "{0}最多{1}位字符")]
        public string? Title { get; set; }

        /// <summary>
        /// 广告内容
        /// </summary>
        [Display(Name = "广告内容")]
        public string? Content { get; set; }

        /// <summary>
        /// 上传文件
        /// </summary>
        [Display(Name = "上传文件")]
        [StringLength(512)]
        public string? FilePath { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        [Display(Name = "链接地址")]
        [StringLength(512)]
        public string? LinkUrl { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        [Required(ErrorMessage = "{0}不可为空")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        [Required(ErrorMessage = "{0}不可为空")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        public int SortId { get; set; } = 99;

        /// <summary>
        /// 状态(0关闭1开启)
        /// </summary>
        [Display(Name = "状态")]
        public byte Status { get; set; } = 0;
    }
}
