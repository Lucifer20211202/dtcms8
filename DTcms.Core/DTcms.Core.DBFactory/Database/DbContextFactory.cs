using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;

namespace DTcms.Core.DBFactory.Database
{
    public class DbContextFactory : IDbContextFactory
    {
        private AppDbContext? content;
        private readonly DbContextOption? options;

        private static int _iSeed = 0;
        private static bool _isSet = true;
        private static readonly object _ObjectisSet_Look = new();

        public DbContextFactory()
        {
            //读取配置文件数据库连接字符串
            options = Appsettings.ToObject<DbContextOption>(new string[] { "ConnectionStrings" });
            //保证第一次初始化时对其赋值
            if (_isSet)
            {
                lock (_ObjectisSet_Look)
                {
                    if (_isSet)
                    {
                        _iSeed = options?.ReadConnectionList?.Count ?? 0;
                        _isSet = false;
                    }
                }
            }
        }

        public AppDbContext CreateContext(WriteRoRead writeRoRead)
        {
            switch (writeRoRead)
            {
                ///写数据
                case WriteRoRead.Write:
                    content = new AppDbContext(options?.DBType, options?.WriteConnection);
                    break;
                //读数据
                case WriteRoRead.Read:
                    content = new AppDbContext(options?.DBType, QueryStrategy());
                    //如果是读则不踪，优化性能
                    content.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
                    break;
            }
            if (content == null)
            {
                throw new InvalidOperationException();
            }
            return content;
        }

        #region 私有辅助函数
        /// <summary>
        /// 选择策略
        /// </summary>
        private string QueryStrategy()
        {
            switch (options?.Strategy)
            {
                case DBStrategy.Polling:
                    //轮循策略
                    return Polling();
                case DBStrategy.Random:
                    //随机策略
                    return Random();
                default:
                    throw new Exception("分库查询策略不存在");
            }
        }

        /// <summary>
        /// 随机策略
        /// </summary>
        private string Random()
        {
            int i = new Random().Next(0, options?.ReadConnectionList?.Count ?? 0);
            return options?.ReadConnectionList?[i] ?? String.Empty;
        }

        /// <summary>
        /// 轮循策略
        /// </summary>
        /// <returns></returns>
        private string Polling()
        {
            int count = options?.ReadConnectionList?.Count ?? 0;
            int i = _iSeed++ % count;
            return options?.ReadConnectionList?[i] ?? String.Empty;
        }
        #endregion
    }
}
