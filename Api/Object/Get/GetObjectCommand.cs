using System.Net.Http;
using System.Threading.Tasks;
using Cuiliang.AliyunOssSdk.Api.Base;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;

namespace Cuiliang.AliyunOssSdk.Api.Object.Get
{
    public class GetObjectCommand : BaseObjectCommand<GetObjectResult>
    {
        

        public GetObjectParams Params { get; set; }



        /// 

        public GetObjectCommand(
            RequestContext requestContext,
            BucketInfo bucket,
            string key,
            GetObjectParams parameters
            ) : base(requestContext, bucket, key)
        {
            Params = parameters;
        }

        public override ServiceRequest BuildRequest()
        {

            var req = new ServiceRequest(Bucket, Key, HttpMethod.Get);

            Params?.SetupRequest(req);

            return req;
        }



        public override async Task<OssResult<GetObjectResult>> ParseResultAsync(HttpResponseMessage response)
        {
            var result = new GetObjectResult();
            result.Headers = response.Headers;
            result.Content = response.Content;

            return new OssResult<GetObjectResult>()
            {
                IsSuccess = true,
                SuccessResult = result
            };
        }
    }
}
