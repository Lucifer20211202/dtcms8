using DTcms.Core.Common.Emums;

namespace DTcms.Core.DBFactory.Database
{
    public class DbContextOption
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DBType DBType { get; set; }
        /// <summary>
        /// 写数据库连接字符串
        /// </summary>
        public string WriteConnection { get; set; } = String.Empty;
        /// <summary>
        /// 读数据库连接字符串
        /// </summary>
        public List<string>? ReadConnectionList { get; set; }
        /// <summary>
        /// 数据库集群策略
        /// </summary>
        public DBStrategy Strategy { get; set; }
    }
}
