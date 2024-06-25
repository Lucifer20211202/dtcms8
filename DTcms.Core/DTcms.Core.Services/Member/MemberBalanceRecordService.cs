using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 会员金额日志接口实现
    /// </summary>
    public class MemberBalanceRecordService(IDbContextFactory contentFactory, ICacheService cacheService)
        : BaseService(contentFactory, cacheService), IMemberBalanceRecordService
    {
        /// <summary>
        /// 新增余额记录，须同时增加会员余额，检查升级
        /// </summary>
        public async Task<MemberBalanceRecords> AddAsync(MemberBalanceRecords model)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库

            var memberModel = await _context.Set<Members>().Include(x => x.Group).FirstOrDefaultAsync(x => x.UserId == model.UserId);
            if (memberModel == null || memberModel.Group == null)
            {
                throw new ResponseException("会员账户不存在或已删除");
            }
            //如果是负数，检查会员余额是否够扣减
            if (model.Value < 0 && memberModel.Amount < (model.Value * -1))
            {
                throw new ResponseException("会员账户余额不足本次扣减");
            }
            memberModel.Amount += model.Value; //添减余额

            //如果是正数则检查是否需要升级
            if (model.Value > 0)
            {
                //检查有无可升级的会员组
                var upgradeGroupModel = await _context.Set<MemberGroups>()
                    .OrderBy(x => x.Grade)
                    .FirstOrDefaultAsync(x => x.IsUpgrade == 1 && x.Id != memberModel.GroupId && x.Grade > memberModel.Group.Grade && x.Amount <= model.Value);

                if (upgradeGroupModel != null)
                {
                    memberModel.GroupId = upgradeGroupModel.Id;
                }
            }

            //升级会员组
            _context.Set<Members>().Update(memberModel);
            //新增余额记录，更新用户余额
            await _context.Set<MemberBalanceRecords>().AddAsync(model);
            await this.SaveAsync();

            return model;
        }
    }
}
