using AutoMapper;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;

namespace DTcms.Core.Model.Profiles
{
    public class FeedbackProfile : Profile
    {
        /// <summary>
        /// 留言反馈实体映射
        /// </summary>
        public FeedbackProfile()
        {
            //将源数据映射到DTO
            CreateMap<Feedbacks, FeedbacksDto>();
            CreateMap<Feedbacks, FeedbacksEditDto>();
            //将DTO映射到源数据
            CreateMap<FeedbacksEditDto, Feedbacks>();
            CreateMap<FeedbacksClientDto, Feedbacks>();
        }
    }
}
