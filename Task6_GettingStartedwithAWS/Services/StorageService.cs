using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task6_GettingStartedwithAWS.Models;

namespace Task6_GettingStartedwithAWS.Services;

public class StorageService : IStorageService
{
    public async Task<S3ResponseDto> UploadFileAsync(S3Object s3Object, AwsCredentials awsCredentials)
    {
        //Adding AwsCredentials
        var credentials = new BasicAWSCredentials(awsCredentials.AwsKey, awsCredentials.AwsSecretKey);

        //specify the region
        var config = new AmazonS3Config()
        {
            RegionEndpoint = Amazon.RegionEndpoint.EUNorth1
        };

        var response = new S3ResponseDto();

        try
        {
            //create the upload request
            var uploadRequest = new TransferUtilityUploadRequest()
            {
                InputStream = s3Object.InputStream,
                Key = s3Object.Name,
                BucketName = s3Object.BucketName,
                CannedACL = S3CannedACL.NoACL
            };

            //create an S3 Client
            using var client = new AmazonS3Client(credentials, config);

            //upload utility to s3
            var transferutility = new TransferUtility(client);

            // Actually uploading file to s3
            await transferutility.UploadAsync(uploadRequest);

            response.StatusCode = 200;
            response.Message = $"{s3Object.Name} has been uploaded successfully";
        }
        catch(AmazonS3Exception ex)
        {

            response.StatusCode = (int)ex.StatusCode;
            response.Message = ex.Message;
        }
        catch(Exception ex)
        {
            response.StatusCode = 500;
            response.Message = ex.Message;
        }

        return response;
    }
}

