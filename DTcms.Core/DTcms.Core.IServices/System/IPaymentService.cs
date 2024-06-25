using DTcms.Core.Model.Models;
using System.Linq.Expressions;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 支付方式接口
    /// </summary>
    public interface IPaymentService : IBaseService
    {
        /// <summary>
        /// 批量删除(含站点支付方式删除)
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<Payments, bool>> funcWhere);
    }
}
