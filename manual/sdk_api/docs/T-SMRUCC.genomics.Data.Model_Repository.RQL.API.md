---
title: API
---

# API
_namespace: [SMRUCC.genomics.Data.Model_Repository.RQL](N-SMRUCC.genomics.Data.Model_Repository.RQL.html)_





### Methods

#### ImportsGBK
```csharp
SMRUCC.genomics.Data.Model_Repository.RQL.API.ImportsGBK(System.String)
```
当**path**参数为一个文件夹的时候，会扫描该文件夹之中的所有文件

|Parameter Name|Remarks|
|--------------|-------|
|path|If the path parameter is specific to a file, then it will imports the target GenBank file, orelse it will try imports the path as a folder in batch mode!|


#### SQLImports
```csharp
SMRUCC.genomics.Data.Model_Repository.RQL.API.SQLImports(System.String,System.Object)
```
更新或者插入新的数据对象**obj**到目标数据表**tableName**之中

|Parameter Name|Remarks|
|--------------|-------|
|tableName|-|
|obj|-|


#### UpdateGenbankTableSchema
```csharp
SMRUCC.genomics.Data.Model_Repository.RQL.API.UpdateGenbankTableSchema
```
这个函数仅仅是在修改了数据结构之后，需要更新表结构的时候才需要使用的


