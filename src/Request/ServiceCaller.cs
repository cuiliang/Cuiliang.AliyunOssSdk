using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Cuiliang.AliyunOssSdk.Entites;
using HttpHeaders = Cuiliang.AliyunOssSdk.Api.Common.Consts.HttpHeaders;

namespace Cuiliang.AliyunOssSdk.Request
{
    /// <summary>
    /// 执行一个ServiceRequest服务请求
    /// </summary>
    public class ServiceCaller
    {
        private readonly static HttpClient _client = new HttpClient();
        private RequestContext _requestContext;
        private readonly HttpClient _client;

        public ServiceCaller(RequestContext requestContext, HttpClient client)
        {
            _requestContext = requestContext;
            _client = client;
        }

        /// <summary>
        /// 执行一个服务请求
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> CallServiceAsync(ServiceRequest serviceRequest)
        {
            var request = new HttpRequestMessage(serviceRequest.HttpMethod,
                serviceRequest.BuildRequestUri(_requestContext));

            //
            // setup headers
            //
            // 超时时间，毫秒
            //httpclient 默认100s
            // httpclient的 BaseAddress TimeOut MaxResponseContentBufferSize 属性 只能被设置一次
            //第二次设置就会出现异常：
            //“This instance has already started one or more requests. 
            // Properties can only be modified before sending the first request.”
            if (_requestContext.ClientConfiguration.ConnectionTimeout != -1)
                _client.Timeout = TimeSpan.FromMilliseconds(_requestContext.ClientConfiguration.ConnectionTimeout);
            foreach (var h in serviceRequest.Headers)
            {
                bool rtn = request.Headers.TryAddWithoutValidation(h.Key, h.Value);
                if (rtn == false)
                {
                    throw new InvalidOperationException("不支持的header:" + h.Key);
                }
            }

            if (!string.IsNullOrWhiteSpace(_requestContext.ClientConfiguration.UserAgent))
            {
                _client.DefaultRequestHeaders.UserAgent.TryParseAdd(_requestContext.ClientConfiguration.UserAgent);

            }

            // request content
            //            

            if (serviceRequest.RequestContentType != RequestContentType.None
                && (serviceRequest.HttpMethod == HttpMethod.Put || serviceRequest.HttpMethod == HttpMethod.Post))
            {
                if (serviceRequest.RequestContentType == RequestContentType.String)
                {
                    request.Content = new StringContent(serviceRequest.StringContent, Encoding.UTF8, serviceRequest.ContentMimeType);
                }
                else if (serviceRequest.RequestContentType == RequestContentType.Stream)
                {
                    request.Content = new StreamContent(serviceRequest.StreamContent);
                    request.Content.Headers.ContentType =
                        MediaTypeHeaderValue.Parse(serviceRequest.ContentMimeType);
                }

            }

            if (serviceRequest.ContentMd5 != null && request.Content != null)
            {
                request.Content.Headers.ContentMD5 = serviceRequest.ContentMd5;
            }

            // 因为传入的contenttype会被自动更改，所以在最后再进行签名处理
            SignatureHelper.SignRequest(serviceRequest, _requestContext.OssCredential, request);

            // 需要增加是因为objectMeta和head API返回的消息体是空的，但是content-length却是实际object的长度，此处违反了协议。
            // 使用默认参数会自动读取content，导致异常。
            return await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        }
    }
}
