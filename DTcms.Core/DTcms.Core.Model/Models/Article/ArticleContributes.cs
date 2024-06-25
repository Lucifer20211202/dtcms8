using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 文章投稿
    /// </summary>
    public class ArticleContributes
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
        /// 所属频道ID
        /// </summary>
        [Display(Name = "所属频道")]
        public int ChannelId { get; set; }

        /// <summary>
        /// 投稿用户ID
        /// </summary>
        [Display(Name = "投稿用户")]
        public int UserId { get; set; }

        /// <summary>
        /// 投稿用户名
        /// </summary>
        [Display(Name = "用户名")]
        [StringLength(128)]
        public string? UserName { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        [Display(Name = "文章标题")]
        [StringLength(512)]
        public string? Title { get; set; }

        /// <summary>
        /// 文章来源
        /// </summary>
        [Display(Name = "文章来源")]
        [StringLength(128)]
        public string? Source { get; set; }

        /// <summary>
        /// 文章作者
        /// </summary>
        [Display(Name = "文章作者")]
        [StringLength(128)]
        public string? Author { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        [Display(Name = "图片地址")]
        [StringLength(512)]
        public string? ImgUrl { get; set; }

        /// <summary>
        /// 扩展字段集合
        /// </summary>
        [Display(Name = "扩展字段集合")]
        public string? FieldMeta { get; set; }

        /// <summary>
        /// 相册扩展字段值
        /// </summary>
        [Display(Name = "相册扩展字段值")]
        public string? AlbumMeta { get; set; }

        /// <summary>
        /// 附件扩展字段值
        /// </summary>
        [Display(Name = "附件扩展字段值")]
        public string? AttachMeta { get; set; }

        /// <summary>
        /// 内容摘要
        /// </summary>
        [Display(Name = "内容摘要")]
        [StringLength(255)]
        public string? Zhaiyao { get; set; }

        /// <summary>
        /// 文章内容
        /// </summary>
        [Display(Name = "文章内容")]
        [Column(TypeName = "text")]
        public string? Content { get; set; }

        /// <summary>
        /// 状态(0待审1通过2返回)
        /// </summary>
        [Display(Name = "状态")]
        public byte Status { get; set; } = 0;

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新人
        /// </summary>
        [Display(Name = "更新人")]
        [StringLength(128)]
        public string? UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        public DateTime? UpdateTime { get; set; }


        /// <summary>
        /// 频道信息
        /// </summary>
        [ForeignKey("ChannelId")]
        public SiteChannels? Channel { get; set; }
    }
}
