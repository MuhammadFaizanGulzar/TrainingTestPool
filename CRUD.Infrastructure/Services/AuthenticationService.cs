using CRUD.Application.Interfaces;
using CRUD.Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CRUD.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationservice
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(UserManager<User> userManager, IConfiguration configuration, RoleManager<Role> roleManager, AppDbContext context, Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<IdentityResult> Register(User user, string password, string role)
        {
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                var roleEntity = await _roleManager.FindByNameAsync(role);
                if (result.Succeeded)
                {
                    // Find the role based on the role name
                   
                    if (roleEntity != null)
                    {
                        var userRole = new CRUD.Domain.Models.UserRole
                        {
                            UserId = user.Id,
                            RoleId = roleEntity.Id
                        };
                        _context.UserRoles.Add(userRole);

                        await _context.SaveChangesAsync();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"An error occurred: {ex.Message}" });
            }
        }

        public async Task<AuthResponse> Authenticate(string username, string password, HttpContext httpContext)
        {
            try
            {

                var user = await _userManager.FindByNameAsync(username);
                var roles = await _userManager.GetRolesAsync(user);
                if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                {
                    return new AuthResponse
                    {
                        message = "User Does Not Exist"
                    };
                }

                var passwordHasher = new PasswordHasher<User>();
                var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

                if (passwordVerificationResult == PasswordVerificationResult.Success)
                {
                    var claims = new[]
                    {
                        new Claim("id", user.Id.ToString()),
                        new Claim("Name", user.UserName),
                        new Claim("Email", user.Email),
                        new Claim("Role", string.Join(",", roles))
                    };

                    var jwtKey = _configuration["Jwt:Key"];
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])); // Replace with your secret key
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                    issuer: null, // your issuer
                    audience: null, // your audience
                    claims: claims,
                    expires: DateTime.Now.AddHours(1), // adjust the expiration time
                    signingCredentials: creds
                    );
                    string stringToken = new JwtSecurityTokenHandler().WriteToken(token);

                    if (string.IsNullOrEmpty(stringToken))
                    {
                        return new AuthResponse
                        {
                            message = "Incorrect Data, Token not generatedS"
                        };
                    }
                    httpContext.Response.Cookies.Append("jwt", stringToken, new CookieOptions { HttpOnly = true });

                    return new AuthResponse
                    {
                        Token = stringToken,

                    };
                }
                else
                {
                    return new AuthResponse
                    {
                        message = "Incorrect Email or Password"
                    };
                }
          
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> GetUserById(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }
        }

    }
}
