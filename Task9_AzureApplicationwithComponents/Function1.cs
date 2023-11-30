using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Newtonsoft.Json;

namespace Task9_AzureApplicationwithComponents
{
    public static class ProcessFileFunction
    {
        [FunctionName("ProcessFileFunction")]
        public static async Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
      [SignalR(HubName = "notificationHub")] IAsyncCollector<SignalRMessage> signalRMessages,
      ILogger log)
        {
            try
            {
                // 1. Process the file and retrieve data
                var processedData = ProcessFileAndGetResult(req);

                // 2. Notify clients using SignalR
                await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "SendNotification",
                        Arguments = new[] { "Processing is complete!" }
                    });

                // 3. Return data in the response
                return new OkObjectResult(processedData);
            }
            catch (Exception ex)
            {
                // Log any unexpected exceptions
                log.LogError($"An error occurred: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        private static string ProcessFileAndGetResult(HttpRequest req)
        {
            // Get the uploaded file from the request
            var file = req.Form.Files["file"];

            // Perform your file processing logic here
            // Example: Deserialize the JSON content from the file
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var jsonContent = reader.ReadToEnd();
                // Assuming the JSON content represents a string, adjust the deserialization based on your actual data structure
                var processedData = JsonConvert.DeserializeObject<string>(jsonContent);

                // Perform any additional processing if needed

                return processedData;
            }
        }
    }
}
