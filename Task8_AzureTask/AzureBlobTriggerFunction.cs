using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Task8_AzureTask.Model;

namespace Task8_AzureTask
{
    public static class AzureBlobTriggerFunction
    {
        [FunctionName("AzureBlobTriggerFunction")]
        public static async Task Run(
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
            Users user = JsonConvert.DeserializeObject<Users>(jsonContent);

            // Save the User object to the database
            await SaveUserToDatabase(user, log);

       
        }

        private static async Task SaveUserToDatabase(Users user, ILogger log)
        {
            try
            {
                string secretName = Environment.GetEnvironmentVariable("Secretname");
                string kvUri = Environment.GetEnvironmentVariable("KvUri") ;

                var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
                var secret = await client.GetSecretAsync(secretName);
  
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

