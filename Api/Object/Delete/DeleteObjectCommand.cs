using System.Net.Http;
using System.Threading.Tasks;
using Cuiliang.AliyunOssSdk.Api.Base;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;

namespace Cuiliang.AliyunOssSdk.Api.Object.Delete
{
    public class DeleteObjectCommand: BaseObjectCommand<DeleteObjectResult>
    {
        public DeleteObjectCommand(RequestContext requestContext, BucketInfo bucket, string key) : base(requestContext, bucket, key)
        {
        }

        public override ServiceRequest BuildRequest()
        {
            var req = new ServiceRequest(Bucket, Key, HttpMethod.Delete);

            return req;
        }

        public override async Task<OssResult<DeleteObjectResult>> ParseResultAsync(HttpResponseMessage response)
        {
            return new OssResult<DeleteObjectResult>(new DeleteObjectResult());
        }
    }
}
