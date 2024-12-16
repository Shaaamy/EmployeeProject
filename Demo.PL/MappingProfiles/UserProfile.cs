using AutoMapper;
using Demo.DAL.Models;
using Demo.PL.ViewModels;

namespace Demo.PL.MappingProfiles
{
    public class UserProfile :Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser,UserViewModel>()
                .ForMember(dest => dest.Fname, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.Lname, opt => opt.MapFrom(src => src.LastName)).ReverseMap();
        }
    }
}
