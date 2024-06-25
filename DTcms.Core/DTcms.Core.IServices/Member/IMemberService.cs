using DTcms.Core.Common.Emums;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using System.Linq.Expressions;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 会员信息接口
    /// </summary>
    public interface IMemberService : IBaseService
    {
        /// <summary>
        /// 获取会员总数量
        /// </summary>
        Task<int> QueryCountAsync(Expression<Func<Members, bool>> funcWhere, WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 获取会员组ID
        /// </summary>
        Task<int> QueryGroupIdAsync(int userId, WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 统计会员总数量列表
        /// </summary>
        Task<IEnumerable<MembersReportDto>> QueryCountListAsync(int top, Expression<Func<Members, bool>> funcWhere,
            WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 添加会员
        /// </summary>
        Task<Members?> AddAsync(MembersEditDto modelDto);

        /// <summary>
        /// 修改会员
        /// </summary>
        Task<bool> UpdateAsync(int userId, MembersEditDto modelDto);

        /// <summary>
        /// 删除会员
        /// </summary>
        Task<bool> DeleteAsync(int userId);
    }
}
