using System;
using System.Threading.Tasks;
using CRUD.Domain.Models;
using CRUD.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserAccessActionFilter : IAsyncActionFilter
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;

    public UserAccessActionFilter(UserManager<User> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.User.Identity.IsAuthenticated)
        {
            var userId = context.HttpContext.User.FindFirst("id")?.Value;
            var todoIdString = context.RouteData.Values["Id"]?.ToString(); // Adjust this based on your routing.

            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(todoIdString, out Guid todoId))
            {
                var user = await _userManager.FindByIdAsync(userId);

                var roles = await _userManager.GetRolesAsync(user);

                if (!roles.Contains("Admin"))
                {
                    var todo = await GetUserTodoById(user, todoId);

                    if (todo == null || todo.UserId != user.Id)
                    {
                        context.Result = new UnauthorizedObjectResult(new { message = "Permission Denied" });
                        return;
                    }
                }
            }
        }

        await next();
    }

    private async Task<TodoItem> GetUserTodoById(User user, Guid todoId)
    {
        var todo = await _context.Todos
            .Where(t => t.Id == todoId && t.UserId == user.Id)
            .FirstOrDefaultAsync();

        return todo;
    }
}