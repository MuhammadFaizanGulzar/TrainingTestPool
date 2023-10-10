using CRUD.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUD.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TodoItem> Todos { get; set; }
    }
}