using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Task9_AzureApplication.Pages
{
    public class FileUploadModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public FileUploadModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                // Handle the case where no file is selected for upload
                ModelState.AddModelError("File", "Please select a file.");
                return Page();
            }

            // Get the connection string for your Azure Storage account
            string storageConnectionString = _configuration.GetConnectionString("AzureStorageAccount");

            // Create a CloudStorageAccount object using the connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create a CloudBlobClient object using the storage account
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to the container
            CloudBlobContainer container = blobClient.GetContainerReference("your-container-name");

            // Create a unique name for the blob (you can customize this logic)
            string blobName = Path.GetFileName(file.FileName);

            // Get a reference to the blob
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            // Upload the file to the blob
            using (var stream = file.OpenReadStream())
            {
                await blob.UploadFromStreamAsync(stream);
            }

            // Optionally, you can perform additional processing here

            // Redirect to a success page or perform other actions
            return RedirectToPage("SuccessPage");
        }
    }
}
