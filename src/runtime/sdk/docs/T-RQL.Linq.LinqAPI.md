---
title: LinqAPI
---

# LinqAPI
_namespace: [RQL.Linq](N-RQL.Linq.html)_

对外部提供Linq查询服务的WebApp



### Methods

#### Free
```csharp
RQL.Linq.LinqAPI.Free(System.Collections.Generic.Dictionary{System.String,System.String})
```
释放掉一个Linq查询的资源

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### MoveNext
```csharp
RQL.Linq.LinqAPI.MoveNext(System.Collections.Generic.Dictionary{System.String,System.String})
```


|Parameter Name|Remarks|
|--------------|-------|
|args|uid,n|


#### OpenQuery
```csharp
RQL.Linq.LinqAPI.OpenQuery(System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|url|数据源的引用位置|
|args|查询参数|



### Properties

#### __uidMaps
{hashCode.tolower, linq_uid}
#### uid
Linq数据源的MD5哈希值
