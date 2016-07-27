---
title: ILinqReader
---

# ILinqReader
_namespace: [Microsoft.VisualBasic.ComputingServices.TaskHost](N-Microsoft.VisualBasic.ComputingServices.TaskHost.html)_

Remote Linq source reader



### Methods

#### #ctor
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.ILinqReader.#ctor(Microsoft.VisualBasic.Net.IPEndPoint,System.Type)
```


|Parameter Name|Remarks|
|--------------|-------|
|portal|-|
|type|JSON反序列化所需要的类型信息|


#### __free
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.ILinqReader.__free
```
Automatically free the remote resource.(释放远程主机上面的资源)

#### AsQuerable
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.ILinqReader.AsQuerable
```
使用这个迭代器函数查询会自动重置远程的数据源的位置到初始位置

#### Moves
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.ILinqReader.Moves(System.Int32)
```
这个迭代器函数不会重置远程的数据源

|Parameter Name|Remarks|
|--------------|-------|
|n|迭代器所返回来的元素数量，当小于1的时候会被自动重置为1个元素|



### Properties

#### Portal
Remote entry point
#### Type
Element type in the source collection.
