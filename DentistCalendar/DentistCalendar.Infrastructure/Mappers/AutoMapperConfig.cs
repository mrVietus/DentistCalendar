using AutoMapper;
using DentistCalendar.Core.Entities;
using DentistCalendar.Dto.DTO.Application;
using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Infrastructure.MappedModels.MainMenu;
using System.Linq;

namespace DentistCalendar.Infrastructure.Mappers
{
    public static class AutoMapperConfig
    {
        public static IMapper Initialize()
        {
            return new MapperConfiguration(cfg =>
                       {
                           cfg.CreateMap<Admin, AdminDto>().ReverseMap();
                           cfg.CreateMap<Appointment, AppointmentDto>().ReverseMap();
                           cfg.CreateMap<Dentist, DentistDto>().ReverseMap();
                           cfg.CreateMap<DentistOffice, DentistOfficeDto>()
                            .ForMember(d => d.Dentists, opt => opt.MapFrom(src => src.DentistDentistOffices.Select(x => x.Dentist).ToList()))
                            .ReverseMap();
                           cfg.CreateMap<Invitation, InvitationDto>()
                            .ForMember(d => d.InvitationGuid, opt => opt.MapFrom(src => src.Guid))
                            .ReverseMap();
                           cfg.CreateMap<InvitationDto, SendInvitationDto>().ReverseMap();
                           cfg.CreateMap<InvitationDto, InvitationDataDto>().ReverseMap();
                           cfg.CreateMap<MenuElement, MenuElementDto>()
                            .ForMember(d => d.AspAction, opt => opt.MapFrom(src => src.AspAction))
                            .ForMember(d => d.ControllerName, opt => opt.MapFrom(src => src.ControllerName))
                            .ForMember(d => d.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                            .ForMember(d => d.IsForAuthenticatedUser, opt => opt.MapFrom(src => src.IsForAuthenticatedUser))
                            .ForMember(d => d.ParentMenuId, opt => opt.MapFrom(src => src.ParentMenuElementId))
                            .ForMember(d => d.TypeOfPermittedAccount, opt => opt.MapFrom(src => src.TypeOfPermittedAccount))
                            .ForMember(d => d.Id, opt => opt.MapFrom(src => src.MenuElementId))
                            .ReverseMap();

                           cfg.CreateMap<Patient, PatientDto>().ReverseMap();
                           cfg.CreateMap<Receptionist, ReceptionistDto>().ReverseMap();
                           cfg.CreateMap<Service, ServiceDto>().ReverseMap();
                           cfg.CreateMap<User, UserDto>().ReverseMap();

                           cfg.CreateMap<MenuElementDto, MainMenuModel>().ReverseMap();

                       }).CreateMapper();
        }
    }
}