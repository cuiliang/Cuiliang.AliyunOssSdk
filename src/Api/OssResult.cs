using System;
using Cuiliang.AliyunOssSdk.Entites;

namespace Cuiliang.AliyunOssSdk.Api
{
    /// <summary>
    /// Oss 命令返回结果的通用封装
    /// </summary>
    /// <typeparam name="TResult">成功返回时的结果对象</typeparam>
    public class OssResult<TResult>
    {
        public OssResult()
        {
            
        }

        /// <summary>
        /// 指定成功消息的构造函数
        /// </summary>
        /// <param name="result"></param>
        public OssResult(TResult result)
        {
            IsSuccess = true;
            SuccessResult = result;
        }

        /// <summary>
        /// 指定是否成功的构造函数
        /// </summary>
        /// <param name="success"></param>
        public OssResult(bool success)
        {
            IsSuccess = success;
        }

        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 内部异常
        /// </summary>
        public Exception InnerException { get; set; }

        /// <summary>
        /// 成功执行的结果
        /// </summary>
        public TResult SuccessResult { get; set; }

        /// <summary>
        /// 错误时返回的结果
        /// </summary>
        public ErrorResult ErrorResult { get; set; }
    }
}
