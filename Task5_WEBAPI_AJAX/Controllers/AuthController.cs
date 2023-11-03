using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using WEBAPI.Application.Interfaces;
using WEBAPI.Domain.Entities;
using WEBAPI.Domain.Models;

namespace Task5_WEBAPI_AJAX.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        private IAuthService _authService;

        public AuthController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }


        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Authenticate([FromBody] LoginRequest userParam)
        {
            var user = _authService.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Username or password is incorrect" });
            }

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Token
            });
        }

        [HttpPost("RegisterUser")]
        public IActionResult RegisterUser([FromBody] User userParam)
        {
            string message = _authService.Register(userParam);
            return Ok("message=" + message);
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }
    }
}
