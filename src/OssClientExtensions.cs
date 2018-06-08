using System;
using Cuiliang.AliyunOssSdk;
using Cuiliang.AliyunOssSdk.Api.Bucket.List;
using Cuiliang.AliyunOssSdk.Entites;
using Cuiliang.AliyunOssSdk.Request;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cuiliang.AliyunOssSdk
{
    public static class OssClientExtensions
    {
        public static IServiceCollection AddOssClient(this IServiceCollection services,
        Action<OssCredential> optionsAction,
        ClientConfiguration config = null)
        {

            var credential = new OssCredential();
            optionsAction?.Invoke(credential);
            var requestContext = new RequestContext(credential, config ?? ClientConfiguration.Default);

            services.AddSingleton<RequestContext>(requestContext);
            services.AddHttpClient<OssClient>();

            return services;
        }

        public static IServiceCollection AddOssClient(this IServiceCollection services,
        IConfigurationSection ossClientConf,
        ClientConfiguration config = null)
        {
            var ossClientOptions = new OssClientOptions();
            ossClientConf.Bind(ossClientOptions);

            return
            AddOssClient(services, options =>
            {
                options.AccessKeyId = ossClientOptions.AccessKeyId;
                options.AccessKeySecret = ossClientOptions.AccessKeySecret;
                options.SecurityToken = ossClientOptions.SecurityToken;
            }, config);
        }
    }
}