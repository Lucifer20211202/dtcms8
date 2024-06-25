using AutoMapper;
using DTcms.Core.Common.Helpers;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;

namespace DTcms.Core.Model.Profiles
{
    /// <summary>
    /// 站点实体映射
    /// </summary>
    public class SiteProfile : Profile
    {
        public SiteProfile()
        {
            //站点信息,将源数据映射到DTO
            CreateMap<Sites, SitesDto>();
            CreateMap<Sites, SitesEditDto>();
            CreateMap<SiteDomains, SiteDomainsDto>();
            //站点信息,将DTO映射到源数据
            CreateMap<SitesEditDto, Sites>();
            CreateMap<SiteDomainsDto, SiteDomains>();

            //站点菜单,将源数据映射到DTO
            CreateMap<SiteMenus, SiteMenusDto>();
            CreateMap<SiteMenus, SiteMenusEditDto>();
            //站点菜单,将DTO映射到源数据
            CreateMap<SiteMenusEditDto, SiteMenus>();

            //站点频道,将源数据映射到DTO
            CreateMap<SiteChannels, SiteChannelsDto>();
            CreateMap<SiteChannels, SiteChannelsEditDto>();
            CreateMap<SiteChannelFields, SiteChannelFieldsDto>()
                .ForMember(
                    dest => dest.Options,
                    opt => opt.MapFrom(src => UtilHelper.GetCheckboxOrRadioOptions(src.ControlType, src.ItemOption))
                ).ForMember(
                    dest => dest.FieldValue, opt => opt.MapFrom(src => UtilHelper.GetCheckboxDefaultValue(src.ControlType, src.DefaultValue))
                );
            //站点频道,将DTO映射到源数据
            CreateMap<SiteChannelsEditDto, SiteChannels>();
            CreateMap<SiteChannelFieldsDto, SiteChannelFields>();

            //授权登录,将源数据映射到DTO
            CreateMap<SiteOAuths, SiteOAuthsDto>();
            CreateMap<SiteOAuths, SiteOAuthsEditDto>();
            CreateMap<SiteOAuthLogins, SiteOAuthLoginsDto>()
                .ForMember(
                   dest => dest.UserName,
                   opt =>
                   {
                       opt.MapFrom(src => src.User != null ? src.User.UserName : null);
                   }
               ).ForMember(
                   dest => dest.OAuthTitle,
                   opt =>
                   {
                       opt.MapFrom(src => src.OAuth != null ? src.OAuth.Title : null);
                   }
               );
            //授权登录记录,将DTO映射到源数据
            CreateMap<SiteOAuthsEditDto, SiteOAuths>();
            CreateMap<SiteOAuthLoginsEditDto, SiteOAuthLogins>();
        }

    }
}
