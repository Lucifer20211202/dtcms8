using AutoMapper;
using DTcms.Core.Common.Emums;
using DTcms.Core.Common.Helpers;
using DTcms.Core.DBFactory.Database;
using DTcms.Core.IServices;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DTcms.Core.Services
{
    /// <summary>
    /// 文章投稿接口实现
    /// </summary>
    public class ArticleContributeService(IDbContextFactory contentFactory, ICacheService cacheService, IUserService userService, IMapper mapper)
        : BaseService(contentFactory, cacheService), IArticleContributeService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IUserService _userService = userService;

        /// <summary>
        /// 修改一条记录
        /// </summary>
        public async Task<bool> UpdateAsync(int id, ArticleContributesEditDto modelDto)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库

            var siteChannel = await _context.Set<SiteChannels>().FirstOrDefaultAsync(x => x.Id == modelDto.ChannelId)
                ?? throw new ResponseException($"频道[{modelDto.ChannelId}]不存在或已删除");

            //检查是否可以投稿
            if (siteChannel.IsContribute == 0)
            {
                throw new ResponseException($"该频道不允许投稿");
            }
            modelDto.SiteId = siteChannel.SiteId;
            //检查站点是否存在
            if (await _context.Set<Sites>().FirstOrDefaultAsync(x => x.Id == siteChannel.SiteId) == null)
            {
                throw new ResponseException($"投稿的站点[{modelDto.SiteId}]不存在或已删除");
            }
            //查找记录
            var model = await _context.Set<ArticleContributes>().FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new ResponseException($"数据[{id}]不存在或已删除");
            //获取当前用户名
            modelDto.UpdateBy = _userService.GetUserName();
            modelDto.UpdateTime = DateTime.Now;
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelDto, model);
            //通过
            if (modelDto.Status == 1)
            {
                //审核通过添加文章到对应的频道
                //类别关系
                List<ArticleCategoryRelationsDto> categoryRelations = [];
                for (int i = 0; i < modelDto.Categorys?.Length; i++)
                {
                    categoryRelations.Add(new ArticleCategoryRelationsDto()
                    {
                        CategoryId = Convert.ToInt64(modelDto.Categorys[i]),
                        ArticleId = 0
                    });
                }
                ArticlesAddDto articlesDto = new()
                {
                    Title = model.Title,
                    Status = 0,
                    Source = model.Source,
                    Author = model.Author,
                    ChannelId = model.ChannelId,
                    SiteId = model.SiteId,
                    Content = model.Content,
                    Zhaiyao = HtmlHelper.CutString(model.Content, 250),
                    AddBy = model.UserName,
                    AddTime = model.AddTime,
                    ImgUrl = model.ImgUrl,
                    //所属分类
                    CategoryRelations = categoryRelations
                };
                //扩展字段
                if (modelDto.Fields != null)
                {
                    List<ArticleFieldValuesDto> fieldValueAdds = [];
                    foreach (var field in modelDto.Fields)
                    {
                        fieldValueAdds.Add(new()
                        {
                            FieldId = field.FieldId,
                            FieldName = field.FieldName,
                            FieldValue = field.FieldValue
                        });
                    }
                    articlesDto.ArticleFields = fieldValueAdds;
                }
                //相册
                if (siteChannel.IsAlbum == 1)
                {
                    var albums = JsonHelper.ToJson<List<ArticleAlbumsDto>>(model.AlbumMeta);
                    if(albums != null)
                    {
                        articlesDto.ArticleAlbums = albums;
                    }
                }
                //附件
                if (siteChannel.IsAttach == 1)
                {
                    var attachs = JsonHelper.ToJson<List<ArticleAttachsDto>>(model.AttachMeta);
                    if(attachs != null)
                    {
                        articlesDto.ArticleAttachs = attachs;
                    }
                }
                Articles articlesModel = new();
                _mapper.Map(articlesDto, articlesModel);
                articlesModel.AddTime = model.AddTime;
                await _context.Set<Articles>().AddAsync(articlesModel);
            }
            //提交保存
            return (await _context.SaveChangesAsync() >= 0);
        }

        /// <summary>
        /// 修改一条记录
        /// </summary>
        public async Task<bool> UserUpdateAsync(int id, ArticleContributesEditDto modelDto)
        {
            _context = _contextFactory.CreateContext(WriteRoRead.Write);//连接数据库

            var siteChannel = await _context.Set<SiteChannels>().FirstOrDefaultAsync(x => x.Id == modelDto.ChannelId);
            //检查频道是否存在
            if (siteChannel == null)
            {
                throw new ResponseException($"频道{modelDto.ChannelId}不存在或已删除");
            }
            //检查是否可以投稿
            if (siteChannel.IsContribute == 0)
            {
                throw new ResponseException($"该频道不允许投稿");
            }
            modelDto.SiteId = siteChannel.SiteId;
            //检查站点是否存在
            if (await _context.Set<Sites>().FirstOrDefaultAsync(x => x.Id == siteChannel.SiteId) == null)
            {
                throw new ResponseException($"站点{modelDto.SiteId}不存在或已删除");
            }
            //查找记录
            //注意：要使用写的数据库进行查询，才能正确写入数据主库
            var model = await _context.Set<ArticleContributes>().FirstOrDefaultAsync(x => x.Id == id);
            if (model == null)
            {
                throw new ResponseException($"数据{id}不存在或已删除");
            }
            var user = await _userService.GetUserAsync();
            if(user == null)
            {
                throw new ResponseException($"用户尚未登录或已超时");
            }
            if (model.UserName != user.UserName)
            {
                throw new ResponseException($"请勿修改别人的数据");
            }
            //获取当前用户名
            modelDto.UpdateBy = user.UserName;
            modelDto.UpdateTime = DateTime.Now;
            //更新操作AutoMapper替我们完成，只需要调用保存即可
            _mapper.Map(modelDto, model);
            //通过
            if (modelDto.Status == 1)
            {
                //审核通过添加文章到对应的频道
                //类别关系
                List<ArticleCategoryRelationsDto> categoryRelations = new();
                for (int i = 0; i < modelDto.Categorys?.Length; i++)
                {
                    categoryRelations.Add(new ArticleCategoryRelationsDto()
                    {
                        CategoryId = Convert.ToInt64(modelDto.Categorys[i]),
                        ArticleId = 0
                    });
                }
                ArticlesAddDto articlesDto = new();
                articlesDto.Title = model.Title;
                articlesDto.Status = 0;
                articlesDto.Source = model.Source;
                articlesDto.Author = model.Author;
                articlesDto.ChannelId = model.ChannelId;
                articlesDto.SiteId = model.SiteId;
                articlesDto.Content = model.Content;
                articlesDto.Zhaiyao = HtmlHelper.CutString(model.Content, 250);
                articlesDto.AddBy = model.UserName;
                articlesDto.AddTime = model.AddTime;
                articlesDto.ImgUrl = model.ImgUrl;
                //所属分类
                articlesDto.CategoryRelations = categoryRelations;
                //扩展字段
                if (modelDto.Fields != null)
                {
                    List<ArticleFieldValuesDto> fieldValueAdds = [];
                    foreach (var field in modelDto.Fields)
                    {
                        fieldValueAdds.Add(new()
                        {
                            FieldId = field.FieldId,
                            FieldName = field.FieldName,
                            FieldValue = field.FieldValue
                        });
                    }
                    articlesDto.ArticleFields = fieldValueAdds;
                }
                Articles articlesModel = new();
                _mapper.Map(articlesDto, articlesModel);
                await _context.Set<Articles>().AddAsync(articlesModel);
            }
            //提交保存
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
