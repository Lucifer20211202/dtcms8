using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Extensions;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 订单收款接口实现
    /// </summary>
    public class OrderPaymentService(IDbContextFactory contentFactory, ICacheService cacheService, 
        ILogger<OrderPaymentService> logger) : BaseService(contentFactory, cacheService), IOrderPaymentService
    {
        private readonly ILogger<OrderPaymentService> _logger = logger;

        /// <summary>
        /// 确认收款
        /// </summary>
        public async Task<bool> ConfirmAsync(string tradeNo)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库
            //检查收款单
            var model = await _context.Set<OrderPayments>()
                .Include(x => x.SitePayment)
                .Include(x => x.MemberRecharge)
                .FirstOrDefaultAsync(x => x.TradeNo == tradeNo)
                ?? throw new ResponseException("订单不存在或已删除");
            //如果已经支付则直接返回
            if (model.Status == 2)
            {
                return true;
            }
            //如果已退款则不能支付
            if (model.Status == 3)
            {
                throw new ResponseException("订单已退款无法确认收款");
            }
            //如果已取消则不能支付
            if (model.Status == 4)
            {
                throw new ResponseException("订单已取消无法确认收款");
            }
            //查找会员账户
            var memberModel = await _context.Set<Members>()
                .Include(x => x.User)
                .Include(x => x.Group)
                .FirstOrDefaultAsync(x => x.UserId == model.UserId)
                ?? throw new ResponseException("会员账户不存在或已删除");

            //开始处理订单，充值订单
            if (model.MemberRecharge == null)
            {
                throw new ResponseException("没有找到充值订单，请重试");
            }
            var currAmount = memberModel.Amount; //记住当前余额
            memberModel.Amount += model.PaymentAmount; //增加余额
                                                       //检查有无可升级的会员组
            if (memberModel.Group != null)
            {
                //检查有无可升级的会员组
                var upgradeGroupModel = await _context.Set<MemberGroups>()
                    .OrderBy(x => x.Grade)
                    .FirstOrDefaultAsync(x => x.IsUpgrade == 1 && x.Id != memberModel.GroupId && x.Grade > memberModel.Group.Grade && x.Amount <= model.PaymentAmount);
                //修改会员组
                if (upgradeGroupModel != null)
                {
                    memberModel.GroupId = upgradeGroupModel.Id;
                }
            }
            //添加消费记录
            await _context.Set<MemberBalanceRecords>().AddAsync(new()
            {
                UserId = model.UserId,
                UserName = memberModel.User?.UserName,
                CurrAmount = currAmount,
                Value = model.PaymentAmount,
                Description = $"在线充值，充值交易号:{model.TradeNo}",
                AddTime = DateTime.Now
            });
            //更新会员信息
            _context.Set<Members>().Update(memberModel);
            //更新收款单
            model.MemberRecharge.Status = 1;
            model.Status = 2;
            model.CompleteTime = DateTime.Now;
            _context.Set<OrderPayments>().Update(model);
            //保存到数据库
            return await this.SaveAsync();
        }

        /// <summary>
        /// 取消收款
        /// </summary>
        public async Task<bool> CancelAsync(Expression<Func<OrderPayments, bool>> funcWhere)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库
            //检查收款单
            var model = await _context.Set<OrderPayments>()
                .Include(x => x.MemberRecharge)
                .FirstOrDefaultAsync(funcWhere)
                ?? throw new ResponseException("交易记录不存在或已删除");

            //如果已经付款或退款取消
            if (model.Status == 2)
            {
                throw new ResponseException("交易已经付款，无法进行取消");
            }
            if (model.Status == 3)
            {
                throw new ResponseException("交易已经退款，无法重复操作");
            }
            if (model.Status == 4)
            {
                throw new ResponseException("交易已经取消，无法重复操作");
            }

            //开始处理订单，充值订单
            if (model.MemberRecharge == null)
            {
                throw new ResponseException("没有找到充值订单，请重试");
            }
            model.MemberRecharge.Status = 2;

            //修改收款状态
            model.Status = 4;
            model.CompleteTime = DateTime.Now;
            _context.Set<OrderPayments>().Update(model);
            //保存到数据库
            return await this.SaveAsync();
        }

        /// <summary>
        /// 取消收款(计划任务)
        /// </summary>
        public async Task JobCancelAsync()
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库
            //检查收款单
            var list = await _context.Set<OrderPayments>()
                .Include(x => x.MemberRecharge)
                .Where(x => x.Status == 1 && x.EndTime.HasValue && x.EndTime.Value <= DateTime.Now)
                .ToListAsync();
            if (list == null || list.Count == 0)
            {
                return;
            }
            //循环处理
            foreach (var model in list)
            {
                //判断收款类型，充值订单
                if (model.MemberRecharge == null)
                {
                    throw new ResponseException("没有找到充值订单，请重试");
                }
                model.MemberRecharge.Status = 2;

                //修改收款状态
                model.Status = 4;
                model.CompleteTime = DateTime.Now;
                _context.Set<OrderPayments>().Update(model);
                //记录日志到文件
                _logger.LogInformation($"定时任务：取消收款单，交易号：{model.TradeNo}，时间：{DateTime.Now}");
            }
            //保存到数据库
            var result = await this.SaveAsync();
            //记录日志到文件
            if (result)
            {
                _logger.LogInformation("--------------------以上任务执行成功------------------------");
            }
            else
            {
                _logger.LogInformation("--------------------以上任务执行失败------------------------");
            }
        }

        /// <summary>
        /// 根据条件删除记录
        /// </summary>
        public async Task<bool> DeleteAsync(Expression<Func<OrderPayments, bool>> funcWhere)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库
            var list = await _context.Set<OrderPayments>()
                .Include(x => x.MemberRecharge)
                .Where(funcWhere).ToListAsync();
            if (list == null)
            {
                return false;
            }
            _context.Set<OrderPayments>().RemoveRange(list);
            return await this.SaveAsync();
        }

        /// <summary>
        /// 提交修改支付方式
        /// </summary>
        public async Task<OrderPayments> PayAsync(OrderPaymentsEditDto modelDto)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库
            //根据ID获取记录
            var model = await _context.Set<OrderPayments>()
                .FirstOrDefaultAsync(x => x.Id == modelDto.Id
                && x.UserId == modelDto.UserId
                && (!x.EndTime.HasValue || x.EndTime >= DateTime.Now))
                ?? throw new ResponseException("交易单号有误或已过期");

            //判断支付方式是否改变
            if (model.PaymentId == modelDto.PaymentId)
            {
                return model;
            }
            //判断是否已支付
            if (model.Status != 1)
            {
                throw new ResponseException("订单已支付或已取消，请确认后操作");
            }
            //判断是否已经选择了线下支付
            if (model.PaymentType == 1)
            {
                throw new ResponseException("订单已选择线下支付，无法变更");
            }
            //查询支付方式
            var paymentModel = await _context.Set<SitePayments>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == modelDto.PaymentId)
                ?? throw new ResponseException("支付方式有误，请确认后操作");
            //不允许线下支付
            if (paymentModel.Type == "cash")
            {
                throw new ResponseException("该订单不支持线下支付，请重试");
            }
            
            model.PaymentId = paymentModel.Id;
            model.PaymentType = paymentModel.Type == "cash" ? (byte)1 : (byte)0;
            model.PaymentTitle = paymentModel.Title;

            //保存到数据库
            _context.Set<OrderPayments>().Update(model);
            await this.SaveAsync();

            return model;
        }
    }
}
