using CRUD.Application.Interfaces;
using CRUD.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApiCRUD.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticationservice _authenticationService;
        public AuthController(UserManager<User> userManager, RoleManager<Role> roleManager, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IAuthenticationservice authenticationService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _authenticationService = authenticationService;
            _httpContextAccessor = httpContextAccessor;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {
            try
            {
                // Check user existence
                var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
                if (userExist != null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new Response { Status = "Error", Message = "User Already exists!" });
                }

                // Determine the role based on user input
                string role = registerUser.role; // Assuming you pass the role from the client

                User user = new()
                {
                    Email = registerUser.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = registerUser.userName,
                    Password = registerUser.Password
                };

                var registrationResult = await _authenticationService.Register(user, registerUser.Password, role);

                if (registrationResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status201Created, new Response { Status = "Success", Message = "User Registered Successfully!" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User Registration Failed!" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User Registration Failed!" });
            }
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginUser)
        {
            try
            {
                var authResponse = await _authenticationService.Authenticate(loginUser.Email.ToLower(), loginUser.Password, _httpContextAccessor.HttpContext);


                if (authResponse?.Token == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Login Failed!" });

                }

                return StatusCode(StatusCodes.Status201Created, new Response { Status = authResponse?.message, Message = "Login Successfull!" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Login Failed" });
            }
        }
    }
}
