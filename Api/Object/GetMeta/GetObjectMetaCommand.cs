using System.Net.Http;
using System.Threading.Tasks;
using Cuiliang.AliyunOssSdk.Api.Base;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;

namespace Cuiliang.AliyunOssSdk.Api.Object.GetMeta
{
    public class GetObjectMetaCommand:BaseObjectCommand<GetObjectMetaResult>
    {
        public GetObjectMetaCommand(RequestContext requestContext, BucketInfo bucket, string key) : base(requestContext, bucket, key)
        {
        }

        public override ServiceRequest BuildRequest()
        {
            var req = new ServiceRequest(Bucket, Key, HttpMethod.Get);

            req.Parameters.Add("objectMeta","");

            return req;
        }

        public override async Task<OssResult<GetObjectMetaResult>> ParseResultAsync(HttpResponseMessage response)
        {
            return new OssResult<GetObjectMetaResult>(new GetObjectMetaResult()
            {
                Headers = response.Headers
            });
        }
    }
}
