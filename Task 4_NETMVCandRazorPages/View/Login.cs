using System.ComponentModel.DataAnnotations;

namespace Task_4_NETMVCandRazorPages.View
{
    public class Login
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }


    }
}
