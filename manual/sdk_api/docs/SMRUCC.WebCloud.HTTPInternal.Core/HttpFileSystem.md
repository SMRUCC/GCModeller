# HttpFileSystem
_namespace: [SMRUCC.WebCloud.HTTPInternal.Core](./index.md)_

不兼容IE和Edge浏览器???为什么会这样子？



### Methods

#### #ctor
```csharp
SMRUCC.WebCloud.HTTPInternal.Core.HttpFileSystem.#ctor(System.Int32,System.String,System.Boolean,SMRUCC.WebCloud.HTTPInternal.Core.HttpFileSystem.IGetResource,System.Int32,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|port|-|
|root|-|
|nullExists|-|


#### __getMapDIR
```csharp
SMRUCC.WebCloud.HTTPInternal.Core.HttpFileSystem.__getMapDIR(System.String@)
```
长

|Parameter Name|Remarks|
|--------------|-------|
|res|-|


#### __handleREST
```csharp
SMRUCC.WebCloud.HTTPInternal.Core.HttpFileSystem.__handleREST(SMRUCC.WebCloud.HTTPInternal.Core.HttpProcessor)
```
handle the GET/POST request at here

|Parameter Name|Remarks|
|--------------|-------|
|p|-|


#### __transferData
```csharp
SMRUCC.WebCloud.HTTPInternal.Core.HttpFileSystem.__transferData(SMRUCC.WebCloud.HTTPInternal.Core.HttpProcessor,System.String,System.Byte[],System.String)
```
为什么不需要添加content-type说明？？

|Parameter Name|Remarks|
|--------------|-------|
|p|-|
|ext|-|
|buf|-|


#### GetResource
```csharp
SMRUCC.WebCloud.HTTPInternal.Core.HttpFileSystem.GetResource(System.String@)
```
默认是获取文件数据

|Parameter Name|Remarks|
|--------------|-------|
|res|-|


#### handleGETRequest
```csharp
SMRUCC.WebCloud.HTTPInternal.Core.HttpFileSystem.handleGETRequest(SMRUCC.WebCloud.HTTPInternal.Core.HttpProcessor)
```
为什么不需要添加@``M:SMRUCC.WebCloud.HTTPInternal.Core.HttpProcessor.writeSuccess(System.String)``方法？？？

|Parameter Name|Remarks|
|--------------|-------|
|p|-|


#### MapPath
```csharp
SMRUCC.WebCloud.HTTPInternal.Core.HttpFileSystem.MapPath(System.String@)
```
Maps the http request url as server file system path.

|Parameter Name|Remarks|
|--------------|-------|
|res|-|


#### SetGetRequest
```csharp
SMRUCC.WebCloud.HTTPInternal.Core.HttpFileSystem.SetGetRequest(SMRUCC.WebCloud.HTTPInternal.Core.HttpFileSystem.IGetResource)
```
Public Delegate Function @``T:SMRUCC.WebCloud.HTTPInternal.Core.HttpFileSystem.IGetResource``(ByRef res As @``T:System.String``) As @``T:System.Byte``

|Parameter Name|Remarks|
|--------------|-------|
|req|-|



### Properties

#### _virtualMappings
{url, mapping path}
#### NoData
``[ERR_EMPTY_RESPONSE::No data send]``
#### Page404
默认的404页面
#### RequestStream
默认是使用@``M:SMRUCC.WebCloud.HTTPInternal.Core.HttpFileSystem.GetResource(System.String@)``获取得到文件数据
