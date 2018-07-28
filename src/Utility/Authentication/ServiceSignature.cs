using System;

namespace Cuiliang.AliyunOssSdk.Utility.Authentication
{
    public abstract class ServiceSignature
    {

        public abstract string SignatureMethod { get; }

        public abstract string SignatureVersion { get; }

        public string ComputeSignature(String key, String data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException(
                    "参数为空", "key");
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException(
                    "参数为空", "data");


            return ComputeSignatureCore(key, data);
        }

        protected abstract string ComputeSignatureCore(string key, string data);

        public static ServiceSignature Create()
        {
            return new HmacSHA1Signature();
        }

    }
}
