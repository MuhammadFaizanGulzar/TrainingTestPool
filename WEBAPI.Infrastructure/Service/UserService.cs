using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEBAPI.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using WEBAPI.Application.Interfaces;
using WEBAPI.Infrastructure.DB;
using Microsoft.Extensions.Options;
using WEBAPI.Domain.Helpers;
using System.Security.Cryptography;

namespace WEBAPI.Infrastructure.Service
{
    public class UserService : IUserService
    {
        private List<User> users;
        ApplicationDbContext AppDb;

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            AppDb = new ApplicationDbContext(_appSettings.ConnectionString);
            users = AppDb.GetUsers();
        }


        public User Authenticate(string username, string password)
        {
            var user = users.SingleOrDefault(x => x.Username == username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            // Generate a strong key for token generation (256 bits)
            byte[] keyBytes = Generate256BitKey();
            var key = new SymmetricSecurityKey(keyBytes);

            // Authentication successful, generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // Remove the password hash before returning
            user.PasswordHash = null;

            return user;
        }

        private byte[] Generate256BitKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] key = new byte[32]; // 32 bytes = 256 bits
                rng.GetBytes(key);
                return key;
            }
        }
       

        public string Register(User user)
        {
            return AppDb.RegisterUser(user);
        }


        public IEnumerable<User> GetAll()
        {
            return users.Select(x =>
            {
                x.PasswordHash = null;
                return x;
            });
        }
    }
}
