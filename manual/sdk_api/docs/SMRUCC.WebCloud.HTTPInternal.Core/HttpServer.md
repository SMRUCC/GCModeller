# HttpServer
_namespace: [SMRUCC.WebCloud.HTTPInternal.Core](./index.md)_

Internal http server core.



### Methods

#### #ctor
```csharp
SMRUCC.WebCloud.HTTPInternal.Core.HttpServer.#ctor(System.Int32,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|port|The network data port of this internal http server listen.|


#### __httpProcessor
```csharp
SMRUCC.WebCloud.HTTPInternal.Core.HttpServer.__httpProcessor(System.Net.Sockets.TcpClient)
```
New HttpProcessor(Client, Me) with {._404Page = "...."}

|Parameter Name|Remarks|
|--------------|-------|
|client|-|


#### getProcessor
```csharp
SMRUCC.WebCloud.HTTPInternal.Core.HttpServer.getProcessor(System.Net.Sockets.TcpClient)
```
一些初始化的设置在这里

|Parameter Name|Remarks|
|--------------|-------|
|client|-|


#### handleGETRequest
```csharp
SMRUCC.WebCloud.HTTPInternal.Core.HttpServer.handleGETRequest(SMRUCC.WebCloud.HTTPInternal.Core.HttpProcessor)
```


|Parameter Name|Remarks|
|--------------|-------|
|p|-|


#### Run
```csharp
SMRUCC.WebCloud.HTTPInternal.Core.HttpServer.Run
```
Running this http server. 
 NOTE: current thread will be blocked at here until the server core is shutdown. 
 (请注意，在服务器开启之后，当前的线程会被阻塞在这里)

#### Shutdown
```csharp
SMRUCC.WebCloud.HTTPInternal.Core.HttpServer.Shutdown
```
Shutdown this internal http server


### Properties

#### _threadPool
处理连接的线程池
#### IsRunning
Indicates this http server is running status or not.
#### LocalPort
The network data port of this internal http server listen.
