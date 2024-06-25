using System;
using System.Collections.Generic;
using System.Text;

namespace DTcms.Core.Common.Emums
{
    /// <summary>
    /// 数据库集群策略枚举
    /// </summary>
    public enum DBStrategy
    {
        /// <summary>
        /// 输循策略
        /// </summary>
        Polling,
        /// <summary>
        /// 随机策略
        /// </summary>
        Random
    }
}
