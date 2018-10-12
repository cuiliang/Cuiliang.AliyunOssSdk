using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Cuiliang.AliyunOssSdk.Api.Base;
using Cuiliang.AliyunOssSdk.Api.Common.Consts;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;

namespace Cuiliang.AliyunOssSdk.Api.Bucket.Get
{
    /// <summary>
    /// GetBucket(ListObject)接口可用来列出 Bucket中所有Object的信息。
    /// 
    /// </summary>
    public class GetBucketCommand : BaseOssCommand<GetBucketResult>
    {
        private readonly BucketInfo _bucketInfo;
        private readonly string _prefix;
        private readonly string _marker;
        private readonly int _maxKeys;
        private readonly string _delimiter;
        private readonly string _encodingType;

        public GetBucketCommand(RequestContext requestContext,
            BucketInfo bucketInfo,
            string prefix,
            string marker,
            int maxKeys,
            string delimiter,
            string encodingType
            ) 
            : base(requestContext)
        {
            _bucketInfo = bucketInfo;
            _prefix = prefix;
            _marker = marker;
            _maxKeys = maxKeys;
            _delimiter = delimiter;
            _encodingType = encodingType;
        }

        public override ServiceRequest BuildRequest()
        {
            var req = new ServiceRequest(_bucketInfo, "", HttpMethod.Get);

            req.AddParameter(RequestParameters.DELIMITER, _delimiter);
            req.AddParameter(RequestParameters.MARKER, _marker);
            req.AddParameter(RequestParameters.MAX_KEYS, _maxKeys.ToString());
            req.AddParameter(RequestParameters.PREFIX, _prefix);
            req.AddParameter(RequestParameters.ENCODING_TYPE, _encodingType);

            return req;
        }
    }
}
