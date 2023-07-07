using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;
        private readonly WeatherApiClient _weatherApiClient;

        public CityInfoRepository(CityInfoContext context, WeatherApiClient weatherApiClient)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _weatherApiClient = weatherApiClient ?? throw new ArgumentNullException(nameof(weatherApiClient));
        }
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return await _context.Cities.Include(c => c.PointsOfInterest)
                .Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }

            return await _context.Cities
                .FirstOrDefaultAsync(c => c.Id == cityId);
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointsOfInterest
                .FirstOrDefaultAsync(p => p.CityId == cityId && p.Id == pointOfInterestId);
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _context.PointsOfInterest
                .Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task AddPointOfInterestForCityAsync(int cityid, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityid, false);
            if (city != null)
            {
                city.PointsOfInterest.Add(pointOfInterest);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public void DeletePointOfInterestForCity(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }

        public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            //collection to start from
            var collection = _context.Cities as IQueryable<City>;
            if(!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name.ToLower() == name.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim().ToLower();
                collection = collection.Where(c => c.Name.ToLower().Contains(searchQuery)
                || (c.Description != null && c.Description.ToLower().Contains(searchQuery)));
            }

            var totalItemCount = await collection.CountAsync();

            var paginationMetaData = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

            var collectionToReturn = await collection
                .OrderBy(c => c.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetaData);
        }

        public async Task<bool> CityNameMatchesCityId(string? cityName, int cityId)
        {
            if (cityName == null)
            {
                return false;
            }

            return await _context.Cities.AnyAsync(c => c.Id == cityId && c.Name.ToLower() == cityName.ToLower());
        }

        public void RegisterUserAsync(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.Users.Add(user);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(c => c.Username == username);
        }

        public async Task<User?> GetUserAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(c => c.Username == username);
        }

        public async Task<WeatherDto> GetWeatherDataForCityAsync(string city)
        {
            HttpResponseMessage result = await _weatherApiClient.GetWeatherDataAsync(city);
            if (result.IsSuccessStatusCode)
            {
                var imgageHost = "https://openweathermap.org/img/wn/";
                var mResult = await result.Content.ReadAsStringAsync();
                var resultObject = JObject.Parse(mResult);
                string weatherDescription = resultObject["weather"][0]["description"].ToString();
                string icon = imgageHost + resultObject["weather"][0]["icon"].ToString() + ".png";
                string temperature = resultObject["main"]["temp"].ToString();
                string placeName = resultObject["name"].ToString();
                string country = resultObject["sys"]["country"].ToString();
                weatherDescription = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(weatherDescription);

                return new WeatherDto(country, weatherDescription, Convert.ToDouble(temperature), icon);
            }

            return null!;
        }

    }
}
