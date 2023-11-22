using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using Amazon.Lambda;
using Amazon.Lambda.SQSEvents;

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
        foreach (var record in s3Event.Records)
        {
            if (record.S3 != null && record.S3.Bucket != null)
            {
                var bucketName = record.S3.Bucket.Name;
                var key = record.S3.Object.Key;
            
                await TriggerLambdaFunctionAsync(bucketName, key);
 
                //context.Logger.LogInformation("Lambda function triggered Successfully");
                //await SendToSqsQueue(bucketName, key);

                //context.Logger.LogInformation("SQS Triggered");

                var processedData = ProcessFile(bucketName, key);
                context.Logger.LogInformation(processedData);

                //context.Logger.LogInformation($"Processing file: {key} in bucket: {bucketName}");
                await PublishToSns(processedData);

                //context.Logger.LogInformation("Published message to SNS");
            }
            else
            {
                // Log a message or handle the case where S3 or Bucket is null
                context.Logger.LogError("S3 or Bucket is null in the S3Event record.");
            }
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