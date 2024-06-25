using DTcms.Core.Common.Emums;
using DTcms.Core.Model.Models;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 系统配置接口
    /// </summary>
    public interface IConfigService : IBaseService
    {
        /// <summary>
        /// 根据配置类型返回相应数据
        /// </summary>
        Task<SysConfig?> QueryByTypeAsync(ConfigType configType, WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 根据配置类型返回相应数据(缓存)
        /// </summary>
        Task<SysConfig?> QueryByTypeAsync(string cacheKey, ConfigType configType);
    }
}
