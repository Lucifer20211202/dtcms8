using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 广告位(显示)
    /// </summary>
    public class AdvertsDto: AdvertsEditDto
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
        /// 广告内容列表
        /// </summary>
        public ICollection<AdvertBannersDto> Banners { get; set; } = new List<AdvertBannersDto>();
    }

    /// <summary>
    /// 广告位(编辑)
    /// </summary>
    public class AdvertsEditDto
    {
        /// <summary>
        /// 站点ID
        /// </summary>
        [Display(Name = "站点ID")]
        public int SiteId { get; set; }

        /// <summary>
        /// 调用标识
        /// </summary>
        [Display(Name = "调用标识")]
        [Required(ErrorMessage = "{0}不可为空")]
        [MaxLength(128, ErrorMessage = "{0}最多{1}位字符")]
        public string? CallIndex { get; set; }

        /// <summary>
        /// 广告位名称
        /// </summary>
        [Display(Name = "广告位名称")]
        [Required(ErrorMessage = "{0}不可为空")]
        [MaxLength(128, ErrorMessage = "{0}最多{1}位字符")]
        public string? Title { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        public int SortId { get; set; } = 99;
    }
}
