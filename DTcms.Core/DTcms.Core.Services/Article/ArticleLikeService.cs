using DTcms.Core.Common.Emums;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 文章点赞接口实现
    /// </summary>
    public class ArticleLikeService(IDbContextFactory contentFactory, ICacheService cacheService, IUserService userService)
        : BaseService(contentFactory, cacheService), IArticleLikeService
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// 更新用户点赞数据
        /// </summary>
        public async Task<int> UserUpdateLikeAsync(long articleId)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库

            //获取当前用户ID
            int userId = _userService.GetUserId();
            //获得点赞数据实体
            var likeModel = await _context.Set<ArticleLikes>().FirstOrDefaultAsync(x => x.ArticleId == articleId && x.UserId == userId);
            //获取文章旧数据
            var articleModel = await _context.Set<Articles>().FirstOrDefaultAsync(x => x.Id == articleId);
            if (articleModel == null)
            {
                return 0;
            }
            if (likeModel != null)
            {
                //删除文章喜欢记录
                _context.Set<ArticleLikes>().Remove(likeModel);
                //文章点赞数减一
                articleModel.LikeCount--;
            }
            else
            {
                //增加文章喜欢记录
                ArticleLikes like = new()
                {
                    ArticleId = articleId,
                    AddTime = DateTime.Now,
                    UserId = userId
                };
                await _context.Set<ArticleLikes>().AddAsync(like);
                //文章点赞数自增
                articleModel.LikeCount++;
            }
            //提交保存
            var result = await this.SaveAsync();

            //删除缓存
            if (result)
            {
                await this.RemoveCacheAsync<ArticleLikes>(true);
            }

            return articleModel.LikeCount;
        }
    }
}
