using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 文章类别(显示)
    /// </summary>
    public class ArticleCategorysDto : ArticleCategorysEditDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 所属站点ID
        /// </summary>
        [Display(Name = "所属站点")]
        public int SiteId { get; set; }

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
        /// 子类列表
        /// </summary>
        public List<ArticleCategorysDto> Children { get; set; } = new List<ArticleCategorysDto>();
    }

    /// <summary>
    /// 文章类别(编辑)
    /// </summary>
    public class ArticleCategorysEditDto
    {
        /// <summary>
        /// 所属频道ID
        /// </summary>
        [Display(Name = "所属频道")]
        public int ChannelId { get; set; }

        /// <summary>
        /// 所属父类ID
        /// </summary>
        [Display(Name = "所属父类")]
        public int ParentId { get; set; } = 0;

        /// <summary>
        /// 调用别名
        /// </summary>
        [Display(Name = "调用别名")]
        [MaxLength(128, ErrorMessage = "{0}不可超出{1}字符")]
        public string? CallIndex { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        [Required(ErrorMessage = "{0}不可为空")]
        [MaxLength(128, ErrorMessage = "{0}不可超出{1}字符")]
        [MinLength(1, ErrorMessage = "{0}不可小于{1}字符")]
        public string? Title { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        [Display(Name = "图片地址")]
        [MaxLength(512, ErrorMessage = "{0}不可超出{1}字符")]
        public string? ImgUrl { get; set; }

        /// <summary>
        /// 外部链接
        /// </summary>
        [Display(Name = "外部链接")]
        [MaxLength(512, ErrorMessage = "{0}不可超出{1}字符")]
        public string? LinkUrl { get; set; }

        /// <summary>
        /// 内容介绍
        /// </summary>
        [Display(Name = "内容介绍")]
        public string? Content { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [Display(Name = "排序数字")]
        public int SortId { get; set; } = 99;

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
        /// 状态0正常1禁用
        /// </summary>
        [Display(Name = "状态")]
        public byte Status { get; set; } = 0;
    }

    /// <summary>
    /// 商品分类(前端)
    /// </summary>
    public class ArticleCategorysClientDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        public string? Title { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        [Display(Name = "图片地址")]
        public string? ImgUrl { get; set; }

        /// <summary>
        /// 文章列表
        /// </summary>
        public IEnumerable<ArticlesClientDto> Data { get; set; } = [];
    }
}
