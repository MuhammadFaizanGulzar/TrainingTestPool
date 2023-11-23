using Microsoft.EntityFrameworkCore;
using Task7_Razor_AWS_SQS_SNS.Model;

namespace Task7_Razor_AWS_SQS_SNS
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
