using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Cuiliang.AliyunOssSdk.Utility
{
    /// <summary>
    /// XML序列化和反序列化处理
    /// </summary>
    public static class SerializeHelper
    {
        /// <summary>
        /// XML -》 对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlStream"></param>
        /// <returns></returns>
        public static T Deserialize<T>(Stream xmlStream)
        {
            var serializer = new XmlSerializer(typeof(T));


            return (T)serializer.Deserialize(xmlStream);

        }

        /// <summary>
        /// 将一个对象序列化为XML字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize<T>(T obj)
        {
            if (obj == null)
            {
                return String.Empty;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));


            using (StringWriter textWriter = new StringWriter())
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);

                serializer.Serialize(textWriter, obj, namespaces);
                return textWriter.ToString();
            }

            //MemoryStream stream = null;
            //try
            //{
            //    stream = new MemoryStream();
            //    var namespaces = new XmlSerializerNamespaces();
            //    namespaces.Add(string.Empty, string.Empty);
            //    Serializer.Serialize(stream, obj, namespaces);
            //    stream.Seek(0, SeekOrigin.Begin);
            //    return stream;
            //}
            //catch (InvalidOperationException ex)
            //{
            //    if (stream != null)
            //        stream.Dispose();

            //    throw new RequestSerializationException(ex.Message, ex);
            //}

        }
    }
}
