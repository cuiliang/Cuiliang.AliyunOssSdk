# 概述
这是一个简化版的oss sdk，目前只包含少数的api支持。
部门代码从官方sdk移植，部分代码整体重构。
重构的目的主要是为了简化代码，支持async模式。

# 主要类说明

## API 接口实现


OssResult<TResult> ：命令返回结果的通用封装
ErrorResult：API调用失败时的返回结果对象（由OSS返回的。）

XXXXCommand 表示一个Oss API 命令，
BaseOssCommand: Oss API 命令基类。

ServiceRequest：封装了一个需要发送给oss的http请求。包含headers/parameters等参数的存储 。

OssClient: 对API调用的封装，类似于一个api的索引，方便调用。

对每个API调用，假设API为XXX，则一般是定义一个XXXXCommand类，此类直接或简介继承于BaseOssCommand类，
提供构建ServiceRequest对象的逻辑以及根据返回的HttpResponse解析返回结果的逻辑代码。

