---
title: FileURI
---

# FileURI
_namespace: [Microsoft.VisualBasic.ComputingServices.FileSystem.Protocols](N-Microsoft.VisualBasic.ComputingServices.FileSystem.Protocols.html)_

Represents a local file or remote file its location on the network.
 (表示一个本地文件或者网络上面的文件的位置)



### Methods

#### FileStreamParser
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.Protocols.FileURI.FileStreamParser(System.String,Microsoft.VisualBasic.Net.IPEndPoint@,Microsoft.VisualBasic.ComputingServices.FileSystem.Protocols.FileHandle@)
```
port@remote_IP://hash+<path>

|Parameter Name|Remarks|
|--------------|-------|
|uri|-|
|remote|-|
|handle|-|


#### ToString
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.Protocols.FileURI.ToString
```
{port}@{ipaddress}://C:\xxx\xxx.file


### Properties

#### EntryPoint
The services portal of this remote filesystem object.
 (远程文件服务对象的服务接口)
#### File
The reference location of this file system object on the target machine.
 (目标机器(@"P:Microsoft.VisualBasic.ComputingServices.FileSystem.Protocols.FileURI.EntryPoint")上面的文件系统的引用的位置)
#### IsLocal
Is this file system object is a local file?.(是否为本地文件)
