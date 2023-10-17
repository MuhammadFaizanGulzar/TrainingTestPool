using CRUD.Domain.Models;
using CRUD.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CRUD.Application.Interfaces;
using System.Data;
using System.Security.Claims;

namespace Web_API_CRUD.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IAuthenticationservice _authenticationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TodoItemsController(AppDbContext context, UserManager<User> userManager, IAuthenticationservice authenticationService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _authenticationService = authenticationService;
            _httpContextAccessor = httpContextAccessor;

        }

        // GET /api/todo
        [HttpGet("getAll")]

        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodos()
        {

            int? userId = null;
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("id")?.Value;
            if (userIdClaim != null && int.TryParse(userIdClaim, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var user = await _authenticationService.GetUserById((int)userId);
            var roles = await _userManager.GetRolesAsync(user);


            // Check if the user has the "Admin" role
            if (roles.Contains("Admin"))
            {
                var todos = await _context.Todos.ToListAsync();
                return Ok(todos);
            }
            else if (roles.Contains("User"))
            {
         
                if (userId != null)
                {
                    var todosUser = await _context.Todos
                        .Where(todo => todo.UserId == userId)
                        .ToListAsync();

                    return Ok(todosUser);
                }          
            }
            var response = new
            {
                Message = "Unauthorized or no role specified"
            };
            return BadRequest(response);
        }

        //GET/api/todo/{id}
        [HttpGet("getById")]
        public async Task <ActionResult<TodoItem>> GetTodoById(Guid id)
        {
            var todo = await _context.Todos.FindAsync(id);

            if (todo == null)
            {
                return NotFound(); // Return 404
            }

            return Ok(todo);
        }


        //POST/api/todo
        [HttpPost("create")]
        public async Task<ActionResult<TodoItem>> createTodo(TodoItem todo)
        {

            int? userId = null;
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("id")?.Value;
            if (userIdClaim != null && int.TryParse(userIdClaim, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var user = await _authenticationService.GetUserById((int)userId);
            var roles = await _userManager.GetRolesAsync(user);

            if (ModelState.IsValid)
            {
                todo.Id = Guid.NewGuid();
                todo.CreatedDate = DateTime.UtcNow;
                todo.UpdatedDate = DateTime.UtcNow;
                todo.UserId = user.Id;

                _context.Todos.Add(todo);
                await _context.SaveChangesAsync();

  
                var response = new
                {
                    Message = $"Todo item with ID {todo.Id} created successfully"
                };

                return Ok(response); // Return status 200 
            }
            else
            {
                return BadRequest(ModelState); // Return status 400
            }
        }

        // PUT /api/todo/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(Guid id, TodoItem updatedTodo)
        {

            int? userId = null;
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("id")?.Value;
            if (userIdClaim != null && int.TryParse(userIdClaim, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var existingTodo = await _context.Todos.FindAsync(id);

            if (existingTodo == null)
            {
                return NotFound(); // Return status 404 if the todo item doesn't exist
            }

            // Get the user's roles from the claims
            var userRoles = _httpContextAccessor.HttpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            if (userRoles.Contains("Admin") || (userRoles.Contains("User") && existingTodo.UserId == userId))
            {
                existingTodo.Title = updatedTodo.Title;
                existingTodo.IsCompleted = updatedTodo.IsCompleted;
                existingTodo.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var response = new
                {
                    Message = $"Todo item with ID {id} updated successfully"
                };

                return Ok(response);
            }
            else
            {
                return Forbid(); // User is not authorized to update the record
            }
        }

        // DELETE /api/todo/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(Guid id)
        {
            int? userId = null;
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("id")?.Value;
            if (userIdClaim != null && int.TryParse(userIdClaim, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var todo = await _context.Todos.FindAsync(id);

            if (todo == null)
            {
                return NotFound(); // Return status 404 if the todo item doesn't exist
            }

            // Get the user's roles from the claims
            var userRoles = _httpContextAccessor.HttpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            if (userRoles.Contains("Admin") || (userRoles.Contains("User") && todo.UserId == userId))
            {
                _context.Todos.Remove(todo);
                await _context.SaveChangesAsync();

                var response = new
                {
                    Message = $"Todo item with ID {id} deleted successfully"
                };

                return Ok(response);
            }
            else
            {
                return Forbid(); // User is not authorized to delete the record
            }
        }


    }
}
