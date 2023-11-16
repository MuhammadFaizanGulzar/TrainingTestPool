using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Task7_Razor_AWS_SQS_SNS.Pages
{

    public class FileUploadModel : PageModel
    {
        private readonly IAmazonS3 _s3Client; // Inject Amazon S3 client through dependency injection

        public FileUploadModel(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public void OnGet()
        {
            // Initialization or additional logic if needed
        }

        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            if (file != null && file.Length > 0 &&
                (Path.GetExtension(file.FileName).ToLower() == ".json") &&
                (file.ContentType.ToLower() == "application/json"))
            { 
                // Specify your S3 bucket name and the desired key (object name) for the file
                string bucketName = "task7-sqs-sns-bucket";
                string key = "uploads/" + file.FileName;

                // Upload the file to S3
                using (var fileStream = file.OpenReadStream())
                {
                    var fileTransferUtility = new TransferUtility(_s3Client);
                    await fileTransferUtility.UploadAsync(fileStream, bucketName, key);
                }

                // You can add additional logic here, such as triggering the Lambda function
                // or sending a notification to SNS

                // Redirect to a confirmation page or notify the user
                return RedirectToPage("/Index"); // You can customize the redirect target
            }

            // Handle the case where no file is selected
            ModelState.AddModelError("File", "Please select a file.");

            // Return to the same page with validation errors
            return Page();
        }
    }

}

