using System;
using AutoMapper;
using EClinic.Models.ViewModels;
using EClinic.Models.Domain;

namespace EClinic
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SignInViewModel, User>();
            CreateMap<CreateDoctorViewModel, User>();
            CreateMap<CreateDoctorViewModel, Doctor>();
            CreateMap<User, UserIndexViewModel>();
            CreateMap<Meetting, MeetingForTableViewModel>()
                .ForMember(dest => dest.PatientName, src => src.MapFrom(e => e.Patient.User.FullName));
            CreateMap<Meetting, MeetingForListViewModel>()
                .ForMember(dest => dest.DoctorName, src => src.MapFrom(e => e.Doctor.User.FullName))
                .ForMember(dest => dest.Cabinet, src => src.MapFrom(e => e.Doctor.Cabinet))
                .ForMember(dest => dest.DoctorType, src => src.MapFrom(e => e.Doctor.DoctorType.Name));
            CreateMap<CreateMedicalCardViewModel, MedicalCard>();
            CreateMap<MedicalCard, CreateMedicalCardViewModel>();
            CreateMap<Meetting, MettingForManagingListViewModel>()
                .ForMember(dest => dest.DoctorTypeName, src => src.MapFrom(e => e.Doctor.DoctorType.Name))
                .ForMember(dest => dest.DoctorName, src => src.MapFrom(e => e.Doctor.User.FullName))
                .ForMember(dest => dest.DoctorTypeId, src => src.MapFrom(e => e.Doctor.DoctorType.Id))
                .ForMember(dest => dest.PatientName, src => src.MapFrom(e => e.Patient.User.FullName))
                .ForMember(dest => dest.MeettingStatusName, src => src.MapFrom(e => e.MeettingStatus.Name));
            CreateMap<User, ShowUserViewModel>();

        }
    }
}
