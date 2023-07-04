namespace CityInfo.API.Models
{
    public class LoginResponseDto
    {
        public string FullName { get; set; }
        public string City { get; set; }
        public string Token { get; set; }
        public LoginResponseDto(string fullname, string city, string token)
        {
            FullName = fullname;

            City = city;

            Token = token;

        }
    }
}
