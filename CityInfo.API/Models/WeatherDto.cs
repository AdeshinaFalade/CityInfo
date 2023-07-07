namespace CityInfo.API.Models
{
    public class WeatherDto
    {
        public string Country { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public double Temperature { get; set; }

        public WeatherDto(string country, string description, double temperature, string icon)
        {
            Country = country;
            Description = description;
            Temperature = temperature;
            Icon = icon;
        }
    }
}
