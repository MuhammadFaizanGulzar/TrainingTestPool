using WEBAPI.Domain.Entities;
using System.Data;
using WEBAPI.Application.Interfaces;
using WEBAPI.Infrastructure.DB;
using Microsoft.Extensions.Options;
using WEBAPI.Domain.Helpers;
using Microsoft.Extensions.Configuration;

namespace WEBAPI.Infrastructure.Service
{
    public class UserService : IUserService
    {
        private List<User> users;
        ApplicationDbContext AppDb;

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            AppDb = new ApplicationDbContext(_appSettings.ConnectionString);
            users = AppDb.GetUsers();

        }
       
        public string Register(User user)
        {
            return AppDb.RegisterUser(user);
        }

        public IEnumerable<User> GetAll()
        {
            return users.Select(x =>
            {
                x.PasswordHash = null;
                return x;
            });
        }
    }
}
