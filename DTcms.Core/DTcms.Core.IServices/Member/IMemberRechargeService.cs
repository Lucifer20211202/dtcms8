using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using System.Linq.Expressions;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 会员充值接口
    /// </summary>
    public interface IMemberRechargeService : IBaseService
    {
        /// <summary>
        /// 新增一条记录
        /// </summary>
        Task<OrderPayments> AddAsync(MemberRechargesEditDto modelDto);

        /// <summary>
        /// 根据条件删除记录
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<MemberRecharges, bool>> funcWhere);

        /// <summary>
        /// 获取收款总金额
        /// </summary>
        Task<decimal> QueryAmountAsync(Expression<Func<OrderPayments, bool>> funcWhere);
    }
}
