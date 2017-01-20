using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cuiliang.AliyunOssSdk.Api.Object.DeleteMultiple
{
    /// <summary>
    /// 删除多个对象的消息体
    /// </summary>
    [XmlRoot("Delete")]
    public class DeleteObjectsRequestModel
    {
        [XmlElement("Quiet")]
        public bool Quiet { get; set; }

        [XmlElement("Object")]
        public ObjectToDel[] Keys { get; set; }

        [XmlRoot("Object")]
        public class ObjectToDel
        {
            [XmlElement("Key")]
            public string Key { get; set; }
        }

        public DeleteObjectsRequestModel()
        {
            
        }

        public DeleteObjectsRequestModel(bool quiet, IList<string> keys )
        {
            Quiet = quiet;
            var keyList = new List<ObjectToDel>();
            foreach (var k in keys)
            {
                keyList.Add(new ObjectToDel() {Key = k});
            }

            Keys = keyList.ToArray();
        }
    }
}
