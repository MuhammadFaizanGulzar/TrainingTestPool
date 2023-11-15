using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Task6_AWS.Helper;
using Task6_GettingStartedwithAWS.Models;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Task6_AWS
{
    public class Function
    {
        private IAmazonS3 S3Client { get; set; }

        public Function()
        {
            S3Client = new AmazonS3Client();

        }

        public async Task FunctionHandlerTrigger(S3Event evnt, ILambdaContext context)
        {
            var eventRecords = evnt.Records ?? new List<S3Event.S3EventNotificationRecord>();

            foreach (var record in eventRecords)
            {
                var s3Event = record.S3;
                if (s3Event == null)
                {
                    continue;
                }

                try
                {

                    var file = await S3Client.GetObjectAsync(s3Event.Bucket.Name, s3Event.Object.Key);
                    // Check if file already exists
                    if (file.Key.StartsWith("processed_"))
                    {
                        context.Logger.LogInformation($"S3 object {s3Event.Object.Key} has already been processed. Skipping.");
                        continue;
                    }

             
                    var response = await S3Client.GetObjectMetadataAsync(s3Event.Bucket.Name, s3Event.Object.Key);
                    context.Logger.LogInformation($"Hello, bucket: {s3Event.Bucket.Name} and filename: {s3Event.Object.Key}");
                    context.Logger.LogInformation(response.Headers.ContentType);

                    using var reader = new StreamReader(file.ResponseStream);
                    var fileContents = await reader.ReadToEndAsync();
                    context.Logger.LogInformation(fileContents);

                    var user = JsonConvert.DeserializeObject<User>(fileContents);
                    context.Logger.LogInformation($"User Name: {user.Name}");
                    context.Logger.LogInformation($"Email: {user.Email}");
                    context.Logger.LogInformation($"Company: {user.Role}");

                    if (user != null)
                    {
                        var connectionString = await SecretsManagerHelper.GetSecret();                     
                   
                        context.Logger.LogInformation(connectionString);

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            await connection.OpenAsync();
                            string insertQuery = "INSERT INTO Users(Name, Email, Role) VALUES (@Name, @Email, @Role)";

                            using (SqlCommand command = new SqlCommand(insertQuery, connection))
                            {
                                command.Parameters.AddWithValue("@Name", user.Name);
                                command.Parameters.AddWithValue("@Email", user.Email);
                                command.Parameters.AddWithValue("@Role", user.Role);

                                await command.ExecuteNonQueryAsync();
                            }
                        }

                   
                        user.Role = "User";
                        string modifiedJson = JsonConvert.SerializeObject(user);

                        // Update the S3 object
                        var putObjectReq = new PutObjectRequest
                        {
                            BucketName = s3Event.Bucket.Name,
                            Key = MarkS3ObjectAsProcessed(s3Event.Object.Key),
                            ContentBody = modifiedJson
                        };
                        await S3Client.PutObjectAsync(putObjectReq);
                    }
                    context.Logger.LogInformation(fileContents);
                }
                catch (Exception e)
                {
                    context.Logger.LogError($"Error processing S3 object {s3Event.Object.Key}. {e.Message}");
                    context.Logger.LogError(e.StackTrace);
                    throw;
                }
            }
        }


        private string MarkS3ObjectAsProcessed(string objectKey)
        {

            return "processed_" + objectKey;

        }


    }
}