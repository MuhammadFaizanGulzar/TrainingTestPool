using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Reflection;


namespace Task6_AWS.Helper
{
    public class SecretsManagerHelper
    {
        private readonly IAmazonSecretsManager _secretsManager;
        private readonly IConfiguration _configuration;

        public SecretsManagerHelper(IAmazonSecretsManager secretsManager, IConfiguration configuration)
        {
            this._secretsManager = secretsManager;
            this._configuration = configuration;
        }

        public static async Task<string?> GetSecret()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(assemblyLocation))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

            string dbName = config["Database"];
            string secretName = config["SecretName"];
            string region = config["Region"];


            using var client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));
            var request = new GetSecretValueRequest()
            {
                SecretId = secretName
            };
            var response = await client.GetSecretValueAsync(request);
            var secretData = JsonConvert.DeserializeObject<Models.SecretCredentials>(response.SecretString);
            if (secretData != null)
            {
                string connectionString = $"Server={secretData.Host};Database={dbName};User ID={secretData.Username};Password={secretData.Password};";
                return connectionString;
            }
            return null;
        }

    }
}
