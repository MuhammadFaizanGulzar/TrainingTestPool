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
            // Process the file...

            // Notify clients using SignalR
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "SendNotification",
                    Arguments = new[] { "Processing is complete!" }
                });

            return new OkResult();
        }
    }
}
