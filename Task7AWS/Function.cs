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
                await SendToSqsQueue(bucketName, key);

    
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



        // After processing S3 events, poll the SQS queue
        //await PollSqsQueue();
    }

    public async Task PollSqsQueueFunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        string queueUrl = "https://sqs.eu-north-1.amazonaws.com/090433784189/task7-sqs-queue";
        // ... (code for polling SQS and processing messages)
        try
        {
            foreach (var record in sqsEvent.Records)
            {
                var messageBody = record.Body; // Get the message body

                // Process the SQS message (replace this with your actual processing logic)
                var processedData = ProcessSqsMessage(messageBody, context);
                context.Logger.LogInformation($"Processed SQS message: {processedData}");

                // Delete the message from the queue
                await _sqsClient.DeleteMessageAsync(queueUrl, record.ReceiptHandle);
            }
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error processing SQS messages: {ex.Message}");
        }
    }

    private string ProcessSqsMessage(string messageBody, ILambdaContext context)
    {
        // Replace this with your actual processing logic for SQS messages
        // For example, you can log or store the file name and data
        context.Logger.LogInformation($"Processed message: {messageBody}");

        // Perform additional processing as needed

        return "Processing completed"; // Adjust the return value as needed
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

    private async Task PollSqsQueue()
    {
        try
        {
            // Specify the URL of your SQS queue
            string queueUrl = "https://sqs.eu-north-1.amazonaws.com/090433784189/task7-sqs-queue";

            var request = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 1,
                WaitTimeSeconds = 10 // Adjust as needed
            };

            var response = await _sqsClient.ReceiveMessageAsync(request);

            if (response != null && response.Messages != null && response.Messages.Any())
            {
                var message = response.Messages[0].Body;
                // Process the message (your processed data)

                // Delete the message from the queue
                await _sqsClient.DeleteMessageAsync(queueUrl, response.Messages[0].ReceiptHandle);
            }
            else
            {
                Console.WriteLine("No messages received from the SQS queue.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error polling SQS queue: {ex.Message}");
        }
    }
}