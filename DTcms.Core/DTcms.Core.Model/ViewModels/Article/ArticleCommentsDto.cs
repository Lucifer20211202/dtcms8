using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 文章评论(显示)
    /// </summary>
    public class ArticleCommentsDto
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
        /// 所属频道ID
        /// </summary>
        [Display(Name = "所属频道")]
        public int ChannelId { get; set; }

        /// <summary>
        /// 所属文章
        /// </summary>
        [Display(Name = "所属文章")]
        public long ArticleId { get; set; }

        /// <summary>
        /// 首层主键
        /// </summary>
        [Display(Name = "首层主键")]
        public int RootId { get; set; }

        /// <summary>
        /// 所属父类
        /// </summary>
        [Display(Name = "所属父类")]
        public int ParentId { get; set; }

        /// <summary>
        /// 评论用户
        /// </summary>
        [Display(Name = "评论用户")]
        [ForeignKey("User")]
        public int UserId { get; set; } = 0;

        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        public string? UserName { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        [Display(Name = "用户头像")]
        public string? UserAvatar { get; set; }

        /// <summary>
        /// 用户IP
        /// </summary>
        [Display(Name = "用户IP")]
        public string? UserIp { get; set; }

        /// <summary>
        /// 被评论人ID
        /// </summary>
        [Display(Name = "被评论人")]
        public int AtUserId { get; set; } = 0;

        /// <summary>
        /// 被评论人用户名
        /// </summary>
        [Display(Name = "被评论人用户名")]
        public string? AtUserName { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [Display(Name = "评论内容")]
        public string? Content { get; set; }

        /// <summary>
        /// 点赞数量
        /// </summary>
        [Display(Name = "点赞数量")]
        public int LikeCount { get; set; } = 0;

        /// <summary>
        /// 状态0正常1隐藏
        /// </summary>
        [Display(Name = "状态")]
        public byte Status { get; set; } = 0;

        /// <summary>
        /// 已删除0否1是
        /// </summary>
        [Display(Name = "已删除")]
        public byte IsDelete { get; set; } = 0;

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime AddTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 时间描述 多少天前
        /// </summary>
        [Display(Name = "时间描述")]
        public string? DateDescription { get; set; }


        /// <summary>
        /// 文章信息
        /// </summary>
        public ArticlesDto? Article { get; set; }

        /// <summary>
        /// 子类列表
        /// </summary>
        public List<ArticleCommentsDto> Children { get; set; } = [];
    }

    /// <summary>
    /// 文章评论(编辑)
    /// </summary>
    public class ArticleCommentsEditDto
    {
        /// <summary>
        /// 所属站点
        /// </summary>
        [Display(Name = "所属站点")]
        public int SiteId { get; set; }

        /// <summary>
        /// 所属文章
        /// </summary>
        [Display(Name = "所属文章")]
        public long ArticleId { get; set; }

        /// <summary>
        /// 所属父类
        /// </summary>
        [Display(Name = "所属父类")]
        public int ParentId { get; set; }

        /// <summary>
        /// 用户IP
        /// </summary>
        [Display(Name = "用户IP")]
        [StringLength(128)]
        public string? UserIp { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [Display(Name = "评论内容")]
        [Required(ErrorMessage = "{0}不可为空")]
        [MaxLength(512, ErrorMessage = "{0}最多{1}位字符")]
        public string? Content { get; set; }

    }
}
