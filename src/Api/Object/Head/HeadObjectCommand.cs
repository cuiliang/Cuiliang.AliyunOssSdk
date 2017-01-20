using System.Net.Http;
using System.Threading.Tasks;
using Cuiliang.AliyunOssSdk.Api.Base;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;

namespace Cuiliang.AliyunOssSdk.Api.Object.Head
{
    public class HeadObjectCommand:BaseObjectCommand<HeadObjectResult>
    {
        

        public HeadObjectCommand(RequestContext requestContext, BucketInfo bucket, string key, HeadObjectParams parameters) : base(requestContext, bucket, key)
        {
            Parameters = parameters;
        }

        public HeadObjectParams Parameters { get; set; }

        public override ServiceRequest BuildRequest()
        {
            var req = new ServiceRequest(Bucket, Key, HttpMethod.Head);

            Parameters?.SetupRequest(req);

            return req;
        }

        public override async Task<OssResult<HeadObjectResult>> ParseResultAsync(HttpResponseMessage response)
        {
            return new OssResult<HeadObjectResult>(new HeadObjectResult()
            {
                Headers = response.Headers
            });
        }
    }
}
