using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cuiliang.AliyunOssSdk.Request;

namespace Cuiliang.AliyunOssSdk.Entites
{
    /// <summary>
    /// 封装了请求内容body。可能是一个字符串类型的内容，或者Stream类型的内容
    /// </summary>
    public class RequestContent
    {
        public string MimeType { get; set; }

        public RequestContentType ContentType { get; set; }

        public string StringContent { get; set; }
        public Stream StreamContent { get; set; }

        public ObjectMetadata Metadata { get; set; }
        public byte[] ContentMd5 { get; internal set; }
    }
}
