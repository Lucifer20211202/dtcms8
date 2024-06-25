using DTcms.Core.Common.Emums;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 评论点赞接口实现
    /// </summary>
    public class ArticleCommentLikeService(IDbContextFactory contentFactory, ICacheService cacheService, IUserService userService)
        : BaseService(contentFactory, cacheService), IArticleCommentLikeService
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// 更新用户点赞数据
        /// </summary>
        public async Task<int> UserUpdateLikeAsync(int commentId)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库
            int userId = _userService.GetUserId(); //获取当前用户ID

            //获取评论实体
            var commentModel = await _context.Set<ArticleComments>().FirstOrDefaultAsync(x => x.Id == commentId);
            if (commentModel == null)
            {
                return 0;
            }
            //获得点赞实体
            var likeModel = await _context.Set<ArticleCommentLikes>().FirstOrDefaultAsync(x => x.CommentId == commentId && x.UserId == userId);
            if (likeModel == null)
            {
                ArticleCommentLikes like = new()
                {
                    CommentId = commentId,
                    AddTime = DateTime.Now,
                    UserId = userId
                };
                await _context.Set<ArticleCommentLikes>().AddAsync(like);
                //总赞数+1
                commentModel.LikeCount++;
            }
            else
            {
                _context.Set<ArticleCommentLikes>().Remove(likeModel);
                //总赞数-1
                commentModel.LikeCount--;
            }

            //提交保存
            _context.Set<ArticleComments>().Update(commentModel);
            var result = await this.SaveAsync();
            //删除缓存
            if (result)
            {
                await this.RemoveCacheAsync<ArticleCommentLikes>(true);
            }
            return commentModel.LikeCount;
        }
    }
}