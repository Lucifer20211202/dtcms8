using AutoMapper;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;

namespace DTcms.Core.Model.Profiles
{
    /// <summary>
    /// 通知模板实体映射
    /// </summary>
    public class NotifyTemplateProfile : Profile
    {
        public NotifyTemplateProfile()
        {
            //将源数据映射到DTO
            CreateMap<NotifyTemplates, NotifyTemplatesDto>();
            //将DTO映射到源数据
            CreateMap<NotifyTemplatesEditDto, NotifyTemplates>();
        }
    }
}
