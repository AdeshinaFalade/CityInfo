using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Models.UserDto, Entities.User>();
            CreateMap<Entities.User, Models.UserDto>();
        }
    }
}
