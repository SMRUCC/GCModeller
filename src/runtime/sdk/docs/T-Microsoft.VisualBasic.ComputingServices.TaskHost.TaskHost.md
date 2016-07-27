---
title: TaskHost
---

# TaskHost
_namespace: [Microsoft.VisualBasic.ComputingServices.TaskHost](N-Microsoft.VisualBasic.ComputingServices.TaskHost.html)_

Using this object to running the method on the remote machine.
 由于是远程调用，所以运行的环境可能会很不一样，所以在设计程序的时候请尽量避免或者不要使用模块变量，以免出现难以调查的BUG



### Methods

#### AsLinq``1
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.TaskHost.AsLinq``1(System.Delegate,System.Object[])
```
If your function pointer returns type is a collection, then using this method is recommended.
 (执行远程机器上面的代码，然后返回数据查询接口)

|Parameter Name|Remarks|
|--------------|-------|
|target|远程机器上面的函数指针|
|args|-|


#### Invoke
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.TaskHost.Invoke(System.Delegate,System.Object[])
```
Running the function delegate pointer on the remote machine. 
 
 *****************************************************************************************************
 NOTE: Performance issue, this is important! if the function pointer its returns type is a collection, 
 then you should using the method @"M:Microsoft.VisualBasic.ComputingServices.TaskHost.TaskHost.AsLinq``1(System.Delegate,System.Object[])" to running 
 your code on the remote. Or a large json data will be return back through network in one package, 
 this may cause a serious performance problem both on your server and your local client.
 (本地服务器通过这个方法调用远程主机上面的函数，假若目标函数的返回值类型是一个集合，
 请使用@"M:Microsoft.VisualBasic.ComputingServices.TaskHost.TaskHost.AsLinq``1(System.Delegate,System.Object[])"方法，否则集合之中的所有数据都将会一次性返回，这个可能会导致严重的性能问题)

|Parameter Name|Remarks|
|--------------|-------|
|target|-|
|args|-|


#### Shell
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.TaskHost.Shell(System.String,System.String)
```
Start the application on the remote host.(相当于Sub，调用远程的命令行程序，只会返回0或者错误代码)

|Parameter Name|Remarks|
|--------------|-------|
|exe|Exe file path|
|args|-|



