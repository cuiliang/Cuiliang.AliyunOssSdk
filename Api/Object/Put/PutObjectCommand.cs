using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Cuiliang.AliyunOssSdk.Api.Base;
using Cuiliang.AliyunOssSdk.Api.Common.Consts;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;
using Cuiliang.AliyunOssSdk.Utility;

namespace Cuiliang.AliyunOssSdk.Api.Object.Put
{
    public class PutObjectCommand: BaseObjectCommand<PutObjectResult>
    {
        public IDictionary<string, string> ExtraHeaders { get; set; }

        public RequestContent RequestContent { get; set; }

        public PutObjectCommand(RequestContext requestContext, BucketInfo bucketInfo, string key, RequestContent requestContent, IDictionary<string, string> extraHeaders) 
            : base(requestContext, bucketInfo, key)
        {
            Bucket = bucketInfo;
            RequestContent = requestContent;
            ExtraHeaders = extraHeaders;
        }


        public override ServiceRequest BuildRequest()
        {
            var req = new ServiceRequest(Bucket, Key, HttpMethod.Put);
           

            // headers
            if (ExtraHeaders != null)
            {
                foreach (var pair in ExtraHeaders)
                {
                    req.Headers.Add(pair);
                }
            }

            RequestContent.Metadata?.Populate(req.Headers);

            //
            req.ContentMd5 = RequestContent.ContentMd5;

            if (RequestContent.ContentType == RequestContentType.Stream)
            {
                req.SetContent(RequestContent.StreamContent, RequestContent.MimeType);
            }
            else if (RequestContent.ContentType == RequestContentType.String)
            {
                req.SetContent(RequestContent.StringContent, RequestContent.MimeType);
            }
            else
            {
                throw new ArgumentException("错误的内容类型");
            }

            return req;
        }

        public override async Task<OssResult<PutObjectResult>> ParseResultAsync(HttpResponseMessage response)
        {
            var result = new PutObjectResult();
            if (response.Headers.Contains(HttpHeaders.ETag))
            {
                result.ETag = OssUtils.TrimQuotes(response.Headers.ETag.ToString());
            }

            return new OssResult<PutObjectResult>()
            {
                IsSuccess = true,
                SuccessResult = result
            };
        }
    }
}
