using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Cuiliang.AliyunOssSdk.Api.Base;
using Cuiliang.AliyunOssSdk.Api.Common.Consts;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;

namespace Cuiliang.AliyunOssSdk.Api.Object.Append
{
    public class AppendObjectCommand : BaseObjectCommand<AppentObjectResult>
    {
        private const string AppendName = "append";
        private const string AppendPosition = "position";

        public AppendObjectCommand(RequestContext requestContext, 
            BucketInfo bucket, 
            string key, 
            long position,
            RequestContent objectInfo) : base(requestContext, bucket, key)
        {
            Position = position;
            RequestContent = objectInfo;
        }

        public long Position { get; set; }

        public RequestContent RequestContent { get; set; }

        public override ServiceRequest BuildRequest()
        {
            var req = new ServiceRequest(Bucket, Key, HttpMethod.Post);


            // headers
            //if (ExtraHeaders != null)
            //{
            //    foreach (var pair in ExtraHeaders)
            //    {
            //        req.Headers.Add(pair);
            //    }
            //}

            RequestContent.Metadata?.Populate(req.Headers);

            // params
            req.Parameters.Add(AppendName, "");
            req.Parameters.Add(AppendPosition, Position.ToString());

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

        public override async Task<OssResult<AppentObjectResult>> ParseResultAsync(HttpResponseMessage response)
        {
            var result = new AppentObjectResult();
            result.ETag = response.Headers.ETag?.ToString();

            if (response.Headers.Contains(HttpHeaders.NextAppendPosition))
            {
                result.NextAppendPosition =
                    Convert.ToInt64(response.Headers.GetValues(HttpHeaders.NextAppendPosition).First());
            }

            if (response.Headers.Contains(HttpHeaders.HashCrc64Ecma))
            {
                result.HashCrc64Ecma = Convert.ToUInt64(response.Headers.GetValues(HttpHeaders.HashCrc64Ecma).FirstOrDefault());
            }

            return new OssResult<AppentObjectResult>(result);
        }
    }
}
