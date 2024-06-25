using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace DTcms.Core.Common.Helpers
{
    /// <summary>
    /// appsettings.json操作类
    /// </summary>
    public class Appsettings
    {
        static IConfiguration? configuration { get; set; }
        static string? contentPath { get; set; }

        public Appsettings(string contentPath)
        {
            string Path = "appsettings.json";

            //如果你把配置文件 是 根据环境变量来分开了，可以这样写
            //Path = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";

            configuration = new ConfigurationBuilder()
               .SetBasePath(contentPath)
               .Add(new JsonConfigurationSource { Path = Path, Optional = false, ReloadOnChange = true })//这样的话，可以直接读目录里的json文件，而不是 bin 文件夹下的，所以不用修改复制属性
               .Build();
        }

        /// <summary>
        /// 封装要操作的字符
        /// </summary>
        /// <param name="sections">节点配置</param>
        /// <returns>String</returns>
        public static string GetValue(params string[] sections)
        {
            return configuration?[string.Join(":", sections)] ?? String.Empty;
        }

        /// <summary>
        /// 读取节点转换为T类型
        /// </summary>
        /// <typeparam name="T">要转换成的T类型</typeparam>
        /// <param name="sections">节点配置</param>
        /// <returns>T</returns>
        public static T? ToObject<T>(params string[] sections) where T : class
        {
            try
            {
                if (sections.Any())
                {
                    return configuration?.GetSection(string.Join(":", sections)).Get<T>();
                }
            }
            catch (Exception) { }

            return null;
        }
    }
}