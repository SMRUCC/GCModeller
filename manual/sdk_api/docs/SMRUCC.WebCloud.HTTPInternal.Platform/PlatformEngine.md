# PlatformEngine
_namespace: [SMRUCC.WebCloud.HTTPInternal.Platform](./index.md)_

服务基础类，REST API的开发需要引用当前的项目



### Methods

#### #ctor
```csharp
SMRUCC.WebCloud.HTTPInternal.Platform.PlatformEngine.#ctor(System.String,System.Int32,System.Boolean,System.String,System.Int32,System.Boolean)
```
Init engine.

|Parameter Name|Remarks|
|--------------|-------|
|port|-|
|root|html wwwroot|
|nullExists|-|
|appDll|Must have a Class object implements the type @``T:SMRUCC.WebCloud.HTTPInternal.AppEngine.WebApp``|


#### __handleREST
```csharp
SMRUCC.WebCloud.HTTPInternal.Platform.PlatformEngine.__handleREST(SMRUCC.WebCloud.HTTPInternal.Core.HttpProcessor)
```
GET

|Parameter Name|Remarks|
|--------------|-------|
|p|-|


#### __init
```csharp
SMRUCC.WebCloud.HTTPInternal.Platform.PlatformEngine.__init(System.String)
```
Scanning the dll file and then load the @``T:SMRUCC.WebCloud.HTTPInternal.AppEngine.WebApp`` content.

|Parameter Name|Remarks|
|--------------|-------|
|dll|-|


#### __runDll
```csharp
SMRUCC.WebCloud.HTTPInternal.Platform.PlatformEngine.__runDll(System.String)
```
Call sub main in the @``T:SMRUCC.WebCloud.HTTPInternal.AppEngine.WebApp`` dll

|Parameter Name|Remarks|
|--------------|-------|
|dll|-|



