---
title: Extensions
---

# Extensions
_namespace: [Microsoft.VisualBasic.ComputingServices.TaskHost](N-Microsoft.VisualBasic.ComputingServices.TaskHost.html)_





### Methods

#### AddressOf
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.Extensions.AddressOf(System.Type,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|type|-|
|name|NameOf|


#### Invoke``1
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.Extensions.Invoke``1(Microsoft.VisualBasic.ComputingServices.TaskHost.InvokeInfo,Microsoft.VisualBasic.ComputingServices.TaskHost.TaskHost)
```
DirectCast

|Parameter Name|Remarks|
|--------------|-------|
|info|-|
|host|-|



### Properties

#### _local
Services running on a LAN?
#### EnvironmentLocal
假若这个参数为真，则说明服务只是运行在局域网之中，则只会返回局域网的IP地址
 假若为假，则说明服务是运行在互联网上面的，则会查询主机的公共IP地址，调试的时候建议设置为真
