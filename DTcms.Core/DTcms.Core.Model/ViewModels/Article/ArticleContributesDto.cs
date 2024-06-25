using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 文章投稿(公共)
    /// </summary>
    public class ArticleContributesBaseDto
    {
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
        /// 文章标题
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
        /// 图片地址
        /// </summary>
        [Display(Name = "图片地址")]
        [MaxLength(512, ErrorMessage = "{0}不可超出{1}字符")]
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
        public string? Zhaiyao { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        public string? Content { get; set; }

        /// <summary>
        /// 投稿用户
        /// </summary>
        [Display(Name = "投稿用户")]
        public int UserId { get; set; } = 0;

        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        public string? UserName { get; set; }

        /// <summary>
        /// 状态(0待审1通过2返回)
        /// </summary>
        [Display(Name = "状态")]
        public byte Status { get; set; } = 0;

    }
    /// <summary>
    /// 文章投稿(显示)
    /// </summary>
    public class ArticleContributesDto: ArticleContributesBaseDto
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
        /// 更新人
        /// </summary>
        [Display(Name = "更新人")]
        public string? UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        public DateTime? UpdateTime { get; set; }
    }

    /// <summary>
    /// 文章投稿(添加)
    /// </summary>
    public class ArticleContributesAddDto : ArticleContributesBaseDto
    {
        /// <summary>
        /// 扩展字段值
        /// </summary>
        public List<ArticleContributeFieldsDto>? Fields { get; set; }

        /// <summary>
        /// 相册
        /// </summary>
        public List<ArticleAlbumsDto>? Albums { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public List<ArticleAttachsDto>? Attachs { get; set; }
    }

    /// <summary>
    /// 文章投稿(编辑)
    /// </summary>
    public class ArticleContributesEditDto : ArticleContributesBaseDto
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
        /// 扩展字段值
        /// </summary>
        public List<ArticleContributeFieldsDto>? Fields { get; set; }
        /// <summary>
        /// 相册
        /// </summary>
        public List<ArticleAlbumsDto>? Albums { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public List<ArticleAttachsDto>? Attachs { get; set; }

        /// <summary>
        /// 所属分类
        /// </summary>
        public string[]? Categorys { get; set; }
    }

    /// <summary>
    /// 文章投稿编辑展示(查看)
    /// </summary>
    public class ArticleContributesViewDto : ArticleContributesBaseDto
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
        /// 扩展字段
        /// </summary>
        public List<ArticleContributeFieldsDto>? Fields { get; set; }

        /// <summary>
        /// 相册
        /// </summary>
        public List<ArticleAlbumsDto>? Albums { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public List<ArticleAttachsDto>? Attachs { get; set; }
    }

}
