using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Utility;

namespace Cuiliang.AliyunOssSdk.Request
{
    /// <summary>
    /// 封装了要向服务器发送的一个请求
    /// </summary>
    public class ServiceRequest
    {
        public ServiceRequest(BucketInfo bucket, string key, HttpMethod httpMethod)
        {
            Bucket = bucket;
            HttpMethod = httpMethod;
            ObjectKey = key;
        }

        /// <summary>
        /// 请求方式
        /// </summary>
        public HttpMethod HttpMethod { get; set; }

        /// <summary>
        /// 所属bucket的信息
        /// </summary>
        public BucketInfo Bucket { get; set; }

        /// <summary>
        /// 要操作对象的key
        /// </summary>
        public string ObjectKey { get; set; }

        /// <summary>
        /// 附加的http头
        /// </summary>
        public IDictionary<string, string> Headers { get;  } = new Dictionary<string, string>();

        /// <summary>
        /// URL 参数
        /// </summary>
        public IDictionary<string,string> Parameters { get; } = new Dictionary<string, string>();

        /// <summary>
        /// 添加一个参数
        /// </summary>
        /// <param name="key">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="skipIfEmpty">如果参数值为空，是否忽略此参数</param>
        public ServiceRequest AddParameter(string key, string value, bool skipIfEmpty = true)
        {
            if (skipIfEmpty && String.IsNullOrEmpty(value))
                return this;

            Parameters[key] = value;

            return this;
        }

        /// <summary>
        /// Content MD5
        /// </summary>
        public byte[] ContentMd5 { get; set; }

        

        /// <summary>
        /// 内容的mime类型
        /// </summary>
        public string ContentMimeType { get; private set; }

        /// <summary>
        /// 请求内容类型
        /// </summary>
        public RequestContentType RequestContentType { get; private set; } = RequestContentType.None;


        /// <summary>
        /// 流内容，和字符串内容二者只要有1个
        /// </summary>
        public Stream StreamContent { get; private set; }

        /// <summary>
        /// 字符串内容
        /// </summary>
        public string StringContent { get; private set; }

        /// <summary>
        /// 设置stream类型的内容
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="contentType"></param>
        public void SetContent(Stream stream, string contentType)
        {
            RequestContentType = RequestContentType.Stream;
            StreamContent = stream;
            ContentMimeType = contentType;
        }

        /// <summary>
        /// 设置字符串类型的内容
        /// </summary>
        /// <param name="body"></param>
        /// <param name="contentType"></param>
        public void SetContent(string body, string contentType)
        {
            RequestContentType = RequestContentType.String;
            StringContent = body;
            ContentMimeType = contentType;
        }

        //生成目标url
        public string BuildRequestUri(RequestContext context)
        {
            //对象路径
            var uri = Bucket.GetObjectUrl(ObjectKey);

            if (IsParameterInUri())
            {
                var paramString = HttpUtils.CombineQueryString(Parameters);
                if (!string.IsNullOrEmpty(paramString))
                    uri += "?" + paramString;
            }

            return uri;
        }

        private bool IsParameterInUri()
        {
            var requestHasPayload = RequestContentType != RequestContentType.None;
            var requestIsPost = HttpMethod == HttpMethod.Post;
            var putParamsInUri = !requestIsPost || requestHasPayload;
            return putParamsInUri;
        }
    }
}
