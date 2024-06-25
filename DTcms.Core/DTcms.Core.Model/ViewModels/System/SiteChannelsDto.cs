using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 站点频道(显示)
    /// </summary>
    public class SiteChannelsDto : SiteChannelsEditDto
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
        public DateTime? AddTime { get; set; }
    }

    /// <summary>
    /// 站点频道(编辑)
    /// </summary>
    public class SiteChannelsEditDto
    {
        /// <summary>
        /// 所属站点ID
        /// </summary>
        [Display(Name = "所属站点")]
        public int SiteId { get; set; }

        /// <summary>
        /// 频道名称
        /// </summary>
        [Display(Name = "频道名称")]
        [Required(ErrorMessage = "{0}不可为空")]
        [MaxLength(128, ErrorMessage = "{0}不可超出{1}字符")]
        public string? Name { get; set; }

        /// <summary>
        /// 频道标题
        /// </summary>
        [Display(Name = "频道标题")]
        [Required(ErrorMessage = "{0}不可为空")]
        [MaxLength(128, ErrorMessage = "{0}不可超出{1}字符")]
        public string? Title { get; set; }

        /// <summary>
        /// 是否开启评论
        /// </summary>
        [Display(Name = "是否开启评论")]
        public byte IsComment { get; set; } = 0;

        /// <summary>
        /// 是否开启相册
        /// </summary>
        [Display(Name = "是否开启相册")]
        public byte IsAlbum { get; set; } = 0;

        /// <summary>
        /// 是否开启附件
        /// </summary>
        [Display(Name = "是否开启附件")]
        public byte IsAttach { get; set; } = 0;

        /// <summary>
        /// 是否允许投稿
        /// </summary>
        [Display(Name = "是否允许投稿")]
        public byte IsContribute { get; set; } = 0;

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        public int SortId { get; set; } = 99;

        /// <summary>
        /// 状态0正常1禁用
        /// </summary>
        [Display(Name = "状态")]
        public byte Status { get; set; } = 0;


        /// <summary>
        /// 字段列表
        /// </summary>
        public ICollection<SiteChannelFieldsDto> Fields { get; set; } = new List<SiteChannelFieldsDto>();
    }
}