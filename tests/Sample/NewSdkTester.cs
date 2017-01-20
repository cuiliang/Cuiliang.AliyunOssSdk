using Cuiliang.AliyunOssSdk;
using Cuiliang.AliyunOssSdk.Entites;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    class NewSdkTester
    {
        public static string AccessKeyId = "";  // 设置您的AccessKeyId

        internal static string AssessSecret = ""; //设置您的AssessSecret

        internal static string EndPoint = "oss-cn-shanghai.aliyuncs.com";  //设置要操作的区域


        internal static string BucketName = ""; //设置要操作的BucketName


        public static async Task RunAsync()
        {



            var crediential = new OssCredential()
            {
                AccessKeyId = AccessKeyId,
                AccessKeySecret = AssessSecret
            };

            var client = new OssClient(crediential);

            ////list buckets
            //var listBucketResult = await client.ListBucketsAsync(OssRegions.ShangHai);
            //Console.WriteLine(listBucketResult.IsSuccess + ":" + listBucketResult.ErrorMessage);

            var bucket = BucketInfo.CreateByRegion(EndPoint, BucketName, false, false);

            // put string
            {
                string content = "这是一个文本文件\naaaaaaaa\nbbbbbb\nccccccccc";
                var putResult = await client.PutObjectAsync(bucket, "test_put_object_string.txt", content);
                Console.WriteLine($"Put string object  {putResult.IsSuccess} {putResult.ErrorMessage}  Etag:{putResult.SuccessResult?.ETag}");
            }

        

            //// put file
            //var file = @"D:\Work\Weixin\resource\IMG_1399.png";
            //putResult = await client.PutObjectByFileNameAsync(bucket, "test_put_file.png", file);
            //Console.WriteLine($"Put file object  {putResult.IsSuccess} {putResult.ErrorMessage}  Etag:{putResult.SuccessResult?.ETag}");


            //// copy file
            //Console.WriteLine("\n\n===Copy Object=============");
            //var copyResult =
            //    await
            //        client.CopyObjectAsync(bucket, "test_put_object_string.txt", bucket,
            //            "test_03_copy_object_string.txt", null);
            //Console.WriteLine($"Copy object  {copyResult.IsSuccess} {copyResult.ErrorMessage}  Etag:{copyResult.SuccessResult?.LastModified}");

            //// get file
            //Console.WriteLine("\n\n===Get Object=============");
            //var getResult = await client.GetObjectAsync(bucket, "test_put_object_string.txt");
            //Console.WriteLine($"Get Object = {getResult.IsSuccess}");
            //if (getResult.IsSuccess)
            //{
            //    var content = await getResult.SuccessResult.Content.ReadAsStringAsync();
            //    Console.WriteLine("FileContent" + content);
            //}

            //// append file
            //Console.WriteLine("\n\n===append Object=============");
            //{
            //    var content = "This is a line 这是一行字符串.";
            //    var file = new OssObjectInfo()
            //    {
            //        ContentType = RequestContentType.String,
            //        MimeType = "text/text",
            //        StringContent = content
            //    };
            //    var appendResult = await client.AppendObject(bucket, "test_append_object.txt", 0, file);
            //    Console.WriteLine($"1st append:{appendResult.IsSuccess} nextPos={appendResult.SuccessResult?.NextAppendPosition}");
            //    if (appendResult.IsSuccess)
            //    {
            //        appendResult = await client.AppendObject(bucket, "test_append_object.txt", appendResult.SuccessResult.NextAppendPosition, file);
            //        Console.WriteLine($"2st append:{appendResult.IsSuccess} nextPos={appendResult.SuccessResult?.NextAppendPosition}");

            //    }
            //}

            //// delete object
            //Console.WriteLine("\n\n===append Object=============");
            //{
            //    var content = "This is a line 这是一行字符串.";
            //    var key = "test_delete_object.txt";
            //    var putResult = await client.PutObjectAsync(bucket, key, content);
            //    if (putResult.IsSuccess)
            //    {
            //        var deleteResult = await client.DeleteObjectAsync(bucket, key);
            //        Console.WriteLine($"Delete reuslt:{deleteResult.IsSuccess} {deleteResult.ErrorMessage}");
            //    }
            //    else
            //    {
            //        Console.WriteLine($" Put object failed.{putResult.ErrorMessage}");
            //    }
            //}

            //// delete multiple
            //Console.WriteLine("\n\n===delete multiple=============");
            //{
            //    var content = "This is a line 这是一行字符串.";

            //    IList<string> fielKeys = new List<string>();

            //    for (int i = 1; i < 10; i++)
            //    {
            //        var key = $"test_delete_multi_object_{i}.txt";
            //        var putResult = await client.PutObjectAsync(bucket, key, content);
            //        if (putResult.IsSuccess == false)
            //        {
            //            Console.WriteLine($" Put object {i} failed.{putResult.ErrorMessage}");
            //            return;
            //        }
            //        fielKeys.Add(key);
            //    }

            //    var deleteResult = await client.DeleteMultipleObjectsAsync(bucket, fielKeys, false);

            //    Console.WriteLine($"Delete multi:{deleteResult.IsSuccess} {deleteResult.ErrorMessage}");

            //}


            // head object
            // delete multiple
            Console.WriteLine("\n\n===head object=============");
            {
                var content = "This is a line 这是一行字符串.";
                var key = "test_head_object.txt";
                //var putResult = await client.PutObjectAsync(bucket, key, content);
                //if (putResult.IsSuccess)
                {
                    var headResult = await client.HeadObjectAsync(bucket, key, null);
                    Console.WriteLine($"Head object: {headResult.IsSuccess} {headResult.ErrorMessage}");
                }

            }


            Console.WriteLine("\n\n===Get object meta=============");
            {
                //var content = "This is a line 这是一行字符串.";
                var key = "test_get_meta_object.txt";
                //var putResult = await client.PutObjectAsync(bucket, key, content);
                //if (putResult.IsSuccess)
                {
                    var headResult = await client.GetObjectMetaAsync(bucket, key);
                    Console.WriteLine($"Head object: {headResult.IsSuccess} {headResult.ErrorMessage}");
                }

            }

        }
    }
}
