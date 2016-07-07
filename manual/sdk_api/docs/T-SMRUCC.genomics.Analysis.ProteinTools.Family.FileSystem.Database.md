---
title: Database
---

# Database
_namespace: [SMRUCC.genomics.Analysis.ProteinTools.Family.FileSystem](N-SMRUCC.genomics.Analysis.ProteinTools.Family.FileSystem.html)_

数据库是分模块存放的，每一个模块单独存储于一个Xml文件之中，这个对象就是管理这些模块在文件系统之中的位置的对象



### Methods

#### Add
```csharp
SMRUCC.genomics.Analysis.ProteinTools.Family.FileSystem.Database.Add(System.String,SMRUCC.genomics.Analysis.ProteinTools.Family.FileSystem.Family)
```
请注意，这个函数会直接覆盖已经存在的数据库文件而不会给出任何警告的

|Parameter Name|Remarks|
|--------------|-------|
|Name|-|
|Db|-|


#### EntireDb
```csharp
SMRUCC.genomics.Analysis.ProteinTools.Family.FileSystem.Database.EntireDb
```
当比对没有指定数据库的时候默认是比对全部的数据库

#### GetDatabase
```csharp
SMRUCC.genomics.Analysis.ProteinTools.Family.FileSystem.Database.GetDatabase(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Name|数据库的名称或者文件路径|



