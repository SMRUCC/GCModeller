# APPEngine
_namespace: [SMRUCC.WebCloud.HTTPInternal.AppEngine](./index.md)_

Engine for executes the API that defined in the @``T:SMRUCC.WebCloud.HTTPInternal.AppEngine.WebApp``.
 (执行@``T:SMRUCC.WebCloud.HTTPInternal.AppEngine.WebApp``的工作引擎)



### Methods

#### GetHelp
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.APPEngine.GetHelp
```
Gets help page.

#### GetParameter
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.APPEngine.GetParameter(System.String,System.String@,System.String@,System.String@)
```
If returns false, which means this function can not parsing any arguments parameter from the input url.
 (返回False标识无法正确的解析出调用数据)

|Parameter Name|Remarks|
|--------------|-------|
|url|Url inputs from the user browser.|
|application|-|
|API|-|
|parameters|-|


#### Invoke
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.APPEngine.Invoke(SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments.HttpRequest,System.Collections.Generic.Dictionary{System.String,SMRUCC.WebCloud.HTTPInternal.AppEngine.APPEngine},SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments.HttpResponse,SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.APIAbstract)
```
分析url，然后查找相对应的WebApp，并进行数据请求的执行


### Properties

#### API
必须按照从长到短来排序
