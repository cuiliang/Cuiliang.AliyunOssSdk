using System.Net.Http;
using System.Net.Http.Headers;

namespace Cuiliang.AliyunOssSdk.Api.Object.Get
{
    public class GetObjectResult
    {
        public HttpResponseHeaders Headers { get; set; }

        public HttpContent Content { get; set; }

        
    }
}
