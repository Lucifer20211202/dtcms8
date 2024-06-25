using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 支付方式接口实现
    /// </summary>
    public class PaymentService(IDbContextFactory contentFactory, ICacheService cacheService) : BaseService(contentFactory, cacheService), IPaymentService
    {
        /// <summary>
        /// 批量删除(含站点支付方式删除)
        /// </summary>
        public async Task<bool> DeleteAsync(Expression<Func<Payments, bool>> funcWhere)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);

            var list = await _context.Set<Payments>().Where(funcWhere).ToListAsync()
                ?? throw new ResponseException("数据不存在或已删除");

            //删除支付方式
            foreach (var model in list)
            {
                var removeList = _context.Set<SitePayments>().Where(x => x.SiteId == model.Id);
                _context.Set<SitePayments>().RemoveRange(removeList);
            }
            //删除支付商
            _context.Set<Payments>().RemoveRange(list);
            var result = await this.SaveAsync();

            //删除缓存
            if (result)
            {
                await this.RemoveCacheAsync<SitePayments>(true);
                await this.RemoveCacheAsync<Payments>(true);
            }

            return result;
        }
    }
}
