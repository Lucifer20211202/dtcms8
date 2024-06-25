using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTcms.Core.Common.Emums
{
    /// <summary>
    /// 用户角色类型枚举
    /// </summary>
    public enum RoleType
    {
        /// <summary>
        /// 普通用户
        /// </summary>
        Member = 0,
        /// <summary>
        /// 系统管理员
        /// </summary>
        Admin = 1,
        /// <summary>
        /// 超级管理员
        /// </summary>
        SuperAdmin = 2
    }
}
