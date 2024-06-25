using System;
using System.Collections.Generic;
using System.Text;

namespace DTcms.Core.Model.ViewModels
{
    /// <summary>
    /// 第三方授权查询参数
    /// </summary>
    public class OAuthParameter : BaseParameter
    {
        /// <summary>
        /// 接口类型
        /// 示例：web(网站)|mp(小程序)|app
        /// </summary>
        public string? Types { get; set; }
    }
}
