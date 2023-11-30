using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.ServiceBus;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Task9_AzureApplication.Helper;

namespace Task9_AzureApplication.Pages
{
    public class FileUploadModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<NotificationHub> _hubContext;

        public FileUploadModel(IConfiguration configuration, IHubContext<NotificationHub> hubContext)
        {
            _configuration = configuration;
            _hubContext = hubContext;
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
            CloudBlobContainer container = blobClient.GetContainerReference("task9container");

            // Create a unique name for the blob (you can customize this logic)
            string blobName = Path.GetFileName(file.FileName);

            // Get a reference to the blob
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            // Check if the blob already exists
            if (await blob.ExistsAsync())
            {
                // Optionally, you can perform additional logic here
                // For example, you might want to create a backup of the existing file before replacing it.

                // Delete the existing blob before uploading the new file
                await blob.DeleteIfExistsAsync();
            }

            // Upload the file to the blob
            using (var stream = file.OpenReadStream())
            {
                await blob.UploadFromStreamAsync(stream);
            }


            // Send a message to Azure Service Bus with details about the uploaded file
            await SendMessageToServiceBus(blobName);

            // Optionally, you can perform additional processing here

            // Retrieve processed data from Azure Function
            var processedDataResponse = await RetrieveProcessedDataFromAzureFunction(blobName);

            // Display data on the frontend
            ViewData["ProcessedData"] = processedDataResponse;

            // Notify clients using SignalR
            await _hubContext.Clients.All.SendAsync("SendNotification", "Processing is complete!");

            // Redirect to a success page or perform other actions
            return RedirectToPage("index");
        }

        private async Task<string> RetrieveProcessedDataFromAzureFunction(string blobName)
        {
            try
            {
                // Construct the URL for your Azure Function endpoint
                string requestUrl = "http://localhost:7250/api/ProcessFileFunction";

                using (HttpClient httpClient = new HttpClient())
                {
                    // Log the request URL
                    Console.WriteLine($"Sending request to: {requestUrl}");

                    // Make an HTTP GET request
                    HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

                    // Log the response status code
                    Console.WriteLine($"Response status code: {response.StatusCode}");

                    // Check if the response is successful
                    response.EnsureSuccessStatusCode();

                    // Read the content from the response
                    string content = await response.Content.ReadAsStringAsync();

                    // Log the response content
                    Console.WriteLine($"Response content: {content}");

                    // Return the processed data
                    return content;
                }
            }
            catch (HttpRequestException ex)
            {
                // Log the exception or handle it accordingly
                Console.WriteLine($"HTTP request failed: {ex.Message}");
                throw;
            }
        }
        private async Task SendMessageToServiceBus(string blobName)
        {
            // Get the connection string for your Azure Service Bus namespace
            string serviceBusConnectionString = _configuration.GetConnectionString("AzureServiceBus");

            // Create a ServiceBusClient
            var serviceBusClient = new ServiceBusClient(serviceBusConnectionString);

            // Create a sender for the queue
            var sender = serviceBusClient.CreateSender("task9queue");

            try
            {
                // Create a message with details about the uploaded file
                var messageBody = new { FileName = blobName };
                var messageJson = JsonConvert.SerializeObject(messageBody);
                var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageJson));

                // Send the message to the queue
                await sender.SendMessageAsync(serviceBusMessage);
            }
            finally
            {
                // Close the sender and client
                await sender.CloseAsync();
                //await serviceBusClient.CloseAsync();
            }
        }
    }

}
