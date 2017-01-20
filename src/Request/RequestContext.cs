using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuiliang.AliyunOssSdk.Entites;

namespace Cuiliang.AliyunOssSdk.Request
{
    /// <summary>
    /// 请求上下文参数，存储认证信息、客户端配置等
    /// </summary>
    public class RequestContext
    {
        public RequestContext(OssCredential credential, ClientConfiguration config)
        {
            OssCredential = credential;
            ClientConfiguration = config;
        }
        /// <summary>
        /// 客户端配置
        /// </summary>
        public ClientConfiguration ClientConfiguration { get; set; }

        /// <summary>
        /// 密钥凭据
        /// </summary>
        public OssCredential OssCredential { get; set; }
    }
}
