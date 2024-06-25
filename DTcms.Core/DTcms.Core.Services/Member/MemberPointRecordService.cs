using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 会员积分记录接口实现
    /// </summary>
    public class MemberPointRecordService(IDbContextFactory contentFactory, ICacheService cacheService)
        : BaseService(contentFactory, cacheService), IMemberPointRecordService
    {
        /// <summary>
        /// 新增一条数据，须检查是否正数，正数须同时增加会员积分及经验值，检查升级
        /// </summary>
        public async Task<MemberPointRecords> AddAsync(MemberPointRecords model)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库

            var memberModel = await _context.Set<Members>().Include(x => x.Group).FirstOrDefaultAsync(x => x.UserId == model.UserId);
            if (memberModel == null || memberModel.Group == null)
            {
                throw new ResponseException("会员账户不存在或已删除");
            }
            //如果是负数，检查会员积分是否够扣减
            if (model.Value < 0 && memberModel.Point < (model.Value * -1))
            {
                throw new ResponseException("会员账户积分不足本次扣减");
            }
            memberModel.Point += model.Value; //添加积分

            //如果是正数则增加经验值
            if (model.Value > 0)
            {
                memberModel.Exp += model.Value; //增加经验值

                //检查有无可升级的会员组
                var upgradeGroupModel = await _context.Set<MemberGroups>()
                    .OrderBy(x => x.Grade)
                    .FirstOrDefaultAsync(x => x.IsUpgrade == 1 && x.Id != memberModel.GroupId && x.Grade > memberModel.Group.Grade && x.Exp <= memberModel.Exp);

                if (upgradeGroupModel != null)
                {
                    memberModel.GroupId = upgradeGroupModel.Id;
                }
            }

            //升级会员组
            _context.Set<Members>().Update(memberModel);
            //新增积分记录
            await _context.Set<MemberPointRecords>().AddAsync(model);
            await this.SaveAsync();

            return model;
        }
    }
}