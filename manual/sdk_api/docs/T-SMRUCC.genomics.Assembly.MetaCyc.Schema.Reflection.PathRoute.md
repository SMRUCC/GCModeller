---
title: PathRoute
---

# PathRoute
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection](N-SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection.html)_

寻找MetaCyc数据库之中的任意两个对象之间的连接关系



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection.PathRoute.#ctor(System.String)
```
MetaCyc数据库的数据文件夹

|Parameter Name|Remarks|
|--------------|-------|
|MetaCycDir|-|


#### GetPath
```csharp
SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection.PathRoute.GetPath(System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|objA|对象A的UniqueId属性|
|objB|对象B的UniqueId属性|

> 
>  程序会首先尝试查找A-->B的最短路线，假若没有查找到，则会尝试查找B-->A的最短路线
>  


