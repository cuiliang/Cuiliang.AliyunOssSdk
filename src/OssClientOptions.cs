namespace Cuiliang.AliyunOssSdk
{
    public class OssClientOptions
    {
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public string SecurityToken { get; internal set; }
    }
}