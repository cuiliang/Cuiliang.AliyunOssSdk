using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Cuiliang.AliyunOssSdk.Api.Bucket.Get
{
    /// <summary>
    /// 返回bucket里的对象
    /// https://help.aliyun.com/document_detail/31965.html?spm=a2c4g.11186623.6.1047.6c233ba9EwUHgb
    /// </summary>
    [XmlRoot("ListBucketResult")]
    public class GetBucketResult
    {
        /// <summary>
        /// 保存每个返回Object meta的容器。
        /// </summary>
        [XmlArrayItem("Contents")]
        public IList<ObjectMeta> Contents { get; set; }

        /// <summary>
        /// 如果请求中指定了delimiter参数，则在OSS返回的响应中包含CommonPrefixes元素。该元素标明那些以delimiter结尾，并有共同前缀的object名称的集合。
        /// </summary>
        public string CommonPrefixes { get; set; }

        /// <summary>
        /// 是一个用于对Object名字进行分组的字符。所有名字包含指定的前缀且第一次出现delimiter字符之间的object作为一组元素CommonPrefixes。
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// 	指明返回结果中编码使用的类型。如果请求的参数中指定了encoding-type，那会对返回结果中的Delimiter、Marker、Prefix、NextMarker和Key这些元素进行编码。
        /// </summary>
        public string EncodingType { get; set; }

        /// <summary>
        /// 指明是否所有的结果都已经返回； “true”表示本次没有返回全部结果；“false”表示本次已经返回了全部结果。
        /// </summary>
        public string IsTruncated { get; set; }

        /// <summary>
        /// 标明这次Get Bucket（List Object）的起点。
        /// </summary>
        public string Marker { get; set; }

        /// <summary>
        /// 响应请求内返回结果的最大数目。
        /// </summary>
        public string MaxKeys { get; set; }

        /// <summary>
        /// Bucket名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 保存Bucket拥有者信息的容器。
        /// </summary>
        public OwnerClass Owner
        {
            get; set;
        }

        /// <summary>
        /// 本次查询结果的开始前缀。
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Content里的每个元素，保存每个Object对象的信息。
        /// </summary>
        [XmlRoot("Contents")]
        public class ObjectMeta
        {
            /// <summary>
            /// 保存Bucket拥有者信息的容器。
            /// </summary>
            [XmlElement("Owner")]
            public OwnerClass Owner
            {
                get; set;
            }

            /// <summary>
            /// ETag (entity tag) 在每个Object生成的时候被创建，用于标示一个Object的内容。对于Put Object请求创建的Object，ETag值是其内容的MD5值；对于其他方式创建的Object，ETag值是其内容的UUID。ETag值可以用于检查Object内容是否发生变化。不建议用户使用ETag来作为Object内容的MD5校验数据完整性。
            /// </summary>
            public string ETag { get; set; }

            /// <summary>
            /// Object的Key.
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// Object最后被修改的时间。
            /// </summary>
            public string LastModified { get; set; }


            /// <summary>
            /// Object的字节数。
            /// </summary>
            public string Size { get; set; }

            /// <summary>
            /// Object的存储类型，目前只能是“Standard”类。
            /// </summary>
            public string StorageClass { get; set; }
        }

        [XmlRoot("Owner")]
        public class OwnerClass
        {
            /// <summary>
            /// Object 拥有者的名字。
            /// </summary>
            public string DisplayName { get; set; }

            /// <summary>
            /// Bucket拥有者的用户ID。
            /// </summary>
            public string ID { get; set; }
        }
    }
}
