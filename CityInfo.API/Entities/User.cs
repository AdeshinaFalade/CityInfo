using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CityInfo.API.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [MaxLength(20)]
        public string UserName { get; set; }
        
        [Required]
        [JsonIgnore]
        public string Password { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        public User(
            string userName,
            string password,
            string firstName,
            string lastName,
            string city)
        {
            UserName = userName;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            City = city;
        }

    }
}
