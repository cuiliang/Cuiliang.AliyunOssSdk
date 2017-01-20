using System.Xml.Serialization;
using Cuiliang.AliyunOssSdk.Utility;

namespace Cuiliang.AliyunOssSdk.Api.Object.DeleteMultiple
{
    [XmlRoot("DeleteResult")]
    public class DeleteMultipleObjectsResult
    {
        private DeletedObject[] _keys;

        /// <summary>
        /// Deleted部分的解析和获取
        /// </summary>
        [XmlElement("Deleted")]
        public DeletedObject[] Keys
        {
            get
            {
                if (EncodingType == null)
                    return _keys;

                bool isUrlEncoding = EncodingType.ToLowerInvariant().Equals(HttpUtils.UrlEncodingType);
                foreach (var key in _keys)
                {
                    key.Key = isUrlEncoding ? HttpUtils.DecodeUri(key.Key) : key.Key;
                }
                return _keys;
            }
            set
            {
                this._keys = value;
            }
        }

        /// <summary>
        /// EncodingType值的解析和获取
        /// </summary>
        [XmlElement("EncodingType")]
        public string EncodingType { get; set; }

        public DeleteMultipleObjectsResult()
        {
        }

        /// <summary>
        /// Deleted部分的解析和获取
        /// </summary>
        [XmlRoot("Deleted")]
        public class DeletedObject
        {
            /// <summary>
            /// Deleted Key的解析和获取
            /// </summary>
            [XmlElement("Key")]
            public string Key { get; set; }
        }
    }
}
