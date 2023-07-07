namespace CityInfo.API.Services
{
    public class WeatherApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherApiClient(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<HttpResponseMessage> GetWeatherDataAsync(string city)
        {
            string apiUrl = "https://api.openweathermap.org/data/2.5";
            string endpointUrl = $"{apiUrl}/weather?q={city}&appid={_apiKey}&units=metric";

            return await _httpClient.GetAsync(endpointUrl);
        }
    }
}
