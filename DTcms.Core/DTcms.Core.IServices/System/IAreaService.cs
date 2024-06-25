using DTcms.Core.Common.Emums;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using System.Linq.Expressions;

namespace DTcms.Core.IServices
{
    /// <summary>
    /// 全国省市区接口
    /// </summary>
    public interface IAreaService : IBaseService
    {
        /// <summary>
        /// 从缓存中返回所有地区目录树
        /// </summary>
        Task<IEnumerable<AreasDto>> QueryListAsync(string cacheKey, int parentId);

        /// <summary>
        /// 返回所有地区目录树
        /// </summary>
        Task<IEnumerable<AreasDto>> QueryListAsync(int parentId, WriteRoRead writeAndRead = WriteRoRead.Read);

        /// <summary>
        /// 根据条件删除数据(迭代删除)
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<Areas, bool>> funcWhere);

        /// <summary>
        /// 批量导入数据
        /// </summary>
        Task<bool> ImportAsync(List<AreasImportDto> listDto);
    }
}
