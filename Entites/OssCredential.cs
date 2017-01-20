using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cuiliang.AliyunOssSdk.Entites
{
    /// <summary>
    /// Oss访问安全凭据
    /// </summary>
    public class OssCredential
    {
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }

        /// <summary>
        /// 是否以STS获得的
        /// </summary>
        public bool UseToken {
            get { return !String.IsNullOrEmpty(SecurityToken); }
        }

        /// <summary>
        /// 令牌
        /// </summary>
        public string SecurityToken { get; set; }
    }
}
