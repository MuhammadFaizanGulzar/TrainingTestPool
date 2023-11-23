using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using Amazon.Lambda;
using Microsoft.Data.SqlClient;
using Task7AWS.Models;


[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Task7AWS;
public class Function
{
    private readonly IAmazonS3 _s3Client;
    private readonly IAmazonSimpleNotificationService _snsClient;
    private readonly IAmazonSQS _sqsClient;


    public Function()
    {
        _s3Client = new AmazonS3Client();
        _snsClient = new AmazonSimpleNotificationServiceClient();
        _sqsClient = new AmazonSQSClient();
    }

    public async Task FunctionHandler(S3Event s3Event, ILambdaContext context)
    {
        var eventRecords = s3Event.Records ?? new List<S3Event.S3EventNotificationRecord>();

        foreach (var record in eventRecords)
        {
            var s3EventRecord = record.S3;
            if (s3EventRecord == null)
            {
                continue;
            }

            try
            {
                var file = await _s3Client.GetObjectAsync(s3EventRecord.Bucket.Name, s3EventRecord.Object.Key);
                var response = await _s3Client.GetObjectMetadataAsync(s3EventRecord.Bucket.Name, s3EventRecord.Object.Key);
                context.Logger.LogInformation($"Hello, bucket: {s3EventRecord.Bucket.Name} and filename: {s3EventRecord.Object.Key}");
                context.Logger.LogInformation(response.Headers.ContentType);

                using var reader = new StreamReader(file.ResponseStream);
                var fileContents = await reader.ReadToEndAsync();
                context.Logger.LogInformation(fileContents);

                

                var processedData = ProcessFile(s3EventRecord.Bucket.Name, s3EventRecord.Object.Key);



                // Connect to SQL Server and store data
                await StoreDataInSqlServer(fileContents, context);

                await SendToSqsQueue(s3EventRecord.Bucket.Name, s3EventRecord.Object.Key);

                await PublishToSns(processedData);

                context.Logger.LogInformation("Processing completed");
            }
            catch (Exception e)
            {
                context.Logger.LogError($"Error processing S3 object {s3EventRecord.Object.Key}. {e.Message}");
                context.Logger.LogError(e.StackTrace);
                throw;
            }
        }
    }
    private async Task StoreDataInSqlServer(string fileContents, ILambdaContext context)
    {
        try
        {
            //var connectionString = await SecretsManagerHelper.GetSecret();
            var connectionString = "Server=database-1.ccyxixvgakuw.eu-north-1.rds.amazonaws.com;Database=Task6AWS;User ID=admin;Password=Password1;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string insertQuery = "INSERT INTO Users(Name, Email, Role) VALUES (@Name, @Email, @Role)";
                context.Logger.LogInformation($"File Contents: {fileContents}");
                var user = JsonConvert.DeserializeObject<Users>(fileContents);

                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Role", user.Role);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error storing data in SQL Server: {ex.Message}");
        }
    }
    private async Task TriggerLambdaFunctionAsync(string bucketName, string key)
    {
        try
        {
            string lambdaFunctionArn = "arn:aws:lambda:eu-north-1:090433784189:function:task7FileUploadTrigger";
            var lambdaClient = new Amazon.Lambda.AmazonLambdaClient();

            var request = new Amazon.Lambda.Model.InvokeRequest
            {
                FunctionName = lambdaFunctionArn,
                InvocationType = InvocationType.Event,
                Payload = $"{{\"bucket\": \"{bucketName}\", \"key\": \"{key}\"}}"
            };

            await lambdaClient.InvokeAsync(request);
        }
        catch (Exception ex)
        {
            //_logger.Log($"Error triggering Lambda function: {ex.Message}");
            Console.WriteLine($"Error triggering Lambda function: {ex.Message}");
        }
    }



    private string ProcessFile(string bucket, string key)
    {
        //_logger.Log($"Processing file: {key} in bucket: {bucket}");
        return JsonConvert.SerializeObject(new { status = "success", message = "File processed successfully" });
    }

    private async Task PublishToSns(string message)
    {
        try
        {
            string snsTopicArn = "arn:aws:sns:eu-north-1:090433784189:task7Sns";

            var request = new PublishRequest
            {
                TopicArn = snsTopicArn,
                Message = message
            };

            var response = await _snsClient.PublishAsync(request);
            //_logger.Log($"Published message to SNS. MessageId: {response.MessageId}");
        }
        catch (Exception ex)
        {
            //_logger.Log($"Error publishing message to SNS: {ex.Message}");
            Console.WriteLine($"Error publishing message to SNS: {ex.Message}");
        }
    }

    private async Task SendToSqsQueue(string bucket, string key)
    {
        try
        {
            string queueUrl = "https://sqs.eu-north-1.amazonaws.com/090433784189/task7-sqs-queue";

            var request = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = $"{{\"bucket\": \"{bucket}\", \"key\": \"{key}\"}}"
            };

            await _sqsClient.SendMessageAsync(request);
        }
        catch (Exception ex)
        {
            //_logger.Log($"Error sending message to SQS: {ex.Message}");
            Console.WriteLine($"Error sending message to SQS: {ex.Message}");
        }
    }

    
}