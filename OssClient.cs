using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cuiliang.AliyunOssSdk.Api;
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

namespace Cuiliang.AliyunOssSdk
{
    /// <summary>
    /// 封装并简化调用接口
    /// </summary>
    public class OssClient
    {
        private RequestContext _requestContext = null;

        public OssClient(OssCredential credential)
            :this(credential, ClientConfiguration.Default)
        {
            
        }

        public OssClient(OssCredential credential, ClientConfiguration config)
        {
            _requestContext = new RequestContext(credential, config);
        }

        /// <summary>
        /// 列出所有的Bucket
        /// </summary>
        /// <returns></returns>
        public async Task<OssResult<ListBucketsResult>> ListBucketsAsync(string region)
        {
            
            var cmd = new ListBucketCommand(_requestContext, region, new ListBucketsRequest());
            return await cmd.ExecuteAsync();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<OssResult<PutObjectResult>> PutObjectAsync(BucketInfo bucket, string key, RequestContent file)
        {
            var cmd = new PutObjectCommand(_requestContext, bucket, key, file, null);

            return await cmd.ExecuteAsync();
        }

        /// <summary>
        /// 将指定的字符串内容上传为文件
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public async Task<OssResult<PutObjectResult>> PutObjectAsync(BucketInfo bucket, string key, string content, string mimeType = "text/plain")
        {
            var file = new RequestContent()
            {
                ContentType = RequestContentType.String,
                StringContent = content,
                MimeType = mimeType
            };

            return await PutObjectAsync(bucket, key, file);
        }

        /// <summary>
        /// 根据指定的文件名上传文件
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <param name="filePathName"></param>
        /// <returns></returns>
        public async Task<OssResult<PutObjectResult>> PutObjectByFileNameAsync(BucketInfo bucket, string key,
            string filePathName)
        {
            using (var stream = File.OpenRead(filePathName))
            {
                var file = new RequestContent()
                {
                    ContentType = RequestContentType.Stream,
                    StreamContent = stream,
                    MimeType = MimeHelper.GetMime(filePathName)
                };

                return await PutObjectAsync(bucket, key, file);
            }
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

            return await cmd.ExecuteAsync();
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

            return await cmd.ExecuteAsync();
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

            return await cmd.ExecuteAsync();
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

            return await cmd.ExecuteAsync();
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

            return await cmd.ExecuteAsync();
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
            return await cmd.ExecuteAsync();
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
            return await cmd.ExecuteAsync();
        }
    }
}
