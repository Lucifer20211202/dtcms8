using AutoMapper;
using DTcms.Core.Model.Models;
using DTcms.Core.Model.ViewModels;

namespace DTcms.Core.Model.Profiles
{
    /// <summary>
    /// 管理员实体映射
    /// </summary>
    public class ManagerProfile : Profile
    {
        public ManagerProfile()
        {
            //管理员,将源数据映射到DTO
            CreateMap<Managers, ManagersDto>()
                .ForMember(
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
                    dest => dest.RoleId,
                    opt =>
                    {
                        opt.MapFrom(src => src.User != null ? src.User!.UserRoles!.FirstOrDefault()!.RoleId : 0);
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
            CreateMap<Managers, ManagersEditDto>();
            //管理员,将DTO映射到源数据
            CreateMap<ManagersDto, Managers>();
            CreateMap<ManagersEditDto, Managers>();

            //管理角色,将源数据映射到DTO
            CreateMap<ApplicationRole, ManagerRolesDto>();
            CreateMap<ApplicationRole, ManagerRolesEditDto>();
            //管理角色,将DTO映射到源数据
            CreateMap<ManagerRolesDto, ApplicationRole>();
            CreateMap<ManagerRolesEditDto, ApplicationRole>();

            //管理日志,将源数据映射到DTO
            CreateMap<ManagerLogs, ManagerLogsDto>();
            //管理日志,将DTO映射到源数据
            CreateMap<ManagerLogsDto, ManagerLogs>();

            //管理菜单,将源数据映射到DTO
            CreateMap<ManagerMenus, ManagerMenusDto>();
            CreateMap<ManagerMenus, ManagerMenusEditDto>();
            //管理菜单,将DTO映射到源数据
            CreateMap<ManagerMenusEditDto, ManagerMenus>();
        }
    }
}
