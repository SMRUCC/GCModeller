---
title: HttpServer
---

# HttpServer
_namespace: [SMRUCC.HTTPInternal.Core](N-SMRUCC.HTTPInternal.Core.html)_

Internal http server core.



### Methods

#### #ctor
```csharp
SMRUCC.HTTPInternal.Core.HttpServer.#ctor(System.Int32,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|port|The network data port of this internal http server listen.|
|homeShowOnStart|-|


#### __httpProcessor
```csharp
SMRUCC.HTTPInternal.Core.HttpServer.__httpProcessor(System.Net.Sockets.TcpClient)
```
New HttpProcessor(Client, Me) with {._404Page = "...."}

|Parameter Name|Remarks|
|--------------|-------|
|client|-|


#### handleGETRequest
```csharp
SMRUCC.HTTPInternal.Core.HttpServer.handleGETRequest(SMRUCC.HTTPInternal.Core.HttpProcessor)
```


|Parameter Name|Remarks|
|--------------|-------|
|p|-|


#### Run
```csharp
SMRUCC.HTTPInternal.Core.HttpServer.Run
```
Running this http server. 
 NOTE: current thread will be blocked at here until the server core is shutdown. 
 (请注意，在服务器开启之后，当前的线程会被阻塞在这里)

#### Shutdown
```csharp
SMRUCC.HTTPInternal.Core.HttpServer.Shutdown
```
Shutdown this internal http server


### Properties

#### IsRunning
Indicates this http server is running status or not.
#### LocalPort
The network data port of this internal http server listen.
