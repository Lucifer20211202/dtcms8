using System;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 会员组查询参数
    /// </summary>
    public class GroupParameter : BaseParameter
    {
        /// <summary>
        /// 是否默认
        /// </summary>
        public int IsDefault { get; set; } = -1;

        /// <summary>
        /// 是否参与升级
        /// </summary>
        public int IsUpgrade { get; set; } = -1;
    }
}
