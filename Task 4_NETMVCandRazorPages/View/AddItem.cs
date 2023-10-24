using System.ComponentModel.DataAnnotations;

namespace Task_4_NETMVCandRazorPages.View
{
    public class AddItem
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }
    }
}
