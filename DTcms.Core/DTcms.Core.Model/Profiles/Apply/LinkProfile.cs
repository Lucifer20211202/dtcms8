using AutoMapper;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;

namespace DTcms.Core.Model.Profiles
{
    public class LinkProfile : Profile
    {
        /// <summary>
        /// 友情链接实体映射
        /// </summary>
        public LinkProfile()
        {
            //将源数据映射到DTO
            CreateMap<Links, LinksDto>();
            CreateMap<Links, LinksEditDto>();
            //将DTO映射到源数据
            CreateMap<LinksEditDto, Links>();
            CreateMap<LinksClientDto, Links>();
        }
    }
}
