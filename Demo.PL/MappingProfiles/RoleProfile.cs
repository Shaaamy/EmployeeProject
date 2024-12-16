using AutoMapper;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Demo.PL.MappingProfiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<IdentityRole, RoleViewModel>()
                 .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Name))
                 .ReverseMap();
        }
    }
}
