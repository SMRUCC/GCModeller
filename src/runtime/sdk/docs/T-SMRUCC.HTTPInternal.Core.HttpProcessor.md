---
title: HttpProcessor
---

# HttpProcessor
_namespace: [SMRUCC.HTTPInternal.Core](N-SMRUCC.HTTPInternal.Core.html)_

这个对象包含有具体的http request的处理方法



### Methods

#### HandlePOSTRequest
```csharp
SMRUCC.HTTPInternal.Core.HttpProcessor.HandlePOSTRequest
```
This post data processing just reads everything into a memory stream.
 this is fine for smallish things, but for large stuff we should really
 hand an input stream to the request processor. However, the input stream 
 we hand him needs to let him see the "end of the stream" at this content 
 length, because otherwise he won't know when he's seen it all!

#### writeFailure
```csharp
SMRUCC.HTTPInternal.Core.HttpProcessor.writeFailure(System.String)
```
404


### Properties

#### _404Page
You can customize your 404 error page at here.
#### http_url
File location or GET/POST request arguments
#### IsWWWRoot
If current request url is indicates the HTTP root: index.html
#### MAX_POST_SIZE
10MB
#### Out
可以向这里面写入数据从而回传数据
