using DTcms.Core.Model.Models;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 会员积分记录接口
    /// </summary>
    public interface IMemberPointRecordService : IBaseService
    {
        /// <summary>
        /// 新增一条数据，须检查是否正数，正数须同时增加会员积分及经验值，检查升级
        /// </summary>
        Task<MemberPointRecords> AddAsync(MemberPointRecords model);
    }
}
