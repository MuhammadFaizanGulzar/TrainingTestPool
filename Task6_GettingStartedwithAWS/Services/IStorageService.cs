using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task6_GettingStartedwithAWS.Models;

namespace Task6_GettingStartedwithAWS.Services
{
    public interface IStorageService
    {
        Task<S3ResponseDto> UploadFileAsync(S3Object s3Object, AwsCredentials awsCredentials);
    }
}
