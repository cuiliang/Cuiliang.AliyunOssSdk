namespace Cuiliang.AliyunOssSdk.Api.Object.Append
{
    public class AppentObjectResult
    {
        /// <summary>
        /// 获取一个值表示与Object相关的hex编码的128位MD5摘要。
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// 指明下一次请求应当提供的position。
        /// </summary>
        public long NextAppendPosition { get; set; }

        /// <summary>
        /// 表明Object的64位CRC值。该64位CRC根据ECMA-182标准计算得出。
        /// </summary>
        public ulong HashCrc64Ecma { get; set; }
    }
}
