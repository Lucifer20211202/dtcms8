using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 会员充值接口实现
    /// </summary>
    public class MemberRechargeService(IDbContextFactory contentFactory, ICacheService cacheService)
        : BaseService(contentFactory, cacheService), IMemberRechargeService
    {
        /// <summary>
        /// 新增一条记录
        /// </summary>
        public async Task<OrderPayments> AddAsync(MemberRechargesEditDto modelDto)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write); //连接数据库

            //检查会员是否存在
            var memberModel = await _context.Set<Members>().Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == modelDto.UserId)
                ?? throw new ResponseException("会员账户不存在或已删除");
            //检查支付方式是否存在
            var paymentModel = await _context.Set<SitePayments>().Include(x => x.Payment).FirstOrDefaultAsync(x => x.Id == modelDto.PaymentId);
            if (paymentModel == null || paymentModel.Payment == null)
            {
                throw new ResponseException("支付方式不存在或已删除");
            }

            //新增一条充值记录
            var orderPayment = new OrderPayments()
            {
                UserId = modelDto.UserId.GetValueOrDefault(),
                TradeNo = $"RN{UtilConvert.GetGuidToNumber()}",
                PaymentId = modelDto.PaymentId,
                PaymentTitle = paymentModel.Title,
                PaymentType = paymentModel.Payment.Type == 0 ? (byte)1 : (byte)0,
                PaymentAmount = modelDto.Amount,
                Status = 1,
                AddTime = DateTime.Now
            };
            var model = new MemberRecharges()
            {
                UserId = modelDto.UserId.GetValueOrDefault(),
                UserName = memberModel.User?.UserName,
                Amount = modelDto.Amount,
                OrderPayments = new List<OrderPayments>()
                {
                    orderPayment
                }
            };

            //保存到数据库
            await _context.Set<MemberRecharges>().AddAsync(model);
            await this.SaveAsync();

            return orderPayment;
        }

        /// <summary>
        /// 根据条件删除记录
        /// </summary>
        public async Task<bool> DeleteAsync(Expression<Func<MemberRecharges, bool>> funcWhere)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write); //连接数据库

            //查找主表
            var list = await _context.Set<MemberRecharges>().Include(x => x.OrderPayments).Where(funcWhere).ToListAsync();
            if (list == null) return false;

            //提交执行删除
            _context.Set<MemberRecharges>().RemoveRange(list);
            return await this.SaveAsync();
        }

        /// <summary>
        /// 获取收款总金额
        /// </summary>
        public async Task<decimal> QueryAmountAsync(Expression<Func<OrderPayments, bool>> funcWhere)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Read);//连接数据库
            var result = await _context.Set<OrderPayments>()
                .Where(funcWhere).OrderByDescending(x => x.AddTime)
                .GroupBy(x => new { x.AddTime.Month, x.AddTime.Day })
                .Select(g => g.Sum(x => x.PaymentAmount)).FirstOrDefaultAsync();
            return result;
        }
    }
}
