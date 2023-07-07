using CityInfo.API.Entities;
using CityInfo.API.Models;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
        Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);
        Task<User?> GetUserAsync(string username);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
        Task<bool> CityExistsAsync(int cityId);
        Task<bool> UserExistsAsync(string username);
        Task<bool> SaveChangesAsync();
        Task<bool> CityNameMatchesCityId(string? cityName, int cityId);
        Task AddPointOfInterestForCityAsync(int cityid, PointOfInterest pointOfInterest);
        void RegisterUserAsync(User user);
        void DeletePointOfInterestForCity(PointOfInterest pointOfInterest);
        Task<WeatherDto> GetWeatherDataForCityAsync(string city);
    }
}
