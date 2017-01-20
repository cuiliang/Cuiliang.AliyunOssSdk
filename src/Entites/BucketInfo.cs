using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuiliang.AliyunOssSdk.Utility;

namespace Cuiliang.AliyunOssSdk.Entites
{
    /// <summary>
    /// Bucket的位置信息，包含endpoint和
    /// </summary>
    public class BucketInfo
    {
        /// <summary>
        /// 是否自定义域名
        /// </summary>
        public bool IsCname { get; private set; }

        /// <summary>
        /// 是否https访问方式
        /// </summary>
        public bool IsHttps { get; private set; }

      

        /// <summary>
        /// Bucket名称
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// OSS端点
        /// </summary>
        public Uri EndpointUri { get; private set; }

        /// <summary>
        /// 带有bucket网址的uri
        /// </summary>
        public Uri BucketUri { get; private set; }

       

        private BucketInfo()
        {
        }

        /// <summary>
        /// 使用自定义域名创建地址信息
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="bucket"></param>
        /// <returns></returns>
        public static BucketInfo CreateByCname(Uri uri, string bucket)
        {
            var newBucket = new BucketInfo()
            {
                IsCname = true,
                IsHttps = uri.Scheme.ToLower() == "https",
                BucketName = bucket,
                EndpointUri = uri,
                BucketUri = uri, //TODO? CName情况下，oss端点地址和bucket端点地址一致？
            };

            return newBucket;
        }

        /// <summary>
        /// 根据所属区域创建bucket地址信息
        /// </summary>
        /// <param name="region"></param>
        /// <param name="bucketName"></param>
        /// <param name="useHttps"></param>
        /// <param name="useInternal">是否使用内网地址</param>
        /// <returns></returns>
        public static BucketInfo CreateByRegion(string region, string bucketName, bool useHttps = false, bool useInternal = false)
        {
            var  baseDomain = useInternal? "-internal.aliyuncs.com" : ".aliyuncs.com";
            var method = useHttps ? "https://" : "http://";

            var bucket = new BucketInfo()
            {
                IsCname = false,
                BucketName = bucketName,
                IsHttps = useHttps
            };

            bucket.EndpointUri = new Uri(method + region + baseDomain);

            // bucket名称为空的情况，直接访问oss
            bucket.BucketUri = String.IsNullOrEmpty(bucketName)? bucket.EndpointUri : new Uri(method + bucketName + "." + region + baseDomain);

            return bucket;
        }


        public string MakeResourcePathForSign(string key)
        {
            // https://help.aliyun.com/document_detail/31951.html?spm=5176.doc31950.6.384.DnW8Hi
            // 放入要访问的OSS资源：“ /BucketName/ObjectName”（无ObjectName则CanonicalizedResource为”/BucketName/“，如果同时也没有BucketName则为“/”） 

            if (String.IsNullOrEmpty(BucketName))
            {
                return "/";
            }

            return $"/{BucketName}/{key}";

        }

        /// <summary>
        /// 指定对象key获取访问对象的URL
        /// </summary>
        /// <param name="objectKey"></param>
        /// <returns></returns>
        public string GetObjectUrl(string objectKey)
        {
            var bucketUrl = BucketUri.ToString();

            if (String.IsNullOrEmpty(objectKey))
            {
                return bucketUrl;
            }

            if (bucketUrl.EndsWith("/"))
            {
                return bucketUrl + OssUtils.UrlEncodeKey(objectKey);
            }
            else
            {
                return bucketUrl + "/" + OssUtils.UrlEncodeKey(objectKey);
            }
        }
    }
}
