using CRUD.Domain.Models;
using CRUD.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace Web_API_CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {

        private readonly AppDbContext _context;

        public TodoItemsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/todo
        [HttpGet("getAll")]

        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodos()
        {
            var todos = await _context.Todos.ToListAsync();
            return Ok(todos);
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
            if (ModelState.IsValid)
            {
                todo.Id = Guid.NewGuid();
                todo.CreatedDate = DateTime.UtcNow;
                todo.UpdatedDate = DateTime.UtcNow;

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
            var existingTodo = await _context.Todos.FindAsync(id);

            if (existingTodo == null)
            {
                return NotFound(); // Return status 404 
            }

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

        // DELETE /api/todo/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(Guid id)
        {
            var todo = await _context.Todos.FindAsync(id);

            if (todo == null)
            {
                return NotFound(); // Return status 404 
            }

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            var response = new
            {
                Message = $"Todo item with ID {id} deleted successfully"
            };

            return Ok(response);
        }


    }
}
