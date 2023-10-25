using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Task_4_NETMVCandRazorPages.Data;
using Task_4_NETMVCandRazorPages.Models.Domain;
using Task_4_NETMVCandRazorPages.View;

namespace Task_4_NETMVCandRazorPages.Pages.Items
{
    [Authorize]
    public class AddItemModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        [BindProperty]
        public AddItem addItemRequest { get; set; }

        public AddItemModel(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

   
        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            var itemDomainModel = new Item
            {
                Name = addItemRequest.Name,
                Description = addItemRequest.Description
            };
            dbContext.Items.Add(itemDomainModel);
            dbContext.SaveChanges();

            ViewData["Message"] = "Item Created Successfully!";

            return RedirectToPage("/Items/List");
        }
    }
}
