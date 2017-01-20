using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cuiliang.AliyunOssSdk.Utility
{
    public class Ensure
    {
        /// <summary>
        /// 参数不为空
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="argName"></param>
        public static void NotNull(object argValue, string argName)
        {
            if (argValue == null)
            {
                throw new ArgumentNullException($"{argName} 不应为空！");
            }
        }

        public static void NotEqZero(long argValue, string argName)
        {
            if (argValue == 0)
            {
                throw new ArgumentNullException($"{argName} 不应为0！");
            }
        }

        /// <summary>
        /// 验证某个条件为True
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        public static void ToBeTrue(bool value, string message = null)
        {
            if (!value)
            {

                throw new ArgumentException(string.IsNullOrWhiteSpace(message) ? "数据不符合要求" : message);
            }
        }

        public static void NotEmpty(string value, string paramName)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentOutOfRangeException($"参数 {paramName} 不应该为空.");
            }
        }

        public static void UriNotEmpty(Uri uri, string paramName)
        {
            if (uri == null || String.IsNullOrEmpty(uri.Host))
            {
                throw new ArgumentOutOfRangeException($"参数 {paramName} 不应该为空.");
            }
        }
    }
}
