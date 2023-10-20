using System;
using System.Threading.Tasks;
using CRUD.Domain.Models;
using CRUD.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserAccessActionFilter : IActionFilter
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;

    public UserAccessActionFilter(UserManager<User> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
       
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.User.Identity!.IsAuthenticated)
        {
            var userId = context.HttpContext.User.FindFirst("id")?.Value;
            var todoIdString = context.RouteData.Values["Id"]?.ToString();

            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(todoIdString, out Guid todoId))
            {
                var user = _userManager.FindByIdAsync(userId).Result;

                var roles = _userManager.GetRolesAsync(user).Result;

                if (!roles.Contains("Admin"))
                {
                    var todo = GetUserTodoById(user, todoId);


                    if (todo == null || todo.UserId != user.Id)
                    {
                        context.Result = new UnauthorizedObjectResult(new { message = "Permission Denied" });
                        return;
                    }
                }
            }
        }
    }


    private TodoItem GetUserTodoById(User user, Guid todoId)
    {
        var todo =  _context.Todos
            .Where(t => t.Id == todoId && t.UserId == user.Id)
            .FirstOrDefault();

        return todo!;
    }
}