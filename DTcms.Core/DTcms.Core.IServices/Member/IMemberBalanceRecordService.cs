using DTcms.Core.Model.Models;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 会员金额日志接口
    /// </summary>
    public interface IMemberBalanceRecordService : IBaseService
    {
        /// <summary>
        /// 新增余额记录，须同时增加会员余额，检查升级
        /// </summary>
        Task<MemberBalanceRecords> AddAsync(MemberBalanceRecords model);
    }
}
