using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Domain.Models
{
    public class RegisterUser
    {
        [Required]
        public string? userName { get; set; }

        [Required]

        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

    }
}
