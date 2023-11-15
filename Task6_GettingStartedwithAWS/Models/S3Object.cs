using System;


namespace Task6_GettingStartedwithAWS.Models;

public class S3Object
{
    public string Name { get; set; } = null!;

    public MemoryStream InputStream { get; set; } = null!;

    public string BucketName { get; set; } = null!;
}

