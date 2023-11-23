using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Task7_Razor_AWS_SQS_SNS.Model;

namespace Task7_Razor_AWS_SQS_SNS.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public List<User> Users { get; set; }

        public IndexModel(ILogger<IndexModel> logger, DbContextOptions<ApplicationDbContext> dbContextOptions)
        {
            _logger = logger;
            _dbContextOptions = dbContextOptions;
        }

       public void OnGet()
        {
            // Retrieve data from your database or any other source
            // Set the retrieved data to the Users property
            Users = GetUserDataFromDatabase();
        }

        private List<User> GetUserDataFromDatabase()
        {
            using (var dbContext = new ApplicationDbContext(_dbContextOptions))
            {
                // Retrieve data from the Users table
                return dbContext.Users.ToList();
            }
        }
    }
}