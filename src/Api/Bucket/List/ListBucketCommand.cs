using System.Net.Http;
using Cuiliang.AliyunOssSdk.Api.Base;
using Cuiliang.AliyunOssSdk.Api.Common.Consts;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;

namespace Cuiliang.AliyunOssSdk.Api.Bucket.List
{
    public class ListBucketCommand: BaseOssCommand<ListBucketsResult>
    {
        private ListBucketsRequest _request;
        private string _region;

        public ListBucketCommand(RequestContext requestContext, string region, ListBucketsRequest request) : base(requestContext)
        {
            _request = request;
            _region = region;
        }

        public override ServiceRequest BuildRequest()
        {
            
            var req = new ServiceRequest(BucketInfo.CreateByRegion(_region, ""), "", HttpMethod.Get);

            //
            req.AddParameter(RequestParameters.PREFIX, _request.Prefix);
            req.AddParameter(RequestParameters.MARKER, _request.Marker);
            req.AddParameter(RequestParameters.MAX_KEYS, _request.MaxKeys?.ToString());

            return req;
        }
        
    }
}
