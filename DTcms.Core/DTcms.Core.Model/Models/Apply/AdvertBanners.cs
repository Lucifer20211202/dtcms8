using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 广告内容
    /// </summary>
    public class AdvertBanners
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 所属广告位ID
        /// </summary>
        [Display(Name = "所属广告位")]
        public int AdvertId { get; set; }

        /// <summary>
        /// 广告名称
        /// </summary>
        [Display(Name = "广告名称")]
        [StringLength(128)]
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
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        public DateTime? EndTime { get; set; }

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
        /// 广告位信息
        /// </summary>
        [ForeignKey("AdvertId")]
        public Adverts? Advert { get; set; }
    }
}
