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
        private RequestContext _requestContext;

        public ServiceCaller(RequestContext requestContext)
        {
            _requestContext = requestContext;
        }

        /// <summary>
        /// 执行一个服务请求
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> CallServiceAsync(ServiceRequest serviceRequest)
        {
            //
            HttpClient client = null;

            

            //
            // setup proxy
            // 
            if (!string.IsNullOrWhiteSpace(_requestContext.ClientConfiguration.ProxyHost))
            {
                HttpClientHandler httpClientHandler = new HttpClientHandler()
                {
                    Proxy = new MyProxy(string.Format("{0}:{1}", _requestContext.ClientConfiguration.ProxyHost, _requestContext.ClientConfiguration.ProxyPort)),
                    PreAuthenticate = false,
                    UseDefaultCredentials = true,
                };

                if (!String.IsNullOrEmpty(_requestContext.ClientConfiguration.ProxyUserName))
                {
                    httpClientHandler.Proxy.Credentials = String.IsNullOrEmpty(_requestContext.ClientConfiguration.ProxyDomain)
                        ? new NetworkCredential(_requestContext.ClientConfiguration.ProxyUserName, _requestContext.ClientConfiguration.ProxyPassword ?? string.Empty)
                        : new NetworkCredential(_requestContext.ClientConfiguration.ProxyUserName, _requestContext.ClientConfiguration.ProxyPassword ?? string.Empty,
                            _requestContext.ClientConfiguration.ProxyDomain);

                    httpClientHandler.UseDefaultCredentials = false;
                    httpClientHandler.PreAuthenticate = true;
                }

                client = new HttpClient(httpClientHandler);
            }
            else
            {
                client = new HttpClient();
            }


            var request = new HttpRequestMessage(serviceRequest.HttpMethod,
                serviceRequest.BuildRequestUri(_requestContext));

            //
            // setup headers
            //

            // 超时时间，毫秒
            client.Timeout = TimeSpan.FromMilliseconds(_requestContext.ClientConfiguration.ConnectionTimeout);
            foreach (var h in serviceRequest.Headers)
            {
                bool rtn = request.Headers.TryAddWithoutValidation(h.Key, h.Value);
                if (rtn == false)
                {
                    //throw new InvalidOperationException("不支持的header:" + h.Key);
                    Console.WriteLine("不支持的header:" + h.Key);
                }
            }

            if (!string.IsNullOrWhiteSpace(_requestContext.ClientConfiguration.UserAgent))
            {
                client.DefaultRequestHeaders.UserAgent.TryParseAdd(_requestContext.ClientConfiguration.UserAgent);

            }

            // request content
            //

            

            if (serviceRequest.RequestContentType !=  RequestContentType.None 
                && (serviceRequest.HttpMethod == HttpMethod.Put || serviceRequest.HttpMethod == HttpMethod.Post))
            {
                if (serviceRequest.RequestContentType == RequestContentType.String)
                {
                    request.Content = new StringContent(serviceRequest.StringContent, Encoding.UTF8, serviceRequest.ContentMimeType);
                }else if (serviceRequest.RequestContentType == RequestContentType.Stream)
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
            return await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        }
    }
}
