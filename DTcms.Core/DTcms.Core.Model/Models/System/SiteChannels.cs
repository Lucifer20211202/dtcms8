using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 站点频道
    /// </summary>
    public class SiteChannels
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 所属站点ID
        /// </summary>
        [Display(Name = "所属站点")]
        public int SiteId { get; set; }

        /// <summary>
        /// 频道名称
        /// </summary>
        [Display(Name = "频道名称")]
        [StringLength(128)]
        public string? Name { get; set; }

        /// <summary>
        /// 频道标题
        /// </summary>
        [Display(Name = "频道标题")]
        [StringLength(128)]
        public string? Title { get; set; }

        /// <summary>
        /// 是否开启评论
        /// </summary>
        [Display(Name = "是否开启评论")]
        [Range(0, 9)]
        public byte IsComment { get; set; } = 0;

        /// <summary>
        /// 是否开启相册
        /// </summary>
        [Display(Name = "是否开启相册")]
        [Range(0, 9)]
        public byte IsAlbum { get; set; } = 0;

        /// <summary>
        /// 是否开启附件
        /// </summary>
        [Display(Name = "是否开启附件")]
        [Range(0, 9)]
        public byte IsAttach { get; set; } = 0;

        /// <summary>
        /// 是否允许投稿
        /// </summary>
        [Display(Name = "是否允许投稿")]
        [Range(0, 9)]
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
        /// 字段列表
        /// </summary>
        public ICollection<SiteChannelFields> Fields { get; set; } = new List<SiteChannelFields>();

        /// <summary>
        /// 站点信息
        /// </summary>
        [ForeignKey("SiteId")]
        public Sites? Site { get; set; }
    }
}
