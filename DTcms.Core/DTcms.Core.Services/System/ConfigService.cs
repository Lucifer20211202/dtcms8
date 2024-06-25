using DTcms.Core.Common.Emums;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 系统配置接口实现
    /// </summary>
    public class ConfigService(IDbContextFactory contentFactory, ICacheService cacheService) : BaseService(contentFactory, cacheService), IConfigService
    {
        /// <summary>
        /// 根据配置类型返回相应数据
        /// </summary>
        public async Task<SysConfig?> QueryByTypeAsync(ConfigType configType, WriteRoRead writeAndRead = WriteRoRead.Read)
        {
            _context = _contextFactory.CreateContext(writeAndRead);//连接数据库
            return await _context.Set<SysConfig>().FirstOrDefaultAsync(x => x.Type== configType.ToString());
        }

        /// <summary>
        /// 根据配置类型返回相应数据(缓存)
        /// </summary>
        public async Task<SysConfig?> QueryByTypeAsync(string cacheKey, ConfigType configType)
        {
            string className = typeof(SysConfig).Name; //获取类名的字符串
            string classKey = $"{className}:Show:{cacheKey}";

            return await _cacheService.GetOrSetAsync<SysConfig>(classKey, async () =>
            {
                _context = _contextFactory.CreateContext(WriteRoRead.Read);//连接数据库
                return await _context.Set<SysConfig>().FirstOrDefaultAsync(x => x.Type == configType.ToString());
            });
        }
    }
}
