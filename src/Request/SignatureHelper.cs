using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Cuiliang.AliyunOssSdk.Api.Common.Consts;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Utility;

namespace Cuiliang.AliyunOssSdk.Request
{
    /// <summary>
    /// 签名处理
    /// </summary>
    public class SignatureHelper
    {
        /// <summary>
        /// HmacSha1签名算法
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="data">要签名的数据</param>
        /// <returns></returns>
        private static string HmacSha1Sign(string key, string data)
        {
            var signer = new System.Security.Cryptography.HMACSHA1();


            signer.Key = Encoding.UTF8.GetBytes(key.ToCharArray());
            return Convert.ToBase64String(
                signer.ComputeHash(Encoding.UTF8.GetBytes(data.ToCharArray())));
        }

        /// <summary>
        /// Authorization字段计算的方法
        /// </summary>
        /// <param name="accessKeySecret">密钥</param>
        /// <param name="httpMethod">表示HTTP 请求的Method，主要有PUT，GET，POST，HEAD，DELETE等</param>
        /// <param name="contentMd5">表示请求内容数据的MD5值，对消息内容（不包括头部）计算MD5值获得128比特位数字，对该数字进行base64编码而得到。
        /// 该请求头可用于消息合法性的检查（消息内容是否与发送时一致），如”eB5eJF1ptWaXm4bijSPyxw==”，也可以为空。</param>
        /// <param name="contentType">请求内容的类型，如”application/octet-stream”，也可以为空</param>
        /// <param name="date">表示此次操作的时间，且必须为HTTP1.1中支持的GMT格式，如”Sun, 22 Nov 2015 08:16:38 GMT”</param>
        /// <param name="canonicalizedOSSHeaders">表示以“x-oss-”为前缀的http header的组合
        ///   构建CanonicalizedOSSHeaders的方法：https://help.aliyun.com/document_detail/31951.html?spm=5176.doc31950.6.385.9GhGxu
        /// </param>
        /// <param name="canonicalizedResource">表示用户想要访问的OSS资源。详细算法参见文档</param>
        /// <remarks>
        /// Date和CanonicalizedResource不能为空；如果请求中的Date时间和OSS服务器的时间差15分钟以上，OSS服务器将拒绝该服务，并返回HTTP 403错误。
        /// </remarks>
        /// <returns></returns>
        public static string HmacSha1Sign(
            string accessKeySecret,
            string httpMethod,
            string contentMd5,
            string contentType,
            string date,
            string canonicalizedOSSHeaders,
            string canonicalizedResource)
        {
            var strToSign =
                $"{httpMethod.ToUpper()}\n{contentMd5}\n{contentType}\n{date}\n{canonicalizedOSSHeaders}{canonicalizedResource}";

            //Console.WriteLine("String to sign:" + strToSign);

            return HmacSha1Sign(accessKeySecret, strToSign);
        }

        public static string ComputeCanonicalizedOSSHeaders(IDictionary<string,string> headers, IDictionary<string,string> queryParameters = null)
        {

            var canonicalString = new StringBuilder();

            IDictionary<string, string> headersToSign = new Dictionary<string, string>();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    var lowerKey = header.Key.ToLowerInvariant();

                    if (lowerKey.StartsWith(OssHeaders.OssPrefix))
                    {
                        headersToSign.Add(lowerKey, header.Value);
                    }
                }
            }

            //在Get请求中，有些需要在url中出现的参数，也需要进行签名
            // Add params that start with "x-oss-"
            if (queryParameters != null)
            {
                foreach (var p in queryParameters)
                {
                    if (p.Key.StartsWith(OssHeaders.OssPrefix))
                        headersToSign.Add(p.Key.Trim(), p.Value.Trim());
                }
            }

            // Add all headers to sign into canonical string, 
            // note that these headers should be ordered before adding.
            var sortedHeaders = new List<string>(headersToSign.Keys);
            sortedHeaders.Sort();
            foreach (var key in sortedHeaders)
            {
                var value = headersToSign[key];
                if (key.StartsWith(OssHeaders.OssPrefix))
                    canonicalString.Append(key).Append(':').Append(value);
                else
                    canonicalString.Append(value);

                canonicalString.Append("\n");
            }

            return canonicalString.ToString();
        }

       


        private static readonly IList<string> ParamtersToSign = new List<string> {
            "acl",
"uploadId",
"partNumber",
"uploads",
"logging",
"website",
"location",
"lifecycle",
"referer",
"cors",
"delete",
"append",
"position",
"bucketInfo",
            ResponseHeaderOverrides.ResponseCacheControl,
            ResponseHeaderOverrides.ResponseContentDisposition,
            ResponseHeaderOverrides.ResponseContentEncoding,
            ResponseHeaderOverrides.ResponseHeaderContentLanguage,
            ResponseHeaderOverrides.ResponseHeaderContentType,
            ResponseHeaderOverrides.ResponseHeaderExpires,
            RequestParameters.SECURITY_TOKEN,  //
            "objectMeta"
        };

        public static string BuildCanonicalizedResource(BucketInfo bucket, string key,  IDictionary<string,string> parameters)
        {
            var sb = new StringBuilder();

            sb.Append(bucket.MakeResourcePathForSign(key));

            if (parameters != null)
            {
                var parameterNames = parameters.Keys.Where(k => ParamtersToSign.Contains(k)).ToList();

                if (parameterNames.Count > 0)
                {
                    parameterNames.Sort();


                    var separator = '?';
                    foreach (var paramName in parameterNames)
                    {

                        sb.Append(separator);
                        sb.Append(paramName);
                        var paramValue = parameters[paramName];
                        if (!string.IsNullOrEmpty(paramValue))
                            sb.Append("=").Append(paramValue);

                        separator = '&';
                    }
                }
                

                
            }

            return sb.ToString();
        }

        /// <summary>
        /// 对一个请求计算前面并放入header中
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <param name="credential"></param>
        public static void SignRequest(ServiceRequest serviceRequest, OssCredential credential, HttpRequestMessage httpRequestMessage)
        {
            string httpMethod = serviceRequest.HttpMethod.Method.ToUpperInvariant();
            string contentType = httpRequestMessage.Content?.Headers.ContentType?.ToString();
            string date = serviceRequest.Headers[HttpHeaders.Date];
            string contentMd5 = serviceRequest.ContentMd5 == null ? "" : Convert.ToBase64String(serviceRequest.ContentMd5);
            string ossHeaders = ComputeCanonicalizedOSSHeaders(serviceRequest.Headers, serviceRequest.Parameters);


            string resourcePath = BuildCanonicalizedResource(serviceRequest.Bucket, serviceRequest.ObjectKey, serviceRequest.Parameters);

            var sign = HmacSha1Sign(credential.AccessKeySecret, httpMethod, contentMd5, contentType, date, ossHeaders,
                resourcePath);

            httpRequestMessage.Headers.Add(HttpHeaders.Authorization, "OSS " + credential.AccessKeyId + ":" + sign);
        } 


    }
}
