---
title: Repository`1
---

# Repository`1
_namespace: [RQL.API](N-RQL.API.html)_

RQL Client API.

> 
>  数据对象在申明创建之后并没有立即执行，而是在调用迭代器之后才会执行查询
>  


### Methods

#### #ctor
```csharp
RQL.API.Repository`1.#ctor(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|source|
 URL, examples as:  http://Linq.gcmodeller.org/kegg/ssdb
 
 Where, the remote server http://Linq.gcmodeller.org implements the RQL services.
 And the repository resource name on the server is "/kegg/ssdb"
 |


#### Where
```csharp
RQL.API.Repository`1.Where(System.String)
```
在这里会打开一个新的查询

|Parameter Name|Remarks|
|--------------|-------|
|assertions|断言|



