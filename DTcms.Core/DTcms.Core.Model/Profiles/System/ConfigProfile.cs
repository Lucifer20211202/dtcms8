using AutoMapper;
using DTcms.Core.Model.ViewModels;

namespace DTcms.Core.Model.Profiles
{
    /// <summary>
    /// 系统配置实体映射
    /// </summary>
    public class ConfigProfile : Profile
    {
        public ConfigProfile()
        {
            //系统参数，将DTO映射到上传DTO
            CreateMap<SysConfigDto, UploadConfigDto>();
        }
    }
}
