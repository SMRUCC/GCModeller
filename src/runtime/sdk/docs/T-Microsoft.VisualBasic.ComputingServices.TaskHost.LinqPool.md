---
title: LinqPool
---

# LinqPool
_namespace: [Microsoft.VisualBasic.ComputingServices.TaskHost](N-Microsoft.VisualBasic.ComputingServices.TaskHost.html)_





### Methods

#### Free
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.LinqPool.Free(System.String)
```
uid参数是Linq Portal的Tostring函数的结果

|Parameter Name|Remarks|
|--------------|-------|
|uid|-|


#### OpenQuery
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.LinqPool.OpenQuery(System.Collections.IEnumerable,System.Type)
```


|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|type|
 这里应该是集合的类型，函数会自动从这个类型信息之中得到元素的类型；
 假若函数获取得到集合之中的元素的类型失败的话，则会直接使用所传入的类型参数作为集合之中的元素类型
 |



### Properties

#### __linq
linq池
