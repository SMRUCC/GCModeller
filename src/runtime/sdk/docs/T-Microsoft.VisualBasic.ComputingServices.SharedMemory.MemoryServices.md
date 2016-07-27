---
title: MemoryServices
---

# MemoryServices
_namespace: [Microsoft.VisualBasic.ComputingServices.SharedMemory](N-Microsoft.VisualBasic.ComputingServices.SharedMemory.html)_

Shared the memory with the remote machine.



### Methods

#### #ctor
```csharp
Microsoft.VisualBasic.ComputingServices.SharedMemory.MemoryServices.#ctor(Microsoft.VisualBasic.Net.IPEndPoint,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|remote|-|
|local|local services port that provides to the remote|


#### GetValue``1
```csharp
Microsoft.VisualBasic.ComputingServices.SharedMemory.MemoryServices.GetValue``1(System.String)
```
Gets the memory data from remote machine.

|Parameter Name|Remarks|
|--------------|-------|
|name|建议使用NameOf来设置或者获取参数值|


#### SetValue``1
```csharp
Microsoft.VisualBasic.ComputingServices.SharedMemory.MemoryServices.SetValue``1(System.String,``0)
```


|Parameter Name|Remarks|
|--------------|-------|
|name|建议使用NameOf来设置或者获取参数值|
|value|-|



