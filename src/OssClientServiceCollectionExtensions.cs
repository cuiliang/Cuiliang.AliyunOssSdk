using System;
using Cuiliang.AliyunOssSdk;
using Cuiliang.AliyunOssSdk.Api.Bucket.List;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cuiliang.AliyunOssSdk
{
    public static class OssClientServiceCollectionExtensions
    {
        public static IServiceCollection AddOssClient(
            this IServiceCollection services,
            IConfigurationSection ossClientConf, 
            ClientConfiguration config = null)
        {
            var credential = new OssCredential();
            ossClientConf.Bind(credential);

            var requestContext = new RequestContext(credential, config ?? ClientConfiguration.Default);

            services.AddSingleton<RequestContext>(requestContext);
            services.AddHttpClient<OssClient>();

            return services;
        }
    }
}