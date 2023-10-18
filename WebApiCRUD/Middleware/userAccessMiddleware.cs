using CRUD.Domain.Models;
using CRUD.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApiCRUD.CustomAttribute;

namespace WebApiCRUD.Middleware
{

    public class userAccessMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppDbContext _context;

        public userAccessMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<User> userManager, AppDbContext dbcontext)
        {
            var hasCheckUserAccessAttribute = context.GetEndpoint()?.Metadata?.GetMetadata<CheckUserAccessAttribute>() != null;

            if (hasCheckUserAccessAttribute && context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.FindFirst("id")?.Value;
                var todoIdString = context.Request.RouteValues["Id"]?.ToString(); // Adjust this based on your routing.

                if (!string.IsNullOrEmpty(userId) && Guid.TryParse(todoIdString, out Guid todoId))
                {

                    var user = await userManager.FindByIdAsync(userId);

                    var roles = await userManager.GetRolesAsync(user);

                    var isAdmin = await userManager.IsInRoleAsync(user, "Admin");

                    if (!roles.Contains("Admin"))
                    {                       
                        // Check if the ToDo with the given ID belongs to the authenticated user.
                        var todo = await GetUserTodoById(user, todoId, dbcontext);

                        if (todo == null || user == null || todo.UserId != user.Id)
                        {
                            context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                            await context.Response.WriteAsJsonAsync(new { message = "Permission Denied" });
                            return;
                        }
                    }
                }
            }

            await _next(context);
        }

        public async Task<TodoItem> GetUserTodoById(User user, Guid todoId, AppDbContext dbcontext)
        {
         
            var todo = await dbcontext.Todos
           .Where(t => t.Id == todoId && t.UserId == user.Id)
           .FirstOrDefaultAsync();

            return todo;
        }
    }
}
