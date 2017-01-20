/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;

namespace Cuiliang.AliyunOssSdk.Entites
{
    /// <summary>
    /// 表示访问阿里云服务的配置信息。
    /// </summary>
    public class ClientConfiguration 
    {
        private const string UserAgentPrefix = "aliyun-sdk-dotnet/";
        private static readonly string _userAgent = GetDefaultUserAgent();

        

        /// <summary>
        /// HttpWebRequest最大的并发连接数目。
        /// </summary>
        public const int ConnectionLimit = 512;

        public bool UseHttps { get; set; } = false;

        /// <summary>
        /// 获取访问请求的User-Agent。
        /// </summary>
        public string UserAgent
        {
            get { return _userAgent; }
        }

        /// <summary>
        /// 获取或设置代理服务器的地址。
        /// </summary>
        public string ProxyHost { get; set; }

        /// <summary>
        /// 获取或设置代理服务器的端口。
        /// </summary>
        public int ProxyPort { get; set; } = -1;

        /// <summary>
        /// 获取或设置用户名。
        /// </summary>
        public string ProxyUserName { get; set; }

        /// <summary>
        /// 获取或设置密码。
        /// </summary>
        public string ProxyPassword { get; set; }

        /// <summary>
        /// 获取或设置代理服务器授权用户所在的域。
        /// </summary>
        public string ProxyDomain { get; set; }

        /// <summary>
        /// 获取或设置连接超时时间，单位为毫秒。
        /// </summary>
        public int ConnectionTimeout { get; set; } = -1;

        /// <summary>
        /// 获取或设置请求发生错误时最大的重试次数。
        /// </summary>
        public int MaxErrorRetry { get; set; } = 3;

        

        /// <summary>
        /// <para>设置自定义基准时间。</para>
        /// <para>
        /// 由于OSS的token校验是时间相关的，可能会因为终端系统时间不准导致无法访问OSS服务。
        /// 通过该接口设置自定义Epoch秒数，SDK计算出本机当前时间与自定义时间的差值，之后的
        /// 每次请求时间均加上该差值，以达到时间校验的目的。
        /// </para>
        /// </summary>
        /// <param name="epochTicks">自定义Epoch秒数。</param>
        public void SetCustomEpochTicks(long epochTicks)
        {
            var epochTime = new DateTime(1970, 1, 1);
            var timeSpan = DateTime.UtcNow.Subtract(epochTime);
            var localTicks = (long)timeSpan.TotalSeconds;
            TickOffset = epochTicks - localTicks;
        }

        /// <summary>
        /// 获取自定义基准时间与本地时间的时差值，单位为秒。
        /// </summary>
        public long TickOffset { get; internal set; }


        /// <summary>
        /// 默认配置
        /// </summary>
        public static ClientConfiguration Default {
            get
            {
                return new ClientConfiguration();
            }
        }

       
        /// <summary>
        /// 获取User-Agent信息。
        /// </summary>
        private static string GetDefaultUserAgent()
        {
            return UserAgentPrefix +
                typeof(ClientConfiguration).AssemblyQualifiedName;
        }
    }
}
