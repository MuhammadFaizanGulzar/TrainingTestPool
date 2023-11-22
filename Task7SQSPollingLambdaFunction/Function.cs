using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.SQS;



// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Task7SQSPollingLambdaFunction;

public class Function
{
    private readonly IAmazonSQS _sqsClient;
    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {
        _sqsClient = new AmazonSQSClient();
    }


    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
    /// to respond to SQS messages.
    /// </summary>
    /// <param name="evnt"></param>
    /// <param name="context"></param>
    /// <returns></returns>
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
}