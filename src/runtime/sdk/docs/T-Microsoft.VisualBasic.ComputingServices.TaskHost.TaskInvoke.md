---
title: TaskInvoke
---

# TaskInvoke
_namespace: [Microsoft.VisualBasic.ComputingServices.TaskHost](N-Microsoft.VisualBasic.ComputingServices.TaskHost.html)_





### Methods

#### #ctor
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.TaskInvoke.#ctor(System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|port|You can suing function @"M:Microsoft.VisualBasic.Net.TCPExtensions.GetFirstAvailablePort(System.Int32)" to initialize this server object.|


#### __invoke
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.TaskInvoke.__invoke(Microsoft.VisualBasic.ComputingServices.TaskHost.InvokeInfo,System.Type@)
```
A common function of invoke the method on the remote machine

|Parameter Name|Remarks|
|--------------|-------|
|params|远程主机上面的函数指针|
|value|value's @"T:System.Type"|


#### Invoke
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.TaskInvoke.Invoke(Microsoft.VisualBasic.ComputingServices.TaskHost.InvokeInfo)
```
Invoke the function on the remote server.(远程服务器上面通过这个方法执行函数调用)

|Parameter Name|Remarks|
|--------------|-------|
|params|-|


#### InvokeLinq
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.TaskInvoke.InvokeLinq(System.Int64,Microsoft.VisualBasic.Net.Protocols.RequestStream,System.Net.IPEndPoint)
```
执行远程Linq代码

|Parameter Name|Remarks|
|--------------|-------|
|CA|SSL证书编号|
|args|-|
|remote|-|



