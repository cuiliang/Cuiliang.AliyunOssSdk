using System;
using System.IO;
using System.Threading.Tasks;
using Cuiliang.AliyunOssSdk;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Please run with bucket name and file path");
            return;
        }

        var bucketName = args[0];
        var filePath = args[1];

        var conf = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("appsettings.Development.json", true, true)
                .Build();

        IServiceCollection services = new ServiceCollection();
        services.AddOssClient(conf);
        var sp = services.BuildServiceProvider();
        var client = sp.GetService<OssClient>();

        var bucket = BucketInfo.CreateByRegion(OssRegions.HangZhou, bucketName, false, false);
        var remoteFilePaht = $"test/{DateTime.Now.ToString("yyyyMMddHHmmss")}.png";
        using (var fs = new FileStream(filePath, FileMode.Open))
        {
            var file = new RequestContent()
            {
                MimeType = "images/png",
                ContentType = RequestContentType.Stream,
                StreamContent = fs,
            };

            var result = await client.PutObjectAsync(bucket, remoteFilePaht, file);
            if(result.IsSuccess)
            {
                Console.WriteLine("Succeeded!");                
            }
            else
            {
                Console.WriteLine("Failed!");
                Console.WriteLine(result.ErrorMessage);
            }
        }
    }
}