using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3.Transfer;

namespace Task7_Razor_AWS_SQS_SNS.Pages
{
    public class FileUploadModel : PageModel
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IAmazonSimpleNotificationService _snsClient;
        private readonly IAmazonSQS _sqsClient;

        public FileUploadModel(IAmazonS3 s3Client, IAmazonSimpleNotificationService snsClient, IAmazonSQS sqsClient)
        {
            _s3Client = s3Client;
            _snsClient = snsClient;
            _sqsClient = sqsClient;
        }

        public void OnGet()
        {
            // Initialization or additional logic if needed
        }

        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            if (file != null && file.Length > 0 && (Path.GetExtension(file.FileName).ToLower() == ".json") && (file.ContentType.ToLower() == "application/json"))
            {
                string bucketName = "task7-sqs-sns-bucket";
                string key = "uploads/" + file.FileName;

                using (var fileStream = file.OpenReadStream())
                {
                    var fileTransferUtility = new TransferUtility(_s3Client);
                    await fileTransferUtility.UploadAsync(fileStream, bucketName, key);
                }

                return RedirectToPage("/FileUpload"); // You can customize the redirect target
            }

            // Handle the case where no file is selected
            ModelState.AddModelError("File", "Please select a file.");

            // Return to the same page with validation errors
            return Page();
        }
    }
}