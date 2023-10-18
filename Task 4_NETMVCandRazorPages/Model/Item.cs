using System.ComponentModel.DataAnnotations;

namespace Task_4_NETMVCandRazorPages.Model
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Items { get; set; }
        public int DisplayOrder { get; set; }
            
    }
}
