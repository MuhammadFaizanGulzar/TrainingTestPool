using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Task_4_NETMVCandRazorPages.Data;
using Task_4_NETMVCandRazorPages.Models.Domain;

namespace Task_4_NETMVCandRazorPages.Pages.Items
{
    [Authorize]
    public class ListModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        private readonly IConfiguration _configuration;

        public string NameSort { get; set; }
        public string DescriptionSort { get; set; }
        public string CurrentFilter { get; set; }
        public List<Models.Domain.Item> Items { get; set; }
        public ListModel(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            _configuration = configuration;
        }
        public async void OnGet(string sortOrder, string searchString, int? pageIndex)
        {
            // using System;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DescriptionSort = String.IsNullOrEmpty(sortOrder) ? "description_desc" : "";

            CurrentFilter = searchString;
            if(searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = CurrentFilter;
            }

            IQueryable<Item> items = from s in dbContext.Items
                                             select s;
            switch (sortOrder)
            {
                case "name_desc":
                    items = items.OrderByDescending(s => s.Name);
                    break;
                case "description_desc":
                    items = items.OrderByDescending(s => s.Description);
                    break;
                default:
                    items = items.OrderBy(s => s.Name);
                    break;
            }

            var pageSize = _configuration.GetValue("PageSize", 4);
            //Items = await List<Item>.CreateAsync(items.AsNoTracking(), pageIndex ?? 1, pageSize);
                 
            //Items = dbContext.Items.ToList();
        }

    }
}
