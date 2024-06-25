using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.Models
{
    /// <summary>
    /// 文章主表
    /// </summary>
    public class Articles
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 所属站点
        /// </summary>
        [Display(Name = "所属站点")]
        public int SiteId { get; set; }

        /// <summary>
        /// 所属频道
        /// </summary>
        [Display(Name = "所属频道")]
        [ForeignKey("SiteChannel")]
        public int ChannelId { get; set; }

        /// <summary>
        /// 调用标识
        /// </summary>
        [Display(Name = "调用标识")]
        [StringLength(128)]
        public string? CallIndex { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        [Required]
        [StringLength(512)]
        public string? Title { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [Display(Name = "来源")]
        [StringLength(128)]
        public string? Source { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [Display(Name = "作者")]
        [StringLength(128)]
        public string? Author { get; set; }

        /// <summary>
        /// 外部链接
        /// </summary>
        [Display(Name = "外部链接")]
        [StringLength(512)]
        public string? LinkUrl { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        [Display(Name = "图片地址")]
        [StringLength(512)]
        public string? ImgUrl { get; set; }

        /// <summary>
        /// 视频地址
        /// </summary>
        [Display(Name = "视频地址")]
        [StringLength(512)]
        public string? VideoUrl { get; set; }

        /// <summary>
        /// SEO标题
        /// </summary>
        [Display(Name = "SEO标题")]
        [StringLength(255)]
        public string? SeoTitle { get; set; }

        /// <summary>
        /// SEO关健字
        /// </summary>
        [Display(Name = "SEO关健字")]
        [StringLength(512)]
        public string? SeoKeyword { get; set; }

        /// <summary>
        /// SEO描述
        /// </summary>
        [Display(Name = "SEO描述")]
        [StringLength(512)]
        public string? SeoDescription { get; set; }

        /// <summary>
        /// 内容摘要
        /// </summary>
        [Display(Name = "内容摘要")]
        [StringLength(255)]
        public string? Zhaiyao { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        [Column(TypeName = "text")]
        public string? Content { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        public int SortId { get; set; } = 99;

        /// <summary>
        /// 浏览总数
        /// </summary>
        [Display(Name = "浏览总数")]
        public int Click { get; set; } = 0;

        /// <summary>
        /// 状态0正常1待审2已删
        /// </summary>
        [Display(Name = "状态")]
        [Range(0, 9)]
        public byte Status { get; set; } = 0;

        /// <summary>
        /// 评论0禁止1允许
        /// </summary>
        [Display(Name = "是否评论")]
        [Range(0, 9)]
        public byte IsComment { get; set; } = 0;

        /// <summary>
        /// 评论总数
        /// </summary>
        [Display(Name = "评论总数")]
        public int CommentCount { get; set; } = 0;

        /// <summary>
        /// 点赞总数
        /// </summary>
        [Display(Name = "点赞总数")]
        public int LikeCount { get; set; } = 0;

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
        /// 更新人
        /// </summary>
        [Display(Name = "更新人")]
        [StringLength(30)]
        public string? UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        public DateTime? UpdateTime { get; set; }



        /// <summary>
        /// 扩展字段列表
        /// </summary>
        public ICollection<ArticleFieldValues> ArticleFields { get; set; } = new List<ArticleFieldValues>();

        /// <summary>
        /// 会员组权权限列表
        /// </summary>
        public ICollection<ArticleGroups> ArticleGroups { get; set; } = new List<ArticleGroups>();

        /// <summary>
        /// 类别关联列表
        /// </summary>
        public ICollection<ArticleCategoryRelations> CategoryRelations { get; set; } = new List<ArticleCategoryRelations>();

        /// <summary>
        /// 标签关联列表
        /// </summary>
        public ICollection<ArticleLabelRelations> LabelRelations { get; set; } = new List<ArticleLabelRelations>();

        /// <summary>
        /// 相册列表
        /// </summary>
        public ICollection<ArticleAlbums> ArticleAlbums { get; set; } = new List<ArticleAlbums>();

        /// <summary>
        /// 附件列表
        /// </summary>
        public ICollection<ArticleAttachs> ArticleAttachs { get; set; } = new List<ArticleAttachs>();

        /// <summary>
        /// 评论列表
        /// </summary>
        public ICollection<ArticleComments> ArticleComments { get; set; } = new List<ArticleComments>();

        /// <summary>
        /// 频道信息
        /// </summary>
        public SiteChannels? SiteChannel { get; set; }
    }
}
