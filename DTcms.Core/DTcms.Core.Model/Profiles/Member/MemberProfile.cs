using AutoMapper;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;

namespace DTcms.Core.Model.Profiles
{
    /// <summary>
    /// 会员实体映射
    /// </summary>
    public class MemberProfile : Profile
    {
        public MemberProfile()
        {
            //会员信息，将源数据映射到DTO
            CreateMap<Members, MembersDto>()
                .ForMember(
                    dest => dest.GroupTitle,
                    opt =>
                    {
                        opt.MapFrom(src => src.Group != null ? src.Group.Title : null);
                    }
                ).ForMember(
                    dest => dest.UserName,
                    opt =>
                    {
                        opt.MapFrom(src => src.User != null ? src.User.UserName : null);
                    }
                ).ForMember(
                    dest => dest.Email,
                    opt =>
                    {
                        opt.MapFrom(src => src.User != null ? src.User.Email : null);
                    }
                ).ForMember(
                    dest => dest.Phone,
                    opt =>
                    {
                        opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : null);
                    }
                ).ForMember(
                    dest => dest.Status,
                    opt =>
                    {
                        opt.MapFrom(src => src.User != null ? src.User.Status : 0);
                    }
                ).ForMember(
                    dest => dest.LastIp,
                    opt =>
                    {
                        opt.MapFrom(src => src.User != null ? src.User.LastIp : null);
                    }
                ).ForMember(
                    dest => dest.LastTime,
                    opt =>
                    {
                        opt.MapFrom(src => src.User != null ? src.User.LastTime : null);
                    }
                );
            CreateMap<Members, MembersClientDto>()
                .ForMember(
                    dest => dest.UserName,
                    opt =>
                    {
                        opt.MapFrom(src => src.User != null ? src.User.UserName : null);
                    }
                );
            CreateMap<Members, MembersEditDto>();
            //会员信息，将源数据映射到DTO
            CreateMap<MembersEditDto, Members>();
            CreateMap<MembersModifyDto, Members>();
            CreateMap<RegisterDto, MembersEditDto>();

            //会员组，将源数据映射到DTO
            CreateMap<MemberGroups, MemberGroupsDto>();
            CreateMap<MemberGroups, MemberGroupsEditDto>();
            //会员组，将DTO映射到源数据
            CreateMap<MemberGroupsEditDto, MemberGroups>();

            //会员充值，将源数据映射到DTO
            CreateMap<MemberRecharges, MemberRechargesDto>()
                .ForMember(
                    dest => dest.TradeNo,
                    opt =>
                    {
                        opt.MapFrom(src => src.OrderPayments != null ? src.OrderPayments.First().TradeNo : null);
                    }
                ).ForMember(
                    dest => dest.PaymentId,
                    opt =>
                    {
                        opt.MapFrom(src => src.OrderPayments != null ? (int?)src.OrderPayments.First().PaymentId : null);
                    }
                ).ForMember(
                    dest => dest.PaymentTitle,
                    opt =>
                    {
                        opt.MapFrom(src => src.OrderPayments != null ? src.OrderPayments.First().PaymentTitle : null);
                    }
                );
            CreateMap<MemberRecharges, MemberRechargesEditDto>();
            //会员充值，将DTO映射到源数据
            CreateMap<MemberRechargesEditDto, MemberRecharges>();

            

            //余额记录，将源数据映射到DTO
            CreateMap<MemberBalanceRecords, MemberBalanceRecordsDto>();
            CreateMap<MemberBalanceRecords, MemberBalanceRecordsEditDto>();
            //余额记录，将DTO映射到源数据
            CreateMap<MemberBalanceRecordsEditDto, MemberBalanceRecords>();

            //积分记录，将源数据映射到DTO
            CreateMap<MemberPointRecords, MemberPointRecordsDto>();
            CreateMap<MemberPointRecords, MemberPointRecordsEditDto>();
            //积分记录，将DTO映射到源数据
            CreateMap<MemberPointRecordsEditDto, MemberPointRecords>();

            //附件下载记录，将源数据映射到DTO
            CreateMap<MemberAttachRecords, MemberAttachRecordsDto>()
                .ForMember(
                    dest => dest.UserName,
                    opt =>
                    {
                        opt.MapFrom(src => src.User != null ? src.User.UserName : null);
                    }
                );
            CreateMap<MemberAttachRecords, MemberAttachRecordsEditDto>();
            //附件下载记录，将DTO映射到源数据
            CreateMap<MemberAttachRecordsEditDto, MemberAttachRecords>();

            //站内消息，将源数据映射到DTO
            CreateMap<MemberMessages, MemberMessagesDto>()
                .ForMember(
                    dest => dest.UserName,
                    opt =>
                    {
                        opt.MapFrom(src => src.User != null ? src.User.UserName : null);
                    }
                );
            CreateMap<MemberMessages, MemberMessagesEditDto>();
            //站内消息，将DTO映射到源数据
            CreateMap<MemberMessagesEditDto, MemberMessages>();
        }
    }
}
