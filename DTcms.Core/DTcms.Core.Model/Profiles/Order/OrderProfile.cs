using AutoMapper;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;

namespace DTcms.Core.Model.Profiles
{
    /// <summary>
    /// 订单实体映射
    /// </summary>
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            //支付收款，将源数据映射到DTO
            CreateMap<OrderPayments, OrderPaymentsDto>();
            CreateMap<OrderPayments, OrderPaymentsListDto>();
            //支付收款，将DTO映射到源数据
            CreateMap<OrderPaymentsAddDto, OrderPayments>();
            CreateMap<OrderPaymentsEditDto, OrderPayments>();
        }
    }
}
