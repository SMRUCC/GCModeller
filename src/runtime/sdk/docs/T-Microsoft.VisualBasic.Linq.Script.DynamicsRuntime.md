---
title: DynamicsRuntime
---

# DynamicsRuntime
_namespace: [Microsoft.VisualBasic.Linq.Script](N-Microsoft.VisualBasic.Linq.Script.html)_

LINQ脚本数据源查询运行时环境



### Methods

#### Evaluate
```csharp
Microsoft.VisualBasic.Linq.Script.DynamicsRuntime.Evaluate(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|script|-|

_returns: 
 If the Return statement is presents, then the variable of the returns will be return from the function, and this is a Function in VisualBasic 
 If not, then viod value will be returns, and this is a Sub in VisualBasic
 _

#### EXEC
```csharp
Microsoft.VisualBasic.Linq.Script.DynamicsRuntime.EXEC(System.String)
```
执行单条查询表达式

|Parameter Name|Remarks|
|--------------|-------|
|linq|-|

> 
>  Dim List As List(Of Object) = New List(Of Object)
>  
>  For Each [Object] In LINQ.GetCollection(Statement)
>     Call SetObject([Object])
>     If True = Test() Then
>         List.Add(SelectConstruct())
>     End If
>  Next
>  Return List.ToArray
>  

#### SetObject
```csharp
Microsoft.VisualBasic.Linq.Script.DynamicsRuntime.SetObject(System.String,System.Collections.IEnumerable)
```


|Parameter Name|Remarks|
|--------------|-------|
|name|-|
|source|-|


#### Source
```csharp
Microsoft.VisualBasic.Linq.Script.DynamicsRuntime.Source(System.String)
```
执行一个LINQ查询脚本文件

|Parameter Name|Remarks|
|--------------|-------|
|path|LINQ脚本文件的文件路径|

> 
>  脚本要求：
>  Imports Namespace
>  var result = <Linq>
>  


