using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Cuiliang.AliyunOssSdk.Api.Common.Consts;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;
using Cuiliang.AliyunOssSdk.Utility;

namespace Cuiliang.AliyunOssSdk.Api.Base
{
    /// <summary>
    /// OSS API命令基类，负责构建request，并对结果进行解析
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public abstract class BaseOssCommand<TResult>
    {
        protected RequestContext RequestContext { get; private set; }

        public BaseOssCommand(RequestContext requestContext)
        {
            RequestContext = requestContext;
        }

        /// <summary>
        /// 构造Request请求，由派生类实现
        /// </summary>
        /// <returns></returns>
        public abstract ServiceRequest BuildRequest();

        /// <summary>
        /// 解析结果对象。基类提供通用方式处理：解析xml。对于特殊返回结果，定义重载函数来进行处理。
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public virtual async Task<OssResult<TResult>> ParseResultAsync(HttpResponseMessage response)
        {
            //成功情况下的默认解析处理
            var length = response.Content?.Headers?.ContentLength;
            if (length > 0)
            {
                var result = SerializeHelper.Deserialize<TResult>(await response.Content.ReadAsStreamAsync());
                var ossResult = new OssResult<TResult>()
                {
                    IsSuccess =  true,
                    SuccessResult = result
                };

                return ossResult;
            }

            return new OssResult<TResult>()
            {
                IsSuccess =  false,
                ErrorMessage = "ContentLength = 0"
            };
        }

        /// <summary>
        /// 执行请求并返回结果
        /// </summary>
        /// <returns></returns>
        public async Task<OssResult<TResult>> ExecuteAsync()
        {
            try
            {
                ServiceRequest request = BuildRequest();

                //加入dateheader
                request.Headers[HttpHeaders.Date] = DateUtils.FormatRfc822Date(DateTime.UtcNow);
                

                if (RequestContext.OssCredential.UseToken)
                {
                    request.Headers[HttpHeaders.SecurityToken] = RequestContext.OssCredential.SecurityToken;
                }


                // 发送请求
                var caller = new ServiceCaller(RequestContext);
                HttpResponseMessage response = await caller.CallServiceAsync(request);

                // 解析结果
                return await ProcessResponseInternal(response);
            }
            catch (Exception ex)
            {
                return new OssResult<TResult>()
                {
                    IsSuccess = false,
                    InnerException = ex,
                    ErrorMessage = ex.Message
                };
            }
            
        }

        /// <summary>
        /// 初步处理返回的结果，对于失败的http代码，进行错误解析处理
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task<OssResult<TResult>> ProcessResponseInternal(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await ParseResultAsync(response);
            }
            else if (response.StatusCode == HttpStatusCode.NotModified) //此处处理是参考官方的sdk
            {
                return new OssResult<TResult>()
                {
                    IsSuccess =  false,
                    ErrorMessage = "NOT_MODIFIED"
                };
            }else
            {
                //错误的http代码
                if (response.Content?.Headers.ContentLength > 0)
                {
                    var errorResult =
                        SerializeHelper.Deserialize<ErrorResult>(await response.Content.ReadAsStreamAsync());
                    
                    return new OssResult<TResult>()
                    {
                        IsSuccess = false,
                        ErrorResult = errorResult,
                        ErrorMessage = errorResult.Message
                    };
                }

                return new OssResult<TResult>()
                {
                    IsSuccess = false,
                    ErrorMessage = "STATUSCODE:" + response.StatusCode
                };
            }
        }
    }
}
