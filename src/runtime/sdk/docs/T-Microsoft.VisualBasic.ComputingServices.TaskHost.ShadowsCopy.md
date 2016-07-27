---
title: ShadowsCopy
---

# ShadowsCopy
_namespace: [Microsoft.VisualBasic.ComputingServices.TaskHost](N-Microsoft.VisualBasic.ComputingServices.TaskHost.html)_





### Methods

#### __innerCopy
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.ShadowsCopy.__innerCopy(System.Object,System.Object,Microsoft.VisualBasic.ComputingServices.TaskHost.MemoryHash,Microsoft.VisualBasic.List{System.Int64})
```


|Parameter Name|Remarks|
|--------------|-------|
|from|-|
|target|远程服务器上面的对象|
|memory|远程对象的内存管理模块|
|stack|Avoided of the loop reference.(内存管理的复制堆栈记录)|


#### ShadowsCopy
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.ShadowsCopy.ShadowsCopy(System.Object,System.Object,Microsoft.VisualBasic.ComputingServices.TaskHost.MemoryHash)
```
将客户端上面的对象数据复制到远程主机上面的内存管理模块之中

|Parameter Name|Remarks|
|--------------|-------|
|from|Client上面的|
|target|服务器上面的|
|memory|内存管理模块单元|



