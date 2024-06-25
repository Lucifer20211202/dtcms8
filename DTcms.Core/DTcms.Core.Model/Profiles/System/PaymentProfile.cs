using AutoMapper;
using DTcms.Core.Common.Helpers;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;

namespace DTcms.Core.Model.Profiles
{
    /// <summary>
    /// 支付方式实体映射
    /// </summary>
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            //支付方式，将源数据映射到DTO
            CreateMap<Payments, PaymentsDto>();
            CreateMap<Payments, PaymentsEditDto>();
            //支付方式，将DTO映射到源数据
            CreateMap<PaymentsEditDto, Payments>();

            //站点支付方式，将源数据映射到DTO
            CreateMap<SitePayments, SitePaymentsDto>();
            CreateMap<SitePayments, SitePaymentsEditDto>();
            CreateMap<SitePayments, SitePaymentsClientDto>()
                .ForMember(
                    dest => dest.PaymentType,
                    opt => opt.MapFrom(src => src.Payment == null ? 0 : src.Payment.Type))
                .ForMember(
                    dest => dest.ImgUrl,
                    opt => opt.MapFrom(src => src.Payment == null ? null : src.Payment.ImgUrl))
                .ForMember(
                    dest => dest.Remark,
                    opt => opt.MapFrom(src => src.Payment == null ? null : src.Payment.Remark))
                .ForMember(
                    dest => dest.PayUrl,
                    opt => opt.MapFrom(src => src.Payment == null ? null : src.Payment.PayUrl)
                );
            //站点支付方式，将DTO映射到源数据
            CreateMap<SitePaymentsEditDto, SitePayments>();
        }
    }
}
