using AutoMapper;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;

namespace DTcms.Core.Model.Profiles
{
    public class AdvertProfile : Profile
    {
        /// <summary>
        /// 广告实体映射
        /// </summary>
        public AdvertProfile()
        {
            //将源数据映射到DTO
            CreateMap<Adverts, AdvertsDto>();
            CreateMap<Adverts, AdvertsEditDto>();
            CreateMap<AdvertBanners, AdvertBannersDto>()
                .ForMember(
                    dest => dest.AdvertTitle,
                    opt =>
                    {
                        opt.MapFrom(src => src.Advert != null ? src.Advert.Title : null);
                    }
                );
            CreateMap<AdvertBanners, AdvertBannersEditDto>();
            //将DTO映射到源数据
            CreateMap<AdvertsEditDto, Adverts>();
            CreateMap<AdvertBannersEditDto, AdvertBanners>();
        }
    }
}
