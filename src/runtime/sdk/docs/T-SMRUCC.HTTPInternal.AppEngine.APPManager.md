---
title: APPManager
---

# APPManager
_namespace: [SMRUCC.HTTPInternal.AppEngine](N-SMRUCC.HTTPInternal.AppEngine.html)_





### Methods

#### __defaultFailure
```csharp
SMRUCC.HTTPInternal.AppEngine.APPManager.__defaultFailure(System.String,System.String,System.String@)
```
默认是API执行失败

|Parameter Name|Remarks|
|--------------|-------|
|api|-|
|args|-|
|out|-|


#### Invoke
```csharp
SMRUCC.HTTPInternal.AppEngine.APPManager.Invoke(System.String,System.String@)
```


|Parameter Name|Remarks|
|--------------|-------|
|url|-|
|result|HTML输出页面或者json数据|


#### InvokePOST
```csharp
SMRUCC.HTTPInternal.AppEngine.APPManager.InvokePOST(System.String,SMRUCC.HTTPInternal.AppEngine.POSTParser.PostReader,System.String@)
```


|Parameter Name|Remarks|
|--------------|-------|
|url|-|
|inputs|-|
|result|HTML输出页面或者json数据|


#### Register``1
```csharp
SMRUCC.HTTPInternal.AppEngine.APPManager.Register``1(``0)
```
向开放平台之中注册API接口

|Parameter Name|Remarks|
|--------------|-------|
|application|-|



### Properties

#### DefaultAPI
当WebApp查找失败的时候所执行的默认的API函数
#### RunningAPP
键名要求是小写的
