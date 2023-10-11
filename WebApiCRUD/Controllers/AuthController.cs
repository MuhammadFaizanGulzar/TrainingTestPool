using CRUD.Application.Interfaces;
using CRUD.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApiCRUD.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationservice _authenticationService;
        public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IAuthenticationservice authenticationService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]

        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser,string role)
        {
            //check user exist
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if(userExist!= null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new Response { Status = "Error", Message = "User Already exists!" });
            }

            //Add User
            IdentityUser user = new()
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.userName
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if(result.Succeeded)
            {
                return StatusCode(StatusCodes.Status201Created, new Response { Status = "Success", Message = "User Registered Successfully!" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User Registered Failed!" });
            }
        }
    }
}
