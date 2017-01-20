using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Cuiliang.AliyunOssSdk.Api.Common.Consts;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Utility;

namespace Cuiliang.AliyunOssSdk.Request
{
    /// <summary>
    /// 预先签名URL生成器
    /// </summary>
    public class PresignedUrlCreator
    {
        //默认的超时时间
        const int DEFAULT_EXPIRE_SECONDS = 600;

        public BucketInfo Bucket { get; set; }
        public string ObjectKey { get; set; }

        public DateTime ExpireTime { get; set; } = DateTime.UtcNow.AddSeconds(DEFAULT_EXPIRE_SECONDS);

        public string ContentType { get; set; }
        public string ContentMd5 { get; set; }
        public HttpMethod HttpMethod { get; set; } = HttpMethod.Get;

        public PresignedUrlCreator()
        {
            
        }

        public PresignedUrlCreator(BucketInfo bucket, string objectKey, int expireSeconds = DEFAULT_EXPIRE_SECONDS)
        {
            Bucket = bucket;
            ObjectKey = objectKey;
            ExpireTime = DateTime.UtcNow.AddSeconds(expireSeconds);
        }

        public ResponseHeaderOverrides ResponseHeaderOverrides { get; set; }

        /// <summary>
        /// 用户自定义请求头 //TODO: 此处如果设置了UserMetadata，不光在url里，还需要在http headers里添加。所以如果是这种情况，还不如直接用户在header里签名好了。
        /// </summary>
        public Dictionary<string,string> UserMetadata { get; set; } 

        public Uri Create(RequestContext requestContext)
        {
            Ensure.ToBeTrue(ExpireTime > DateTime.UtcNow);
            Ensure.ToBeTrue(HttpMethod == HttpMethod.Get || HttpMethod == HttpMethod.Put, "不支持的http method"); //只支持这两种

            var method = HttpMethod.ToString().ToUpperInvariant();
            var expire = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString(); //UNIX 时间戳
            
            
            
            // 要额外签名的数据
            var headers = new Dictionary<string,string>();
            
            if (UserMetadata != null)
            {
                foreach (var pair in UserMetadata)
                {
                    headers.Add(OssHeaders.OssUserMetaPrefix + pair.Key, pair.Value);
                }
            }
            var canonicalHeaders = SignatureHelper.ComputeCanonicalizedOSSHeaders(headers); //? 以STS获得的AccessKeyId和AccessKeySecret发送时，是否考虑SecurityToken的处理？



            //
            var parameters = new Dictionary<string, string>();
            if (requestContext.OssCredential.UseToken)
            {
                parameters.Add(RequestParameters.SECURITY_TOKEN, requestContext.OssCredential.SecurityToken);
            }

            ResponseHeaderOverrides?.Populate(parameters);

            var canonicalResource = SignatureHelper.BuildCanonicalizedResource(Bucket, ObjectKey, parameters);

            //var canonicalResource = "/" + (Bucket ?? "") + ((ObjectKey != null ? "/" + ObjectKey : ""));

            //签名
            var sign = SignatureHelper.HmacSha1Sign(requestContext.OssCredential.AccessKeySecret, method, ContentMd5, ContentType, expire, canonicalHeaders,
                canonicalResource);

            // URL 参数
            var urlParams = new Dictionary<string,string>();
            urlParams.Add(RequestParameters.EXPIRES, expire);
            urlParams.Add(RequestParameters.OSS_ACCESS_KEY_ID, requestContext.OssCredential.AccessKeyId);
            urlParams.Add(RequestParameters.SIGNATURE, sign);
            foreach (var pair in parameters)
            {
                urlParams.Add(pair.Key, pair.Value);
            }
            var queryString = HttpUtils.CombineQueryString(urlParams);

            // 生成最终的URI
            var uriString = Bucket.GetObjectUrl(ObjectKey) + "?" + queryString;

            return new Uri(uriString);

        }
    }
}
