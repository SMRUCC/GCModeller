---
title: RESTProvider
---

# RESTProvider
_namespace: [RQL](N-RQL.html)_

在线查询服务提供模块，在这个模块之中只负责进行url参数的解析工作



### Methods

#### #ctor
```csharp
RQL.RESTProvider.#ctor(System.Int32,RQL.Linq.Repository)
```


|Parameter Name|Remarks|
|--------------|-------|
|portal|-|
|repo|需要在这里将url转换为Long以进行protocol的绑定操作|


#### handleGETRequest
```csharp
RQL.RESTProvider.handleGETRequest(SMRUCC.HTTPInternal.Core.HttpProcessor)
```
http://linq.gcmodeller.org/kegg/pathways?where=test_expr(pathway)
 测试条件里面的对象实例的标识符使用资源url里面的最后一个标识符为变量名
 测试条件表达式使用VisualBasic的语法
 测试条件必须以where起头开始

|Parameter Name|Remarks|
|--------------|-------|
|p|-|



