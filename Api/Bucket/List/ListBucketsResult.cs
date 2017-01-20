/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Cuiliang.AliyunOssSdk.Api.Common;

namespace Cuiliang.AliyunOssSdk.Api.Bucket.List
{
    /// <summary>
    /// 列举Bucket的请求结果。
    /// </summary>
    [XmlRoot("ListAllMyBucketsResult")]
    public class ListBucketsResult
    {
        /// <summary>
        /// 获取一个值，限定返回的<see cref="Bucket" />的Key必须以该值作为前缀。
        /// </summary>
        public string Prefix { get;  set; }

        /// <summary>
        /// 获取一个值，用户设定结果从该值之后按字母排序的第一个开始返回。
        /// </summary>
        public string Marker { get;  set; }

        /// <summary>
        /// 获取一个值，用于限定此次返回bucket的最大数。
        /// 如果不设定，默认为100。
        /// </summary>
        public int? MaxKeys { get;  set; }

        /// <summary>
        /// 获取一个值，指明是否所有的结果都已经返回。
        /// </summary>
        public bool? IsTruncated { get;  set; }

        /// <summary>
        /// 获取一个值，指明下一个Marker。
        /// </summary>
        public string NextMaker { get;  set; }

        [XmlElement("Owner")]
        public Owner Owner { get; set; }

        /// <summary>
        /// 获取一个值，指明Bucket请求列表。
        /// </summary>
        [XmlArrayItem("Bucket")]
        public List<Common.Bucket> Buckets { get;  set; }
    }

    [XmlRoot("Bucket")]
    public class BucketModel
    {
        [XmlElement("Location")]
        public string Location { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("CreationDate")]
        public DateTime CreationDate { get; set; }

        public string ExtranetEndpoint { get; set; }

        public string IntranetEndpoint { get; set; }
    }

    
}
