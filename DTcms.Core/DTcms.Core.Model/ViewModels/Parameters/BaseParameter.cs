using System;
using System.Collections.Generic;
using System.Text;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 基本参数
    /// </summary>
    public class BaseParameter
    {
        /// <summary>
        /// 所属站点
        /// </summary>
        public int SiteId { get; set; }
        /// <summary>
        /// 查询关健字
        /// </summary>
        public string? Keyword { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; } = -1;
        /// <summary>
        /// 排序
        /// </summary>
        public string? OrderBy { get; set; }
        /// <summary>
        /// 显示字段,以逗号分隔
        /// </summary>
        public string? Fields { get; set; }
    }
}
