

using System.ComponentModel.DataAnnotations;

namespace Task_4_NETMVCandRazorPages.View
{
    public class Register
    {
        [Required]
        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
