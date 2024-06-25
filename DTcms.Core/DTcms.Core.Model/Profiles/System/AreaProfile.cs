using AutoMapper;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;

namespace DTcms.Core.Model.Profiles
{
    /// <summary>
    /// 省市区实体映射
    /// </summary>
    public class AreaProfile : Profile
    {
        public AreaProfile()
        {
            //将源数据映射到DTO
            CreateMap<Areas, AreasDto>();
            CreateMap<Areas, AreasEditDto>();
            //将DTO映射到源数据
            CreateMap<AreasEditDto, Areas>();
        }
    }
}
