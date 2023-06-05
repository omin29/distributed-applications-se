using ApplicationService.DTOs;
using AutoMapper;
using SchoolInfoMVC.Models;

namespace SchoolInfoMVC.Utility
{
    public class MapperConfig
    {
        public static Mapper InitializeAutomapper()
        {
            //Provide all the Mapping Configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TeacherDTO, TeacherVM>()
                .ForMember(dest => dest.BirthDate,
                    opt => opt.MapFrom(src => new DateOnly(src.BirthDate.Year, src.BirthDate.Month, src.BirthDate.Day)));

                cfg.CreateMap<TeacherVM, TeacherDTO>()
                .ForMember(dest => dest.BirthDate,
                    opt => opt.MapFrom(src => new DateTime(src.BirthDate.Year, src.BirthDate.Month, src.BirthDate.Day)));

                cfg.CreateMap<ClassDTO, ClassVM>();
                cfg.CreateMap<ClassVM, SaveClassDTO>();

                cfg.CreateMap<StudentDTO, StudentVM>()
                .ForMember(dest => dest.BirthDate,
                    opt => opt.MapFrom(src => new DateOnly(src.BirthDate.Year, src.BirthDate.Month, src.BirthDate.Day)));

                cfg.CreateMap<StudentVM, SaveStudentDTO>()
                .ForMember(dest => dest.BirthDate,
                    opt => opt.MapFrom(src => new DateTime(src.BirthDate.Year, src.BirthDate.Month, src.BirthDate.Day)));

                cfg.CreateMap<TeacherClassDTO, TeacherClassVM>();
                cfg.CreateMap<TeacherClassVM, SaveTeacherClassDTO>();

                cfg.CreateMap<UserDTO, UserVM>();
                cfg.CreateMap<UserVM, UserDTO>();
            });

            //Create an Instance of Mapper and return that Instance
            var mapper = new Mapper(config);
            return mapper;
        }
    }
}
