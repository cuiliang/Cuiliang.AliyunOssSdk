using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Cuiliang.AliyunOssSdk.Api.Base;
using Cuiliang.AliyunOssSdk.Api.Common.Consts;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;
using Cuiliang.AliyunOssSdk.Utility;

namespace Cuiliang.AliyunOssSdk.Api.Object.Get
{
    public class GetObjectCommand : BaseObjectCommand<GetObjectResult>
    {
        public GetObjectParams Params { get; set; }

        public GetObjectCommand(
            RequestContext requestContext,
            BucketInfo bucket,
            string key,
            GetObjectParams parameters) : base(requestContext, bucket, key)
        {
            Params = parameters;
        }

        public override ServiceRequest BuildRequest()
        {
            var req = new ServiceRequest(Bucket, Key, HttpMethod.Get);
            Params?.SetupRequest(req);
            return req;
        }

        public override  Task<OssResult<GetObjectResult>> ParseResultAsync(HttpResponseMessage response)
        {
            var result = new GetObjectResult();
            result.Headers = response.Headers;
            result.Content = response.Content;
            result.Metadata = Deserialize(response);

            return Task.FromResult(new OssResult<GetObjectResult>()
            {
                IsSuccess = true,
                SuccessResult = result
            });
        }

        private ObjectMetadata Deserialize(HttpResponseMessage response)
        {
            var metadata = new ObjectMetadata();
            foreach (var header in response.Headers)
            {
                if (header.Key.StartsWith(OssHeaders.OssUserMetaPrefix, false, CultureInfo.InvariantCulture))
                {
                    // The key of user in the metadata should not contain the prefix.
                    metadata.UserMetadata.Add(header.Key.Substring(OssHeaders.OssUserMetaPrefix.Length),
                        header.Value.FirstOrDefault());
                }
                else if (string.Equals(header.Key, HttpHeaders.ContentLength, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Content-Length. Parse should not fail.
                    metadata.ContentLength = long.Parse(header.Value.FirstOrDefault(), CultureInfo.InvariantCulture);
                }
                else if (string.Equals(header.Key, HttpHeaders.ETag, StringComparison.InvariantCultureIgnoreCase))
                {
                    metadata.ETag = OssUtils.TrimETag(header.Value.FirstOrDefault());
                }
                else if (string.Equals(header.Key, HttpHeaders.LastModified, StringComparison.InvariantCultureIgnoreCase))
                {
                    metadata.LastModified = DateUtils.ParseRfc822Date(header.Value.FirstOrDefault());
                }
                else
                {
                    // Treat the other headers just as strings.
                    metadata.AddHeader(header.Key, header.Value);
                }
            }
            return metadata;
        }
    }
}
