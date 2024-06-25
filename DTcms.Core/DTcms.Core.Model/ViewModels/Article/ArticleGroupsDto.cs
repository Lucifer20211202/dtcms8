using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    public class ArticleGroupsDto
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Display(Name = "自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 所属文章ID
        /// </summary>
        [Display(Name = "所属文章")]
        public long ArticleId { get; set; }

        /// <summary>
        /// 所属会员组ID
        /// </summary>
        [Display(Name = "所属会员组")]
        public int GroupId { get; set; }
    }
}
