using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Cuiliang.AliyunOssSdk.Api;
using Cuiliang.AliyunOssSdk.Api.Bucket.Get;
using Cuiliang.AliyunOssSdk.Api.Bucket.List;
using Cuiliang.AliyunOssSdk.Api.Object.Append;
using Cuiliang.AliyunOssSdk.Api.Object.Copy;
using Cuiliang.AliyunOssSdk.Api.Object.Delete;
using Cuiliang.AliyunOssSdk.Api.Object.DeleteMultiple;
using Cuiliang.AliyunOssSdk.Api.Object.Get;
using Cuiliang.AliyunOssSdk.Api.Object.GetMeta;
using Cuiliang.AliyunOssSdk.Api.Object.Head;
using Cuiliang.AliyunOssSdk.Api.Object.Put;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;
using Cuiliang.AliyunOssSdk.Utility;
using Cuiliang.AliyunOssSdk.Utility.Authentication;
using Microsoft.Extensions.Logging;

namespace Cuiliang.AliyunOssSdk
{
    /// <summary>
    /// 封装并简化调用接口
    /// </summary>
    public class OssClient
    {
        private readonly HttpClient _client;
        private readonly RequestContext _requestContext;
        private readonly ILogger _logger;

        public OssClient(HttpClient client, RequestContext requestContext, ILoggerFactory loggerFactory)
        {
            _client = client;
            _requestContext = requestContext;
            _logger = loggerFactory.CreateLogger<OssClient>();
        }

        /// <summary>
        /// 列出所有的Bucket
        /// </summary>
        /// <returns></returns>
        public async Task<OssResult<ListBucketsResult>> ListBucketsAsync(string region)
        {
            var cmd = new ListBucketsCommand(_requestContext, region, new ListBucketsRequest());
            return await cmd.ExecuteAsync(_client);
        }

        /// <summary>
        /// 列出bucket下的object
        /// </summary>
        /// <param name="bucketInfo"></param>
        /// <param name="prefix"></param>
        /// <param name="marker"></param>
        /// <param name="maxKeys"></param>
        /// <param name="delimiter"></param>
        /// <param name="encodingType"></param>
        /// <returns></returns>
        public async Task<OssResult<GetBucketResult>> GetBucketAsync(BucketInfo bucketInfo,
            string prefix,
            string marker,
            int maxKeys = 100,
            string delimiter = "",
            string encodingType = "url")
        {
            var cmd = new GetBucketCommand(_requestContext, bucketInfo, prefix, marker, maxKeys, delimiter, encodingType);

            var result =  await cmd.ExecuteAsync(_client);

            if (!result.IsSuccess)
            {
                _logger.LogError($"Failed in OssClient.{nameof(GetBucketAsync)}(). \nBucket: {bucketInfo.BucketName}\n");
            }

            return result;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<OssResult<PutObjectResult>> PutObjectAsync(BucketInfo bucket, string key, RequestContent file, IDictionary<string, string> extraHeaders = null)
        {
            var cmd = new PutObjectCommand(_requestContext, bucket, key, file, extraHeaders);

            var result = await cmd.ExecuteAsync(_client);

            if(!result.IsSuccess)
            {
                _logger.LogError($"Failed in OssClient.{nameof(PutObjectAsync)}(). \nBucket: {bucket.BucketName}\nPath: {key}");
            }

            return result;
        }

        /// <summary>
        /// 将指定的字符串内容上传为文件
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public async Task<OssResult<PutObjectResult>> PutObjectAsync(BucketInfo bucket, string key, string content, string mimeType = "text/plain", ObjectMetadata meta = null, IDictionary<string, string> extraHeaders = null)
        {
            var file = new RequestContent()
            {
                ContentType = RequestContentType.String,
                StringContent = content,
                MimeType = mimeType,
                Metadata = meta
            };

            return await PutObjectAsync(bucket, key, file, extraHeaders);
        }

        /// <summary>
        /// 根据指定的文件名上传文件
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <param name="filePathName"></param>
        /// <returns></returns>
        public async Task<OssResult<PutObjectResult>> PutObjectByFileNameAsync(BucketInfo bucket, string key,
            string filePathName, ObjectMetadata meta = null, IDictionary<string, string> extraHeaders = null)
        {
            using (var stream = File.OpenRead(filePathName))
            {
                var file = new RequestContent()
                {
                    ContentType = RequestContentType.Stream,
                    StreamContent = stream,
                    MimeType = MimeHelper.GetMime(filePathName),
                    Metadata = meta
                };

                return await PutObjectAsync(bucket, key, file, extraHeaders);
            }
        }

        /// <summary>
        /// 上传流
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <param name="content">内容流</param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public async Task<OssResult<PutObjectResult>> PutObjectAsync(BucketInfo bucket, string key, Stream content,
            string mimeType = "application/octet-stream", ObjectMetadata meta = null, IDictionary<string, string> extraHeaders = null)
        {
            var file = new RequestContent()
            {
                ContentType = RequestContentType.Stream,
                StreamContent = content,
                MimeType = mimeType,
                Metadata = meta
            };

            return await PutObjectAsync(bucket, key, file, extraHeaders);
        }


        /// <summary>
        /// 复制对象
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="srcKey"></param>
        /// <param name="targetBucket"></param>
        /// <param name="targetKey"></param>
        /// <param name="extraHeaders"></param>
        /// <returns></returns>
        public async Task<OssResult<CopyObjectResult>> CopyObjectAsync(BucketInfo bucket, string srcKey,
            BucketInfo targetBucket,
            string targetKey,
            IDictionary<string, string> extraHeaders = null)
        {
            var cmd = new CopyObjectCommand(_requestContext, targetBucket, targetKey, bucket, srcKey, extraHeaders);

            return await cmd.ExecuteAsync(_client);
        }

        /// <summary>
        /// 下载对象
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<OssResult<GetObjectResult>> GetObjectAsync(BucketInfo bucket, string key, GetObjectParams parameters = null)
        {
            var cmd = new GetObjectCommand(_requestContext, bucket, key, parameters);

            return await cmd.ExecuteAsync(_client);
        }

        /// <summary>
        /// Append对象
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <param name="nextAppendPosition"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<OssResult<AppentObjectResult>> AppendObject(BucketInfo bucket, string key,
            long nextAppendPosition, RequestContent file)
        {
            var cmd = new AppendObjectCommand(_requestContext, bucket, key, nextAppendPosition, file);

            return await cmd.ExecuteAsync(_client);
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<OssResult<DeleteObjectResult>> DeleteObjectAsync(BucketInfo bucket, string key)
        {
            var cmd = new DeleteObjectCommand(_requestContext, bucket, key);

            var result  = await cmd.ExecuteAsync(_client);

            if (!result.IsSuccess)
            {
                _logger.LogError($"Failed in OssClient.{nameof(PutObjectAsync)}(). \nBucket: {bucket.BucketName}\nPath: {key}");
            }

            return result;

        }

        /// <summary>
        /// 删除多个对象
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="keys"></param>
        /// <param name="quiet"></param>
        /// <returns></returns>
        public async Task<OssResult<DeleteMultipleObjectsResult>> DeleteMultipleObjectsAsync(BucketInfo bucket,
            IList<string> keys, bool quiet = false)
        {
            var cmd = new DeleteMultipleObjectsCommand(_requestContext, bucket, keys, quiet);

            return await cmd.ExecuteAsync(_client);
        }

        /// <summary>
        /// ead Object只返回某个Object的meta信息，不返回文件内容。
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<OssResult<HeadObjectResult>> HeadObjectAsync(BucketInfo bucket, string key, HeadObjectParams parameters)
        {
            var cmd = new HeadObjectCommand(_requestContext, bucket, key, parameters);
            return await cmd.ExecuteAsync(_client);
        }

        /// <summary>
        /// Get Object Meta用来获取某个Bucket下的某个Object的基本meta信息，包括该Object的ETag、Size（文件大小）、LastModified，并不返回其内容。
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<OssResult<GetObjectMetaResult>> GetObjectMetaAsync(BucketInfo bucket, string key)
        {
            var cmd = new GetObjectMetaCommand(_requestContext, bucket, key);
            return await cmd.ExecuteAsync(_client);
        }

        /// <summary>
        /// 获取文件的下载链接
        /// </summary>
        /// <param name="bucket">bucket信息</param>
        /// <param name="storeKey">文件存储key</param>
        /// <param name="expireSeconds">签名超时时间秒数</param>
        /// <param name="imgStyle">阿里云图片处理样式</param>
        /// <returns></returns>
        public string GetFileDownloadLink(BucketInfo bucket, string storeKey, int expireSeconds, string imgStyle = null)
        {
            long seconds = (DateTime.UtcNow.AddSeconds(expireSeconds).Ticks - 621355968000000000) / 10000000;

            string toSign = String.Format("GET\n\n\n{0}\n/{1}/{2}", seconds, bucket.BucketName, storeKey);
            if (!String.IsNullOrEmpty(imgStyle))
            {
                toSign += $"?x-oss-process=style/{imgStyle}";
            }

            string sign = ServiceSignature.Create().ComputeSignature(
                _requestContext.OssCredential.AccessKeySecret, toSign);

            string styleSegment = String.IsNullOrEmpty(imgStyle) ? String.Empty : $"x-oss-process=style/{imgStyle}&";
            string url = $"{bucket.BucketUri}{storeKey}?{styleSegment}OSSAccessKeyId={_requestContext.OssCredential.AccessKeyId}&Expires={seconds}&Signature={WebUtility.UrlEncode(sign)}";

            return url;
        }

        /// <summary>
        /// 生成直接post到oss的签名
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        public string ComputePostSignature(string policy)
        {
            string sign = ServiceSignature.Create().ComputeSignature(
                _requestContext.OssCredential.AccessKeySecret, policy);

            return sign;
        }
    }
}
