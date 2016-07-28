---
title: APPEngine
---

# APPEngine
_namespace: [SMRUCC.HTTPInternal.AppEngine](N-SMRUCC.HTTPInternal.AppEngine.html)_

Engine for executes the API that defined in the @"T:SMRUCC.HTTPInternal.AppEngine.WebApp".
 (执行@"T:SMRUCC.HTTPInternal.AppEngine.WebApp"的工作引擎)



### Methods

#### GetHelp
```csharp
SMRUCC.HTTPInternal.AppEngine.APPEngine.GetHelp
```
Gets help page.

#### GetParameter
```csharp
SMRUCC.HTTPInternal.AppEngine.APPEngine.GetParameter(System.String,System.String@,System.String@,System.String@)
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
SMRUCC.HTTPInternal.AppEngine.APPEngine.Invoke(System.String,System.Collections.Generic.Dictionary{System.String,SMRUCC.HTTPInternal.AppEngine.APPEngine},System.String@,SMRUCC.HTTPInternal.AppEngine.APIMethods.APIAbstract)
```
分析url，然后查找相对应的WebApp，并进行数据请求的执行

|Parameter Name|Remarks|
|--------------|-------|
|url|-|
|applications|-|
|result|-|



### Properties

#### API
必须按照从长到短来排序
