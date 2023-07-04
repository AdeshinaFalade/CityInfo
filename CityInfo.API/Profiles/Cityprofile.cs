using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class Cityprofile : Profile
    {
        public Cityprofile()
        {
            CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>();
            CreateMap<Entities.City, Models.CityDto>();
        }
    }
}
