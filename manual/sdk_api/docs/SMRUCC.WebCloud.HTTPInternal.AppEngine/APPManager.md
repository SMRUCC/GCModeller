# APPManager
_namespace: [SMRUCC.WebCloud.HTTPInternal.AppEngine](./index.md)_

Help document developer user manual page



### Methods

#### __defaultFailure
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.APPManager.__defaultFailure(System.String,SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments.HttpRequest,SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments.HttpResponse)
```
默认是API执行失败

|Parameter Name|Remarks|
|--------------|-------|
|api|-|


#### GetApp``1
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.APPManager.GetApp``1
```
Get running app by type.

#### Invoke
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.APPManager.Invoke(SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments.HttpRequest,SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments.HttpResponse)
```


|Parameter Name|Remarks|
|--------------|-------|
|response|HTML输出页面或者json数据|


#### InvokePOST
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.APPManager.InvokePOST(SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments.HttpPOSTRequest,SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments.HttpResponse)
```


|Parameter Name|Remarks|
|--------------|-------|
|response|HTML输出页面或者json数据|


#### Register``1
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.APPManager.Register``1(``0)
```
向开放平台之中注册API接口

|Parameter Name|Remarks|
|--------------|-------|
|application|-|



### Properties

#### baseUrl
生成帮助文档所需要的
#### DefaultAPI
当WebApp查找失败的时候所执行的默认的API函数
#### RunningAPP
键名要求是小写的
