using AutoMapper;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;
using DTcms.Core.Common.Helpers;
using DTcms.Core.Common.Extensions;

namespace DTcms.Core.Model.Profiles
{
    /// <summary>
    /// 文章实体映射
    /// </summary>
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            //文章，将源数据映射到DTO
            CreateMap<Articles, ArticlesListDto>()
                .ForMember(
                    dest => dest.CategoryTitle,
                    opt =>
                    {
                        opt.MapFrom(src => string.Join(",", src.CategoryRelations.Select(x => x.Category!.Title).ToArray()));
                    }
                ).ForMember(
                    dest => dest.LabelTitle,
                    opt =>
                    {
                        opt.MapFrom(src => string.Join(",", src.LabelRelations.Select(x => x.Label!.Title).ToArray()));
                    }
                ).ForMember(
                    dest => dest.Fields,
                    opt =>
                    {
                        opt.MapFrom(src => GetFields(src.ArticleFields));
                    }
                ).ForMember(
                    dest => dest.IsComment,
                    opt =>
                    {
                        opt.MapFrom(src => (src.SiteChannel != null && src.SiteChannel.IsComment == 1) ? src.IsComment : 0);
                    }
                );
            CreateMap<Articles, ArticlesClientListDto>()
                .ForMember(
                    dest => dest.CategoryTitle,
                    opt =>
                    {
                        opt.MapFrom(src => string.Join(",", src.CategoryRelations.Select(x => x.Category!.Title).ToArray()));
                    }
                ).ForMember(
                    dest => dest.LabelTitle,
                    opt =>
                    {
                        opt.MapFrom(src => string.Join(",", src.LabelRelations.Select(x => x.Label!.Title).ToArray()));
                    }
                ).ForMember(
                    dest => dest.Fields,
                    opt =>
                    {
                        opt.MapFrom(src => GetFields(src.ArticleFields));
                    }
                ).ForMember(
                    dest => dest.IsComment,
                    opt =>
                    {
                        opt.MapFrom(src => (src.SiteChannel != null && src.SiteChannel.IsComment == 1) ? src.IsComment : 0);
                    }
                );
            CreateMap<Articles, ArticlesDto>();
            CreateMap<Articles, ArticlesClientDto>();
            CreateMap<Articles, ArticlesEditDto>();
            //文章，将DTO映射到源数据
            CreateMap<ArticlesAddDto, Articles>();
            CreateMap<ArticlesEditDto, Articles>();

            //扩展字段，源数据映射到DTO
            CreateMap<ArticleFieldValues, ArticleFieldValuesDto>();
            //扩展字段，DTO映射到源数据
            CreateMap<ArticleFieldValuesDto, ArticleFieldValues>();

            //文章会员组,将源数据映射到DTO
            CreateMap<ArticleGroups, ArticleGroupsDto>();
            //文章会员组，将DTO映射到源数据
            CreateMap<ArticleGroupsDto, ArticleGroups>();

            //文章相册,将源数据映射到DTO
            CreateMap<ArticleAlbums, ArticleAlbumsDto>();
            //文章相册，将DTO映射到源数据
            CreateMap<ArticleAlbumsDto, ArticleAlbums>();

            //文章附件,将源数据映射到DTO
            CreateMap<ArticleAttachs, ArticleAttachsDto>()
                .ForMember(
                    dest => dest.Size,
                    opt =>
                    {
                        opt.MapFrom(src => FileHelper.ByteConvertStorage(src.FileSize));
                    }
                );
            CreateMap<ArticleAttachs, ArticleAttachsClientDto>()
                .ForMember(
                    dest => dest.Size,
                    opt =>
                    {
                        opt.MapFrom(src => FileHelper.ByteConvertStorage(src.FileSize));
                    }
                );
            //文章附件，将DTO映射到源数据
            CreateMap<ArticleAttachsDto, ArticleAttachs>();

            //文章评论，源数据映射到DTO
            CreateMap<ArticleComments, ArticleCommentsDto>()
                .ForMember(
                    dest => dest.UserAvatar,
                    opt =>
                    {
                        opt.MapFrom(src => src.User != null && src.User.Member != null ? src.User.Member.Avatar : null);
                    }
                )
                .ForMember(
                    dest => dest.Content,
                    opt =>
                    {
                        opt.MapFrom(src => (src.IsDelete == 1 ? "原内容已删除" : src.Content));
                    }
                )
                .ForMember(
                    dest => dest.DateDescription,
                    opt =>
                    {
                        opt.MapFrom(src => src.AddTime.ToTimeDifferNow());
                    }
                );
            CreateMap<ArticleComments, ArticleCommentsEditDto>();
            //文章评论，DTO映射到源数据
            CreateMap<ArticleCommentsDto, ArticleComments>();
            CreateMap<ArticleCommentsEditDto, ArticleComments>();

            //文章分类，将源数据映射到DTO
            CreateMap<ArticleCategorys, ArticleCategorysDto>();
            CreateMap<ArticleCategorys, ArticleCategorysEditDto>();
            CreateMap<ArticleCategoryRelations, ArticleCategoryRelationsDto>();
            //文章分类，将DTO映射到源数据
            CreateMap<ArticleCategorysEditDto, ArticleCategorys>();
            CreateMap<ArticleCategoryRelationsDto, ArticleCategoryRelations>();

            //文章标签，源数据映射到DTO
            CreateMap<ArticleLabels, ArticleLabelsDto>();
            CreateMap<ArticleLabels, ArticleLabelsEditDto>();
            CreateMap<ArticleLabelRelations, ArticleLabelRelationsDto>();
            //文章标签，DTO映射到源数据
            CreateMap<ArticleLabelsEditDto, ArticleLabels>();
            CreateMap<ArticleLabelRelationsDto, ArticleLabelRelations>();

            //文章点赞，源数据映射到DTO
            CreateMap<ArticleLikes, ArticleLikesDto>();
            //文章点赞，DTO映射到源数据
            CreateMap<ArticleLikesDto, ArticleLikes>();

            //文章投稿，源数据映射到DTO
            CreateMap<ArticleContributes, ArticleContributesDto>();
            CreateMap<ArticleContributes, ArticleContributesViewDto>()
                .ForMember(dest => dest.Fields, opt => opt.MapFrom(src => JsonHelper.ToObject<List<ArticleContributeFieldsDto>>(src.FieldMeta)))
                .ForMember(dest => dest.Albums, opt => opt.MapFrom(src => JsonHelper.ToObject<List<ArticleAlbumsDto>>(src.AlbumMeta)))
                .ForMember(dest => dest.Attachs, opt => opt.MapFrom(src => JsonHelper.ToObject<List<ArticleAttachsDto>>(src.AttachMeta)));
            //文章投稿，DTO映射到源数据
            CreateMap<ArticleContributesAddDto, ArticleContributes>()
                .ForMember(desc => desc.FieldMeta, opt => opt.MapFrom(src => src.Fields != null ? src.Fields.ToJson() : null))
                .ForMember(desc => desc.AlbumMeta, opt => opt.MapFrom(src => src.Albums != null ? src.Albums!.ToJson() : null))
                .ForMember(desc => desc.AttachMeta, opt => opt.MapFrom(src => src.Attachs != null ? src.Attachs!.ToJson() : null));
            CreateMap<ArticleContributesEditDto, ArticleContributes>()
                .ForMember(desc => desc.FieldMeta, opt => opt.MapFrom(src => src.Fields != null ? src.Fields!.ToJson() : null))
                .ForMember(desc => desc.AlbumMeta, opt => opt.MapFrom(src => src.Albums != null ? src.Albums!.ToJson() : null))
                .ForMember(desc => desc.AttachMeta, opt => opt.MapFrom(src => src.Attachs != null ? src.Attachs!.ToJson() : null));
        }

        /// <summary>
        /// 获得扩展字段键值对
        /// </summary>
        /// <param name="articleFields">扩展字段列表</param>
        public static Dictionary<string, string?> GetFields(ICollection<ArticleFieldValues> articleFields)
        {
            Dictionary<string, string?> dic = [];
            foreach (var item in articleFields)
            {
                if (item.FieldName != null)
                {
                    dic.Add(item.FieldName, item.FieldValue);
                }
            }
            return dic;
        }
    }
}
