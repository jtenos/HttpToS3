using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpToS3
{
    class Program
    {
        private static IConfiguration _config = default!;

        /// <summary>
        /// HttpToS3.exe http://example.com mybucketname folder/subfolder/myfile-{0:yyyyMMddHHmmss}.txt
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string accessKey = _config["AccessKey"];
            string secretKey = _config["SecretKey"];
            string region = _config["Region"];

            var client = new AmazonS3Client(new BasicAWSCredentials(accessKey, secretKey), 
                RegionEndpoint.GetBySystemName(region));

            var transferUtil = new TransferUtility(client);
            string url = args[0];
            string bucketName = args[1];
            string fileName = string.Format(args[2], DateTime.Now);

            var memoryStream = new MemoryStream(await new HttpClient().GetByteArrayAsync(url).ConfigureAwait(false));
            await transferUtil.UploadAsync(memoryStream, bucketName, fileName).ConfigureAwait(false);
        }
    }
}
