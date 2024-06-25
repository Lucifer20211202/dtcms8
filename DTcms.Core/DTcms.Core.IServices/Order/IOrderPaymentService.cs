using DTcms.Core.Common.Emums;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using System.Linq.Expressions;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 订单收款接口
    /// </summary>
    public interface IOrderPaymentService : IBaseService
    {
        /// <summary>
        /// 确认收款
        /// </summary>
        Task<bool> ConfirmAsync(string tradeNo);

        /// <summary>
        /// 取消收款
        /// </summary>
        Task<bool> CancelAsync(Expression<Func<OrderPayments, bool>> funcWhere);

        /// <summary>
        /// 取消收款(计划任务)
        /// </summary>
        Task JobCancelAsync();

        /// <summary>
        /// 根据条件删除记录
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<OrderPayments, bool>> funcWheree);

        /// <summary>
        /// 修改支付方式
        /// </summary>
        Task<OrderPayments> PayAsync(OrderPaymentsEditDto modelDto);
    }
}
