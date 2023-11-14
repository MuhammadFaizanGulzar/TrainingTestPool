using Microsoft.AspNetCore.Mvc;
using Task6_GettingStartedwithAWS.Models;
using Task6_GettingStartedwithAWS.Services;

namespace S3FileUpload.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _config;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IStorageService storageService, IConfiguration config)
        {
            _logger = logger;
            _storageService = storageService;
            _config = config;

        }


        [HttpPost(Name = "UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            //Process file
            await using var memoryStr = new MemoryStream();
            await file.CopyToAsync(memoryStr);

            var fileExt = Path.GetExtension(file.Name);
            var objName = $"{Guid.NewGuid()}.{fileExt}";

            var s3Object = new S3Object()
            {
                BucketName = "demo-bucket-training-testpool",
                InputStream = memoryStr,
                Name = objName

            };
            var Cred = new AwsCredentials()
            {
                AwsKey = _config["AwsConfiguration:AWSAccesskey"],
                AwsSecretKey = _config["AwsConfiguration:AWSSecretKey"]

            };

            var result = await _storageService.UploadFileAsync(s3Object, Cred);

            return Ok(result);
        }

    }
}