using System.Net;


namespace Task6_GettingStartedwithAWS.Models;

    public class S3ResponseDto
    {
    public int StatusCode { get; set; } = (int)HttpStatusCode.OK;
    public string Message { get; set; } = string.Empty;
    }

