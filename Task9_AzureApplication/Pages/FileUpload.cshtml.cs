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
        private readonly IHttpClientFactory _clientFactory;

        public FileUploadModel(IConfiguration configuration, IHubContext<NotificationHub> hubContext, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _hubContext = hubContext;
            _clientFactory = clientFactory;
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

            // Connection string for your Azure Storage account
            string storageConnectionString = _configuration.GetConnectionString("AzureStorageAccount");

            // Create a CloudStorageAccount object using the connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to the container
            CloudBlobContainer container = blobClient.GetContainerReference("task9container");

            string blobName = Path.GetFileName(file.FileName);

            // Get a reference to the blob
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            // Read the content of the blob
            string fileContent = await blob.DownloadTextAsync();

            // Use the fileContent as needed, for example, pass it to the view
            ViewData["ProcessedData"] = fileContent;

            TempData["ProcessedData"] = fileContent;

            // Send a message to Azure Service Bus with details about the uploaded file
            await SendMessageToServiceBus(blobName);

            // Notify clients using SignalR
            await _hubContext.Clients.All.SendAsync("SendNotification", "Processing is complete!");

            return RedirectToPage("FileUpload");
        }

       
        private async Task SendMessageToServiceBus(string blobName)
        {
            // connection string for Azure Service Bus
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
