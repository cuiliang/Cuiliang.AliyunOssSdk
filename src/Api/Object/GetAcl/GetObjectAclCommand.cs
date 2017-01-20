using System.Net.Http;
using Cuiliang.AliyunOssSdk.Api.Base;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;

namespace Cuiliang.AliyunOssSdk.Api.Object.GetAcl
{
    public class GetObjectAclCommand:BaseObjectCommand<GetObjectAclResult>
    {
        public GetObjectAclCommand(RequestContext requestContext, BucketInfo bucket, string key) : base(requestContext, bucket, key)
        {
        }

        public override ServiceRequest BuildRequest()
        {
            var req = new ServiceRequest(Bucket, Key, HttpMethod.Get);

            req.Parameters.Add("acl", "");

            return req;
        }
    }
}
