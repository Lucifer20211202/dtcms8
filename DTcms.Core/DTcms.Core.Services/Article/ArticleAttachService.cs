using AutoMapper;
using DTcms.Core.Common.Emums;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 文章附件接口实现
    /// </summary>
    public class ArticleAttachService(IDbContextFactory contentFactory, ICacheService cacheService, IUserService userService, IMapper mapper)
        : BaseService(contentFactory, cacheService), IArticleAttachService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IUserService _userService = userService;

        /// <summary>
        /// 更新下载数量
        /// </summary>
        public async Task<bool> UpdateDownCount(long id, WriteRoRead writeAndRead = WriteRoRead.Write)
        {
            _context = _contextFactory.CreateContext(writeAndRead); //连接数据库

            var model = await _context.Set<ArticleAttachs>().FirstOrDefaultAsync(t => t.Id == id);
            if (model == null) return false;
            int userId = _userService.GetUserId();
            model.DownCount++;
            if(userId > 0)
            {
                MemberAttachRecords attachModel = new()
                {
                    AddTime = DateTime.Now,
                    AttachId = id,
                    FileName = model.FileName,
                    UserId = userId
                };
                await _context.Set<MemberAttachRecords>().AddAsync(attachModel);
            }
            return await SaveAsync();
        }
    }
}
