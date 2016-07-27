---
title: HttpFileSystem
---

# HttpFileSystem
_namespace: [SMRUCC.HTTPInternal.Core](N-SMRUCC.HTTPInternal.Core.html)_

不兼容IE和Edge浏览器???为什么会这样子？



### Methods

#### #ctor
```csharp
SMRUCC.HTTPInternal.Core.HttpFileSystem.#ctor(System.Int32,System.String,System.Boolean,SMRUCC.HTTPInternal.Core.HttpFileSystem.IGetResource)
```


|Parameter Name|Remarks|
|--------------|-------|
|port|-|
|root|-|
|nullExists|-|


#### __getMapDIR
```csharp
SMRUCC.HTTPInternal.Core.HttpFileSystem.__getMapDIR(System.String@)
```
长

|Parameter Name|Remarks|
|--------------|-------|
|res|-|


#### __handleREST
```csharp
SMRUCC.HTTPInternal.Core.HttpFileSystem.__handleREST(SMRUCC.HTTPInternal.Core.HttpProcessor)
```
handle the GET/POST request at here

|Parameter Name|Remarks|
|--------------|-------|
|p|-|


#### __transferData
```csharp
SMRUCC.HTTPInternal.Core.HttpFileSystem.__transferData(SMRUCC.HTTPInternal.Core.HttpProcessor,System.String,System.Byte[])
```
为什么不需要添加content-type说明？？

|Parameter Name|Remarks|
|--------------|-------|
|p|-|
|ext|-|
|buf|-|


#### handleGETRequest
```csharp
SMRUCC.HTTPInternal.Core.HttpFileSystem.handleGETRequest(SMRUCC.HTTPInternal.Core.HttpProcessor)
```
为什么不需要添加@"M:SMRUCC.HTTPInternal.Core.HttpProcessor.writeSuccess(System.String)"方法？？？

|Parameter Name|Remarks|
|--------------|-------|
|p|-|



### Properties

#### _virtualMappings
{url, mapping path}
#### Page404
默认的404页面
#### RequestStream
默认是使用@"M:SMRUCC.HTTPInternal.Core.HttpFileSystem.GetResource(System.String@)"获取得到文件数据
