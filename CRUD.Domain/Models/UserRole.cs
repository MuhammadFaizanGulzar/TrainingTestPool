using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Domain.Models
{
    public class UserRole : IdentityUserRole<int>
    {
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
