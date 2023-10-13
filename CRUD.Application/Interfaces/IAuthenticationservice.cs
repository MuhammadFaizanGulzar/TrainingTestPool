using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using CRUD.Domain.Models;

namespace CRUD.Application.Interfaces
{
    public interface IAuthenticationservice
    {
        Task<IdentityResult> Register(User user, string password, string role);
        Task<AuthResponse> Authenticate(string username, string password, HttpContext httpContext);
        Task<User> GetUserById(int id);

    }
}
