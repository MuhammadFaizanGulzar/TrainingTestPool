using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WEBAPI.Application.Interfaces;
using WEBAPI.Domain.Entities;
using WEBAPI.Domain.Helpers;
using WEBAPI.Infrastructure.DB;

namespace WEBAPI.Infrastructure.Service
{
    public class AuthService : IAuthService
    {
        private List<User> users;
        ApplicationDbContext AppDb;
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appSettings;

        public AuthService(IOptions<AppSettings> appSettings, IConfiguration configuration)
        {
            _appSettings = appSettings.Value;
            AppDb = new ApplicationDbContext(_appSettings.ConnectionString);
            _configuration = configuration;
            users = AppDb.GetUsers();
        }
        public User Authenticate(string username, string password)
        {
            var user = users.SingleOrDefault(x => x.Username == username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            // Authentication successful, generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            if (double.TryParse(_configuration["Jwt:TokenExpiryDays"], out double tokenExpiryDays))
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("UserName", user.Username.ToString()),
                    }),

                    Expires = DateTime.UtcNow.AddDays(tokenExpiryDays),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.Token = tokenHandler.WriteToken(token);

         
                user.PasswordHash = null;

            }
            return user;
        }


        public string Register(User user)
        {
            return AppDb.RegisterUser(user);
        }
    }
}
