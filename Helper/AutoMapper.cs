using AutoMapper;
using LearnerDuo.Dto;
using LearnerDuo.Extentions;
using LearnerDuo.Models;
using LearnerDuo.ModelViews;

namespace LearnerDuo.Helper
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<User, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Birthday.Value.CaculateAge()))
                .ReverseMap();
            CreateMap<Photo, PhotoDto>().ReverseMap();
            CreateMap<User, Register>().ReverseMap();
        }
    }
}
