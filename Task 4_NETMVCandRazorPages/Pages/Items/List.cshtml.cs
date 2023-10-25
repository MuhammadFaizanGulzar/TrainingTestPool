using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Task_4_NETMVCandRazorPages.Data;
using Microsoft.Extensions.Configuration;
using Task_4_NETMVCandRazorPages.Models.Domain;
using Task_4_NETMVCandRazorPages.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Task_4_NETMVCandRazorPages.Pages.Items
{
    [Authorize]
    public class ListModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ListModel> _logger;

        public ListModel(ApplicationDbContext dbContext, IConfiguration configuration, ILogger<ListModel> logger)
        {
            this.dbContext = dbContext;
            _configuration = configuration;
            _logger = logger;
        }

        public string NameSort { get; set; }
        public string DescriptionSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public PaginatedList<Item> Items { get; set; }

        public async Task OnGet(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {


            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //NameSort = sortOrder == "name_desc" ? "descending" : "ascending";
            DescriptionSort = String.IsNullOrEmpty(sortOrder) ? "description_desc" : "";
            //DescriptionSort = sortOrder == "description_desc" ? "descending" : "ascending";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            IQueryable<Item> items = dbContext.Items; // Avoid calling ToList here

            if (!string.IsNullOrEmpty(searchString))
            {
                items = items.Where(s => s.Name!.ToLower().Contains(searchString));
            }
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

            var pageSize = _configuration.GetValue("PageSize", 3);

            //Items = new PaginatedList<Item>(
            //    await items.AsNoTracking().ToListAsync(), pageIndex ?? 1, pageSize, items.Count());

            Items = await PaginatedList<Item>.CreateAsync(items.AsNoTracking(), pageIndex ?? 1, pageSize);


            //_logger.LogInformation($"Number of items in the query result: {Items.TotalCount}");
        }
    }
}