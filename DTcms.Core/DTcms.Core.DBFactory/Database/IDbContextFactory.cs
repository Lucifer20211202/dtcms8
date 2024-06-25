using DTcms.Core.Common.Emums;

namespace DTcms.Core.DBFactory.Database
{
    public interface IDbContextFactory
    {
        AppDbContext CreateContext(WriteRoRead writeRoRead);
    }
}
