using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Task_4_NETMVCandRazorPages.Data;
using Task_4_NETMVCandRazorPages.View;

namespace Task_4_NETMVCandRazorPages.Pages.Items
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        [BindProperty]
        public EditItem editItemView { get; set; }

        public EditModel(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void OnGet(Guid id)
        {
            var items = dbContext.Items.Find(id);

            if(items != null)
            {
                editItemView = new EditItem()
                {
                    Id = items.Id,
                    Name = items.Name,
                    Description = items.Description
                };
            }
        }

        public IActionResult OnPostUpdate()
        {

            if(editItemView!= null)
            {
               var existingItem = dbContext.Items.Find(editItemView.Id);

                if(existingItem !=null)
                {
                    existingItem.Name = editItemView.Name;
                    existingItem.Description = editItemView.Description;

                    dbContext.SaveChanges();

                    ViewData["Message"] = "Item Updates Successfully";

               

                }
            }
            return RedirectToPage("/Items/List");
        }

        public IActionResult OnPostDelete()
        {
            var existingItem = dbContext.Items.Find(editItemView.Id);

            if(existingItem != null)
            {
                dbContext.Items.Remove(existingItem);
                dbContext.SaveChanges();

                return RedirectToPage("/Items/List");
            }

            return Page();
        }
    }
}
