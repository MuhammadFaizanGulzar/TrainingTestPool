using System;
using System.IO;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Task8_AzureTask.Entity;

namespace Task8_AzureTask
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run(
            [BlobTrigger("task8container/{name}.json", Connection = "AzureWebJobsStorage")] Stream myBlob,
            string name,
            ILogger log)
        {
            log.LogInformation($"C# Blob trigger function processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            // Read the content of the JSON file
            string jsonContent;
            using (StreamReader reader = new StreamReader(myBlob))
            {
                jsonContent = reader.ReadToEnd();
            }

            // Deserialize the JSON content into a User object using JsonConvert
            Entity.User user = JsonConvert.DeserializeObject<Entity.User>(jsonContent);

            // Save the User object to the database
            SaveUserToDatabase(user, log);

            // Continue with any additional logic or processing you need to do.
        }

        private static async Task SaveUserToDatabase(Entity.User user, ILogger log)
        {
            try
            {
                string secretName = Environment.GetEnvironmentVariable("Secretname");
                string kvUri = Environment.GetEnvironmentVariable("KvUri") ;

                var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
                var secret = await client.GetSecretAsync(secretName);


                // You would replace this with your actual DbContext initialization logic
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer(secret.Value.Value)
                    .Options;

                using (var dbContext = new ApplicationDbContext(options))
                {
                    // Save the User to the database using Entity Framework Core
                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error saving User to database: {ex.Message}");
                throw;
            }
        }
    }


}

