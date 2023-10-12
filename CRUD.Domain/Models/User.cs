using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CRUD.Domain.Models
{
    public class User : IdentityUser<int>
    {
        [Required]

        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
