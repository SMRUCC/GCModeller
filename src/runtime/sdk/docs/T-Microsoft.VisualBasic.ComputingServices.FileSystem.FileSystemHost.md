---
title: FileSystemHost
---

# FileSystemHost
_namespace: [Microsoft.VisualBasic.ComputingServices.FileSystem](N-Microsoft.VisualBasic.ComputingServices.FileSystem.html)_





### Methods

#### #ctor
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystemHost.#ctor(System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|port|-|


#### CopyDirectory
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystemHost.CopyDirectory(System.Int64,Microsoft.VisualBasic.Net.Protocols.RequestStream,System.Net.IPEndPoint)
```
Copies the contents of a directory to another directory.

|Parameter Name|Remarks|
|--------------|-------|
|CA|-|
|args|-|
|remote|-|


#### CurrentDirectory
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystemHost.CurrentDirectory(System.Int64,Microsoft.VisualBasic.Net.Protocols.RequestStream,System.Net.IPEndPoint)
```
Gets or sets the current directory.

|Parameter Name|Remarks|
|--------------|-------|
|CA|-|
|args|-|
|remote|-|

_returns: The current directory for file I/O operations._

#### Drives
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystemHost.Drives(System.Int64,Microsoft.VisualBasic.Net.Protocols.RequestStream,System.Net.IPEndPoint)
```
Returns a read-only collection of all available drive names.

|Parameter Name|Remarks|
|--------------|-------|
|CA|-|
|args|-|
|remote|-|

_returns: A read-only collection of all available drives as System.IO.DriveInfo objects._


### Properties

#### OpenedHandles
远程服务器上面已经打开的文件句柄
