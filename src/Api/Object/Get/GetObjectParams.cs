using System;
using System.Collections.Generic;
using Cuiliang.AliyunOssSdk.Api.Common.Consts;
using Cuiliang.AliyunOssSdk.Request;
using Cuiliang.AliyunOssSdk.Utility;

namespace Cuiliang.AliyunOssSdk.Api.Object.Get
{
    public class GetObjectParams
    {
        /// <summary>
        /// 需要自定义返回的header
        /// OSS支持用户在发送GET请求时，可以自定义OSS返回请求中的一些Header，前提条件用户发送的GET请求必须携带签名。
        /// </summary>
        public ResponseHeaderOverrides OverrideResponseHeaders { get; set; }

        /// <summary>
        /// 指定文件传输的范围。如，设定 bytes=0-9，表示传送第0到第9这10个字符。
        /// </summary>
        public ObjectRange Range { get; set; }

        /// <summary>
        /// 如果指定的时间早于实际修改时间，则正常传送文件，并返回200 OK；否则返回304 not modified
        /// </summary>
        public DateTime? IfModifiedSince { get; set; }

        /// <summary>
        /// 如果传入参数中的时间等于或者晚于文件实际修改时间，则正常传输文件，并返回200 OK；否则返回412 precondition failed错误 
        /// </summary>
        public DateTime? IfUnmodifiedSince { get; set; }

        /// <summary>
        /// 如果传入期望的ETag和object的 ETag匹配，则正常传输文件，并返回200 OK；否则返回412 precondition failed错误
        /// </summary>
        public string IfEtagMatch { get; set; }

        /// <summary>
        /// 如果传入的ETag值和Object的ETag不匹配，则正常传输文件,并返回200 OK；否则返回304 Not Modified 
        /// </summary>
        public string IfEtagNoneMatch { get; set; }


        public GetObjectParams()
        {
            
        }

        public GetObjectParams(ResponseHeaderOverrides overrideHeaders = null,
            ObjectRange range = null,
            DateTime? ifModifiedSinceUtc = null,
            DateTime? ifUnmodifiedSinceUtc = null,
            string ifEtagMatch = "",
            string ifEtagNoneMatch = "")
        {
            OverrideResponseHeaders = OverrideResponseHeaders;
            Range = range;
            IfModifiedSince = ifModifiedSinceUtc;
            IfUnmodifiedSince = IfUnmodifiedSince;
            IfEtagMatch = ifEtagMatch;
            IfEtagNoneMatch = ifEtagNoneMatch;
        }

        public void SetupRequest(ServiceRequest req)
        {
            // override 
            if (OverrideResponseHeaders != null)
            {
                OverrideResponseHeaders.Populate(req.Parameters);
            }


            Range?.AddToHeader(req.Headers);
            if (IfModifiedSince != null)
            {
                AddToHeaderIfNotEmpty(OssHeaders.GetObjectIfModifiedSince, DateUtils.FormatRfc822Date(IfModifiedSince.Value), req.Headers);
            }
            if (IfUnmodifiedSince != null)
            {
                AddToHeaderIfNotEmpty(OssHeaders.GetObjectIfUnmodifiedSince, DateUtils.FormatRfc822Date(IfUnmodifiedSince.Value), req.Headers);
            }
            AddToHeaderIfNotEmpty(OssHeaders.GetObjectIfMatch, IfEtagMatch, req.Headers);
            AddToHeaderIfNotEmpty(OssHeaders.GetObjectIfNoneMatch, IfEtagNoneMatch, req.Headers);
        }

        /// <summary>
        /// 如果值不为空，将其加入到Header中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="headers"></param>
        private void AddToHeaderIfNotEmpty(string key, string value, IDictionary<string, string> headers)
        {
            if (!String.IsNullOrEmpty(value))
            {
                headers.Add(key, value);
            }
        }
    }
}
