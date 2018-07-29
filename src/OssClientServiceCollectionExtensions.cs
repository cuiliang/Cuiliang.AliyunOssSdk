using System;
using System.Net.Http;
using Cuiliang.AliyunOssSdk;
using Cuiliang.AliyunOssSdk.Api.Bucket.List;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OssClientServiceCollectionExtensions
    {
        public static IServiceCollection AddOssClient(
            this IServiceCollection services,
            IConfiguration configuration, 
            string sectionName = "ossClient",
            Action<ClientConfiguration> setupClientConfiguration = null,
            Action<HttpClient> configureHttpClient = null)
        {
            services.Configure<OssCredential>(configuration.GetSection("ossClient"));

            var clientConfiguration = new ClientConfiguration();
            setupClientConfiguration?.Invoke(clientConfiguration);
            services.AddSingleton(clientConfiguration);

            services.AddTransient<RequestContext>();

            if (configureHttpClient == null)
            {
                services.AddHttpClient<OssClient>();
            }
            else
            {
                services.AddHttpClient<OssClient>(configureHttpClient);
            }

            return services;
        }
    }
}