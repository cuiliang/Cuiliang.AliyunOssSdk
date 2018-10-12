# 概述
这是一个面向dotnet core的简化版的oss sdk，目前只包含部分的api支持。

相对于官方sdk，很大程度简化了代码，支持async模式。

nuget: https://www.nuget.org/packages/Cuiliang.AliyunOssSdk/

# 说明

## 使用方法

在appSettings.json中配置参数
```
{
    "ossClient": {
    "AccessKeyId": "您的AccessKeyId",
    "AccessKeySecret": "您的AccessKeySecret"
}
```

在ConfigureServices中注册
```
public void ConfigureServices(IServiceCollection services)
{
    ~~~
    services.AddOssClient(Configuration);
    ~~~
}
```

然后在需要的时候注入OssClient对象即可。
```
// 注入变量
public OssManager(OssClient ossClient)
{
     client = ossClient;
    ~~~~
}



//调用api
    
//list buckets
var listBucketResult = await client.ListBucketsAsync(OssRegions.ShangHai);
Console.WriteLine(listBucketResult.IsSuccess + ":" + listBucketResult.ErrorMessage);
var bucket = BucketInfo.CreateByRegion("oss-cn-shanghai.aliyuncs.com", "bucket", false, false);

// save string to a file
string content = "这是一个文本文件\naaaaaaaa\nbbbbbb\nccccccccc";
var putResult = await client.PutObjectAsync(bucket, "test_put_object_string.txt", content);
Console.WriteLine($"Put string object  {putResult.IsSuccess} {putResult.ErrorMessage}  Etag:{putResult.SuccessResult?.ETag}");
    

```


## 主要类说明


OssResult<TResult> ：命令返回结果的通用封装
ErrorResult：API调用失败时的返回结果对象（由OSS返回的。）

XXXXCommand 表示一个Oss API 命令，
BaseOssCommand: Oss API 命令基类。

ServiceRequest：封装了一个需要发送给oss的http请求。包含headers/parameters等参数的存储 。

OssClient: 对API调用的封装，类似于一个api的索引，方便调用。

对每个API调用，假设API为XXX，则一般是定义一个XXXXCommand类，此类直接或简介继承于BaseOssCommand类，
提供构建ServiceRequest对象的逻辑以及根据返回的HttpResponse解析返回结果的逻辑代码。

## 如何增加新的API
如果你希望增加新的API，可以在合适的目录中定义 ApiNameCommand和ApiNameResult类型，并且在OssClient中增加相应的调用代码即可。


## 变更历史
0.3.0 
    加入GetBucket API支持，用于获取文件列表
    合并cnblogs fork代码
    改为使用DI注册而不是直接创建对象
