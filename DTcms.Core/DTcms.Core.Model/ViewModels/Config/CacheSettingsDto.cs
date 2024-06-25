using System.ComponentModel.DataAnnotations;

namespace DTcms.Core.Model.ViewModels
{
    public class CacheSettingsDto
    {
        /// <summary>
        /// 是否启用缓存
        /// </summary>
        [Display(Name = "是否启用缓存")]
        public bool Enabled { get; set; }

        /// <summary>
        /// 缓存服务器
        /// 可选项:"Redis"或"InMemory"
        /// </summary>
        [Display(Name = "缓存服务器")]
        public string? Provider { get; set; }

        /// <summary>
        /// Redis连接地址
        /// </summary>
        [Display(Name = "Redis连接地址")]
        public string? RedisServer { get; set; }

        /// <summary>
        /// 缓存实例名称
        /// </summary>
        [Display(Name = "缓存实例名称")]
        public string? InstanceName { get; set; }
    }
}
