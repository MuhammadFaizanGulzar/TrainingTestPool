using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Domain.Models
{
    public class AuthResponse
    {
        public string Token { get; set; }

        public string message { get; set; }
    }
}
