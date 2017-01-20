using System;
using System.Collections.Generic;
using System.Net.Http;
using Cuiliang.AliyunOssSdk.Api.Base;
using Cuiliang.AliyunOssSdk.Api.Common.Consts;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;
using Cuiliang.AliyunOssSdk.Utility;

namespace Cuiliang.AliyunOssSdk.Api.Object.DeleteMultiple
{
    public class DeleteMultipleObjectsCommand: BaseOssCommand<DeleteMultipleObjectsResult>
    {
        public BucketInfo Bucket { get; set; }

        /// <summary>
        /// 要删除对象的key 数组
        /// </summary>
        public IList<string> Keys { get; set; }

        /// <summary>
        /// 指定对返回的Key进行编码，目前支持url编码。Key使用UTF-8字符，但xml 1.0标准不支持解析一些控制字符，比如ascii值从0到10的字符。对于Key中包含xml 1.0标准不支持的控制字符，可以通过指定encoding-type对返回的Key进行编码。
        /// 数据类型：字符串
        /// 默认值：无,可选值：url
        /// </summary>
        public string EncodingType { get; set; }

        /// <summary>
        /// 打开“简单”响应模式的开关。 
        /// 类型：枚举字符串
        /// 有效值：true、false 
        /// 默认值：false 
        /// 父节点：Delete
        /// </summary>
        public bool Quiet { get; set; } = false;

        public DeleteMultipleObjectsCommand(RequestContext requestContext,
            BucketInfo bucket,
            IList<string> keys,
            bool quiet = false,
            string encodingType="") : base(requestContext)
        {
            Bucket = bucket;
            Keys = keys;
            EncodingType = encodingType;
            Quiet = quiet;
        }

        public override ServiceRequest BuildRequest()
        {
            var req = new ServiceRequest(Bucket, "", HttpMethod.Post);

            // build content
            var model = new DeleteObjectsRequestModel(Quiet, Keys);
            var content = SerializeHelper.Serialize(model);
            req.ContentMd5 = OssUtils.ComputeContentMd5(content);
            req.SetContent(content, "application/xml");

            // params
            req.Parameters.Add("delete", "");

            if (!String.IsNullOrEmpty(EncodingType))
            {
                req.Parameters.Add(RequestParameters.ENCODING_TYPE, EncodingType);
            }

            return req;
        }
    }
}
