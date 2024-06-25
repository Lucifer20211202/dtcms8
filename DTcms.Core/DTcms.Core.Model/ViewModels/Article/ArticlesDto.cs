using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 文章实体(公共)
    /// </summary>
    public class ArticlesBaseDto
    {
        /// <summary>
        /// 所属站点ID
        /// </summary>
        [Display(Name = "所属站点")]
        public int? SiteId { get; set; }

        /// <summary>
        /// 所属频道ID
        /// </summary>
        [Display(Name = "所属频道")]
        public int ChannelId { get; set; }

        /// <summary>
        /// 调用别名
        /// </summary>
        [Display(Name = "调用标识")]
        [StringLength(128)]
        public string? CallIndex { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        [Required(ErrorMessage = "{0}不可为空")]
        [MaxLength(512, ErrorMessage = "{0}不可超出{1}字符")]
        public string? Title { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [Display(Name = "来源")]
        [MaxLength(128, ErrorMessage = "{0}不可超出{1}字符")]
        public string? Source { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [Display(Name = "作者")]
        [MaxLength(128, ErrorMessage = "{0}不可超出{1}字符")]
        public string? Author { get; set; }

        /// <summary>
        /// 外部链接
        /// </summary>
        [Display(Name = "外部链接")]
        [MaxLength(512, ErrorMessage = "{0}不可超出{1}字符")]
        public string? LinkUrl { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        [Display(Name = "图片地址")]
        [MaxLength(512, ErrorMessage = "{0}不可超出{1}字符")]
        public string? ImgUrl { get; set; }

        /// <summary>
        /// 视频地址
        /// </summary>
        [Display(Name = "视频地址")]
        [MaxLength(512, ErrorMessage = "{0}不可超出{1}字符")]
        public string? VideoUrl { get; set; }

        /// <summary>
        /// SEO标题
        /// </summary>
        [Display(Name = "SEO标题")]
        [MaxLength(512, ErrorMessage = "{0}不可超出{1}字符")]
        public string? SeoTitle { get; set; }

        /// <summary>
        /// SEO关健字
        /// </summary>
        [Display(Name = "SEO关健字")]
        [MaxLength(512, ErrorMessage = "{0}不可超出{1}字符")]
        public string? SeoKeyword { get; set; }

        /// <summary>
        /// SEO描述
        /// </summary>
        [Display(Name = "SEO描述")]
        [MaxLength(512, ErrorMessage = "{0}不可超出{1}字符")]
        public string? SeoDescription { get; set; }

        /// <summary>
        /// 内容摘要
        /// </summary>
        [Display(Name = "内容摘要")]
        [MaxLength(255, ErrorMessage = "{0}不可超出{1}字符")]
        public string? Zhaiyao { get; set; }

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
        /// 状态(0正常1待审2已删)
        /// </summary>
        [Display(Name = "状态")]
        public byte Status { get; set; } = 0;

        /// <summary>
        /// 评论(0禁止1允许)
        /// </summary>
        [Display(Name = "是否评论")]
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
    }

    /// <summary>
    /// 文章实体(显示)
    /// </summary>
    public class ArticlesDto : ArticlesBaseDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public long Id { get; set; }

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
        /// 更新人
        /// </summary>
        [Display(Name = "更新人")]
        public string? UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 类别标题，以逗号分隔
        /// </summary>
        public string? CategoryTitle { get; set; }

        /// <summary>
        /// 标签标题，以逗号分隔
        /// </summary>
        public string? LabelTitle { get; set; }

        /// <summary>
        /// 扩展字段键值
        /// </summary>
        [Display(Name = "扩展字段键值")]
        public Dictionary<string, string>? Fields { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        public string? Content { get; set; }


        /// <summary>
        /// 扩展字段列表
        /// </summary>
        public ICollection<ArticleFieldValuesDto> ArticleFields { get; set; } = [];

        /// <summary>
        /// 会员组权限列表
        /// </summary>
        public ICollection<ArticleGroupsDto> ArticleGroups { get; set; } = [];

        /// <summary>
        /// 类别关联列表
        /// </summary>
        public ICollection<ArticleCategoryRelationsDto> CategoryRelations { get; set; } = [];

        /// <summary>
        /// 标签关联列表
        /// </summary>
        public ICollection<ArticleLabelRelationsDto> LabelRelations { get; set; } = [];

        /// <summary>
        /// 相册列表
        /// </summary>
        public ICollection<ArticleAlbumsDto> ArticleAlbums { get; set; } = [];

        /// <summary>
        /// 附件列表
        /// </summary>
        public ICollection<ArticleAttachsDto> ArticleAttachs { get; set; } = [];
    }

    /// <summary>
    /// 文章实体(列表)
    /// </summary>
    public class ArticlesListDto : ArticlesBaseDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public long Id { get; set; }

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
        /// 更新人
        /// </summary>
        [Display(Name = "更新人")]
        public string? UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 类别标题，以逗号分隔
        /// </summary>
        public string? CategoryTitle { get; set; }

        /// <summary>
        /// 标签标题，以逗号分隔
        /// </summary>
        public string? LabelTitle { get; set; }

        /// <summary>
        /// 扩展字段键值
        /// </summary>
        [Display(Name = "扩展字段键值")]
        public Dictionary<string, string>? Fields { get; set; }


        /// <summary>
        /// 类别关联列表
        /// </summary>
        public ICollection<ArticleCategoryRelationsDto> CategoryRelations { get; set; } = [];

        /// <summary>
        /// 标签关联列表
        /// </summary>
        public ICollection<ArticleLabelRelationsDto> LabelRelations { get; set; } = [];

        /// <summary>
        /// 相册列表
        /// </summary>
        public ICollection<ArticleAlbumsDto> ArticleAlbums { get; set; } = [];

        /// <summary>
        /// 附件列表
        /// </summary>
        public ICollection<ArticleAttachsDto> ArticleAttachs { get; set; } = [];
    }

    /// <summary>
    /// 文章实体(新增)
    /// </summary>
    public class ArticlesAddDto : ArticlesBaseDto
    {
        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "创建人")]
        public string? AddBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime? AddTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        public string? Content { get; set; }


        /// <summary>
        /// 扩展字段列表
        /// </summary>
        public ICollection<ArticleFieldValuesDto> ArticleFields { get; set; } = [];

        /// <summary>
        /// 会员组权限列表
        /// </summary>
        public ICollection<ArticleGroupsDto> ArticleGroups { get; set; } = [];

        /// <summary>
        /// 类别关联列表
        /// </summary>
        public ICollection<ArticleCategoryRelationsDto> CategoryRelations { get; set; } = [];

        /// <summary>
        /// 标签关联列表
        /// </summary>
        public ICollection<ArticleLabelRelationsDto> LabelRelations { get; set; } = [];

        /// <summary>
        /// 相册列表
        /// </summary>
        public ICollection<ArticleAlbumsDto> ArticleAlbums { get; set; } = [];

        /// <summary>
        /// 附件列表
        /// </summary>
        public ICollection<ArticleAttachsDto> ArticleAttachs { get; set; } = [];
    }

    /// <summary>
    /// 文章实体(编辑)
    /// </summary>
    public class ArticlesEditDto : ArticlesBaseDto
    {
        /// <summary>
        /// 更新人
        /// </summary>
        [Display(Name = "更新人")]
        public string? UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime? AddTime { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        public string? Content { get; set; }


        /// <summary>
        /// 扩展字段列表
        /// </summary>
        public ICollection<ArticleFieldValuesDto> ArticleFields { get; set; } = [];

        /// <summary>
        /// 会员组权限列表
        /// </summary>
        public ICollection<ArticleGroupsDto> ArticleGroups { get; set; } = [];

        /// <summary>
        /// 类别关联列表
        /// </summary>
        public ICollection<ArticleCategoryRelationsDto> CategoryRelations { get; set; } = [];

        /// <summary>
        /// 标签关联列表
        /// </summary>
        public ICollection<ArticleLabelRelationsDto> LabelRelations { get; set; } = [];

        /// <summary>
        /// 相册列表
        /// </summary>
        public ICollection<ArticleAlbumsDto> ArticleAlbums { get; set; } = [];

        /// <summary>
        /// 附件列表
        /// </summary>
        public ICollection<ArticleAttachsDto> ArticleAttachs { get; set; } = [];
    }

    /// <summary>
    /// 文章实体(前端显示)
    /// </summary>
    public class ArticlesClientDto : ArticlesBaseDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public long Id { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "创建人")]
        public string? AddBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 类别标题，以逗号分隔
        /// </summary>
        public string? CategoryTitle { get; set; }

        /// <summary>
        /// 标签标题，以逗号分隔
        /// </summary>
        public string? LabelTitle { get; set; }

        /// <summary>
        /// 扩展字段键值
        /// </summary>
        [Display(Name = "扩展字段键值")]
        public Dictionary<string, string>? Fields { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        public string? Content { get; set; }


        /// <summary>
        /// 类别关联列表
        /// </summary>
        public IEnumerable<ArticleCategoryRelationsDto> CategoryRelations { get; set; } = [];

        /// <summary>
        /// 标签关联列表
        /// </summary>
        public IEnumerable<ArticleLabelRelationsDto> LabelRelations { get; set; } = [];

        /// <summary>
        /// 相册列表
        /// </summary>
        public IEnumerable<ArticleAlbumsDto> ArticleAlbums { get; set; } = [];

        /// <summary>
        /// 附件列表(前端)
        /// </summary>
        public IEnumerable<ArticleAttachsClientDto> ArticleAttachs { get; set; } = [];
    }

    /// <summary>
    /// 文章实体(前端列表)
    /// </summary>
    public class ArticlesClientListDto : ArticlesBaseDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public long Id { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "创建人")]
        public string? AddBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 类别标题，以逗号分隔
        /// </summary>
        public string? CategoryTitle { get; set; }

        /// <summary>
        /// 标签标题，以逗号分隔
        /// </summary>
        public string? LabelTitle { get; set; }

        /// <summary>
        /// 扩展字段键值
        /// </summary>
        [Display(Name = "扩展字段键值")]
        public Dictionary<string, string>? Fields { get; set; }


        /// <summary>
        /// 类别关联列表
        /// </summary>
        public IEnumerable<ArticleCategoryRelationsDto> CategoryRelations { get; set; } = [];

        /// <summary>
        /// 标签关联列表
        /// </summary>
        public IEnumerable<ArticleLabelRelationsDto> LabelRelations { get; set; } = [];

        /// <summary>
        /// 相册列表
        /// </summary>
        public IEnumerable<ArticleAlbumsDto> ArticleAlbums { get; set; } = [];

        /// <summary>
        /// 附件列表(前端)
        /// </summary>
        public IEnumerable<ArticleAttachsClientDto> ArticleAttachs { get; set; } = [];
    }
}
