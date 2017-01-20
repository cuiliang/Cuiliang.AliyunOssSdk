using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Cuiliang.AliyunOssSdk.Api.Common.Consts;

namespace Cuiliang.AliyunOssSdk.Api.Object.Get
{
    /// <summary>
    /// 指定要Get对象的范围
    /// </summary>
    public class ObjectRange
    {
        public long Start { get; set; }
        public long End { get; set; }

        public long Total { get; set; }

        public bool IsValid()
        {
            return Start >= 0 || End >= 0;
        }

        // 添加到http头中
        public void AddToHeader(IDictionary<string, string> headers)
        {
            if (!IsValid())
                return;

            var sb = new StringBuilder();
            sb.Append("bytes=");
            if (Start >= 0)
                sb.Append(Start.ToString(CultureInfo.InvariantCulture));
            sb.Append("-");
            if (End >= 0)
                sb.Append(End.ToString(CultureInfo.InvariantCulture));

            headers.Add(HttpHeaders.Range, sb.ToString());
        }
    }
}
