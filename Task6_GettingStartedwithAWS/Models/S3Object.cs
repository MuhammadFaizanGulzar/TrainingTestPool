using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task6_GettingStartedwithAWS.Models;

public class S3Object
{
    public string Name { get; set; } = null!;

    public MemoryStream InputStream { get; set; } = null!;

    public string BucketName { get; set; } = null!;
}

