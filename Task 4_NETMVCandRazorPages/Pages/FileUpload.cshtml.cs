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

        public string ErrorMessage { get; set; }

        public FileUploadModel(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void OnGet()
        {
        }

        //public async Task<IActionResult> OnPostAsync(IFormFile excelFile)
        //{
        //    if (excelFile != null && excelFile.Length > 0)
        //    {
        //        using (var stream = new MemoryStream())
        //        {
        //            await excelFile.CopyToAsync(stream);
        //            using (var package = new ExcelPackage(stream))
        //            {
        //                var worksheet = package.Workbook.Worksheets[0]; // Assuming the data is in the first sheet

        //                int rowCount = worksheet.Dimension.Rows;

        //                for (int row = 2; row <= rowCount; row++) //  For Skipping the header row
        //                {
        //                    var name = worksheet.Cells[row, 1].Text;
        //                    var description = worksheet.Cells[row, 2].Text;


        //                    var itemDomainModel = new Item
        //                    {
        //                        Name = name,
        //                        Description = description
        //                    };
        //                    dbContext.Items.Add(itemDomainModel);
        //                    dbContext.SaveChanges();
        //                }


        //            }
        //        }
        //    }

        //    return RedirectToPage("/Index");
        //}

        public async Task<IActionResult> OnPostAsync(IFormFile excelFile)
        {
            bool duplicateDataExists = false; // Flag to track duplicate data

            if (excelFile != null && excelFile.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0]; // Assuming the data is in the first sheet

                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++) // For skipping the header row
                        {
                            var name = worksheet.Cells[row, 1].Text;
                            var description = worksheet.Cells[row, 2].Text;

                            // Check if an item with the same name and description exists in the database
                            var existingItem = dbContext.Items.FirstOrDefault(item =>
                                item.Name == name && item.Description == description);

                            if (existingItem == null)
                            {
                                // If the item does not exist, add it to the database
                                var itemDomainModel = new Item
                                {
                                    Name = name,
                                    Description = description
                                };
                                dbContext.Items.Add(itemDomainModel);
                            }
                            else
                            {
                                // Duplicate data found, set the flag
                                duplicateDataExists = true;
                            }
                        }

                        // Save changes to the database after processing all rows
                        dbContext.SaveChanges();
                    }
                }
            }

            if (duplicateDataExists)
            {
                TempData["Message"] = "Duplicate data exists in the uploaded file.";
            }
            else
            {
                TempData["Message"] = "Data uploaded successfully.";
            }

            return RedirectToPage("/FileUpload");
        }
    }
}
