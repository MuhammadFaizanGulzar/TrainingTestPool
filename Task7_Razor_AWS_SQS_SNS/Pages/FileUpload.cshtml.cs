using Amazon.Lambda;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Task7_Razor_AWS_SQS_SNS.Pages
{

    public class FileUploadModel : PageModel
    {
        private readonly IAmazonS3 _s3Client; // Inject Amazon S3 client through dependency injection
        private readonly IAmazonSimpleNotificationService _snsClient;
        private readonly ILogger<FileUploadModel> _logger;

        public FileUploadModel(IAmazonS3 s3Client, IAmazonSimpleNotificationService snsClient, ILogger<FileUploadModel> logger)
        {
            _s3Client = s3Client;
            _snsClient = snsClient;
            _logger = logger;
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

                // Trigger the Lambda function
                await TriggerLambdaFunctionAsync(bucketName, key);

                // Process the file and publish to SNS (simulate Lambda function behavior)
                var processedData = ProcessFile(bucketName, key);
                await PublishToSns(processedData);
      
                // Redirect to a confirmation page or notify the user
                return RedirectToPage("/FileUpload"); // You can customize the redirect target
            }

            // Handle the case where no file is selected
            ModelState.AddModelError("File", "Please select a file.");

            // Return to the same page with validation errors
            return Page();
        }

        private async Task TriggerLambdaFunctionAsync(string bucketName, string key)
        {
            try
            {
                // Specify the ARN of your Lambda function
                string lambdaFunctionArn = "arn:aws:lambda:eu-north-1:090433784189:function:task7FileUploadTrigger";

                var lambdaClient = new AmazonLambdaClient();

                var request = new Amazon.Lambda.Model.InvokeRequest
                {
                    FunctionName = lambdaFunctionArn,
                    InvocationType = InvocationType.Event, // Set to 'Event' to trigger asynchronously
                    Payload = $"{{\"bucket\": \"{bucketName}\", \"key\": \"{key}\"}}"
                };

                await lambdaClient.InvokeAsync(request);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur while triggering the Lambda function
                Console.WriteLine($"Error triggering Lambda function: {ex.Message}");
            }
        }
        private string ProcessFile(string bucket, string key)
        {
            Console.WriteLine($"Processing file: {key} in bucket: {bucket}");
            _logger.LogInformation($"Processing file: {key} in bucket: {bucket}");

            return JsonConvert.SerializeObject(new { status = "success", message = "File processed successfully" });
        }

        private async Task PublishToSns(string message)
        {
            // Simulate publishing a message to an SNS topic (replace with actual SNS logic)
            Console.WriteLine($"Simulating message publishing to SNS: {message}");

            try
            {
                // Specify the ARN of your SNS topic
                string snsTopicArn = "arn:aws:sns:eu-north-1:090433784189:task7Sns";

                var request = new PublishRequest
                {
                    TopicArn = snsTopicArn,
                    Message = message,
                    MessageStructure = "json"

                };


                var response = await _snsClient.PublishAsync(request);
                LambdaLogger.Log("Published message to SNS. MessageId");
                _logger.LogInformation($"Published message to SNS. MessageId: {response.MessageId}");
   
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error publishing message to SNS: {ex.Message}");
                // Handle any exceptions that may occur while publishing to SNS
            }
        }
    }

}

