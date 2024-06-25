using System;
using System.Collections.Generic;
using System.Text;

namespace DTcms.Core.Model.ViewModels
{
    public class ArticleParameter : BaseParameter
    {
        /// <summary>
        /// 分类主键
        /// </summary>
        public int CategoryId { get; set; } = 0;
        /// <summary>
        /// 标签主键
        /// </summary>
        public int LabelId { get; set; } = 0;
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}
