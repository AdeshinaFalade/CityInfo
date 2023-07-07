using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CityInfo.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ICityInfoRepository cityInfoRepository;
        private readonly IMapper mapper;
        private readonly ILogger<UserController> logger;

        //we won't use this outside of this class, so we can scope it to this namespace
        public class AuthenticationRequestBody
        {
            public string? Username { get; set; }
            public string? Password { get; set; }
        }

        private class CityInfoUser
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(
                int userId,
                string username,
                string firstName,
                string lastName,
                string city)
            {
                UserId = userId;
                Username = username;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }

        }

        public UserController(
            IConfiguration configuration,
            ICityInfoRepository cityInfoRepository,
            IMapper mapper,
            ILogger<UserController> logger)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        /// <summary>
        /// Login 
        /// </summary>
        /// <param name="authenticationRequestBody">Authentication Request Body</param>
        /// <returns>User details and token</returns>
        [HttpPost("authenticate")]
        public async Task<ActionResult<LoginResponseDto>> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            //Step 1: validate login details
            var user = await cityInfoRepository.GetUserAsync(authenticationRequestBody.Username);
            //var user = ValidateUserCredentials(authenticationRequestBody.UserName, authenticationRequestBody.Password);
            if (user == null || !BCrypt.Net.BCrypt.Verify(authenticationRequestBody.Password, user.Password))
            {
                return Unauthorized("Username or password incorrect");
            }
            //step 2: create a token
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Authentication:SecretForKey"] ?? ""));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("given_name", user.FirstName));
            claimsForToken.Add(new Claim("family_name", user.LastName));
            claimsForToken.Add(new Claim("city", user.City));

            var jwtSecurityToken = new JwtSecurityToken(
                configuration["Authentication:Issuer"],
                configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            var fullName = user.FirstName + " " + user.LastName;
            var response = new LoginResponseDto(fullName, user.City, tokenToReturn);
            return Ok(response);

        }
        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="user">User Object</param>
        /// <returns>Message</returns>
        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(UserDto user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await cityInfoRepository.UserExistsAsync(user.Username))
                {
                    return BadRequest($"User with username {user.Username} already exists");
                }
                var finalUser = mapper.Map<User>(user);
                cityInfoRepository.RegisterUserAsync(finalUser);
                await cityInfoRepository.SaveChangesAsync();

                return Ok("User Created successfully");
            }
            catch (Exception e)
            {
                logger.LogInformation(e.Message);
                return StatusCode(500, new { Message = "An error occurred while registering the user" });
            }

        }

        /*private CityInfoUser ValidateUserCredentials(string? userName, string? password)
        {
            // we don't have a user DB or table.  If you have, check the passed-through
            // username/password against what's stored in the database.
            //
            // For demo purposes, we assume the credentials are valid

            // return a new CityInfoUser (values would normally come from your user DB/table)
            if (userName != null && password != null)
            {
                return new CityInfoUser(
                1,
                userName ?? "",
                "Adeshina",
                "Falade",
                "Antwerp"
                );
            }

            return null!;
        }*/
    }
}
