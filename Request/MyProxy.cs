using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Cuiliang.AliyunOssSdk.Request
{
    /// <summary>
    /// Http代理参数
    /// </summary>
    public class MyProxy : IWebProxy
    {
        public MyProxy(string proxyUri)
            : this(new Uri(proxyUri))
        {
        }

        public MyProxy(Uri proxyUri)
        {
            this.ProxyUri = proxyUri;
        }

        public Uri ProxyUri { get; set; }

        public ICredentials Credentials { get; set; }

        public Uri GetProxy(Uri destination)
        {
            return this.ProxyUri;
        }

        public bool IsBypassed(Uri host)
        {
            return false; /* Proxy all requests */
        }
    }
}
