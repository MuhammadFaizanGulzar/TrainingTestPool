using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Task_4_NETMVCandRazorPages.Data;
using Task_4_NETMVCandRazorPages.Models.Domain;
using Task_4_NETMVCandRazorPages.View;

namespace Task_4_NETMVCandRazorPages.Pages
{
    public class FileUploadModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public FileUploadModel(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(IFormFile excelFile)
        {
            if (excelFile != null && excelFile.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0]; // Assuming the data is in the first sheet

                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++) // Skip the header row
                        {
                            var name = worksheet.Cells[row, 1].Text;
                            var description = worksheet.Cells[row, 2].Text;
                            //var price = decimal.Parse(worksheet.Cells[row, 2].Text);

                            var itemDomainModel = new Item
                            {
                                Name = name,
                                Description = description
                            };
                            dbContext.Items.Add(itemDomainModel);
                            dbContext.SaveChanges();
                        }

                    
                    }
                }
            }

            return RedirectToPage("/Index");
        }
    }
}
