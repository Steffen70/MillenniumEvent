using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<RegisterDto, AppUser>();

            CreateMap<AppUser, UserDto>();
            CreateMap<AppUser, UserAdminDto>();
            CreateMap<AppUser, UserListDto>();

            CreateMap<BikeCreateDto, Bike>();
            CreateMap<Bike, BikeDto>();
        }
    }
}