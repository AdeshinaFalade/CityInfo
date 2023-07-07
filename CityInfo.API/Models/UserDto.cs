using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    public class UserDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(50)]
        public string Password { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string City { get; set; }
    }
}
