---
title: DbFile
---

# DbFile
_namespace: [SMRUCC.genomics.Assembly.NCBI.CDD](N-SMRUCC.genomics.Assembly.NCBI.CDD.html)_

CDD database builder.(CDD数据库构建工具)

> 
>  ftp://ftp.ncbi.nlm.nih.gov/pub/mmdb/cdd/cdd.tar.gz
>  


### Methods

#### ContainsId
```csharp
SMRUCC.genomics.Assembly.NCBI.CDD.DbFile.ContainsId(System.String)
```
非并行版本的@"P:SMRUCC.genomics.Assembly.NCBI.CDD.SmpFile.Identifier"[AccessionId], @"P:SMRUCC.genomics.Assembly.NCBI.CDD.SmpFile.Id"[TagId], @"P:SMRUCC.genomics.Assembly.NCBI.CDD.SmpFile.CommonName"[CommonName]

#### ContainsId_p
```csharp
SMRUCC.genomics.Assembly.NCBI.CDD.DbFile.ContainsId_p(System.String)
```
并行版本的

|Parameter Name|Remarks|
|--------------|-------|
|Id|-|


#### PreLoad
```csharp
SMRUCC.genomics.Assembly.NCBI.CDD.DbFile.PreLoad(System.String)
```
在编译整个CDD数据库之前进行预加载

|Parameter Name|Remarks|
|--------------|-------|
|DIR|-|


#### Takes
```csharp
SMRUCC.genomics.Assembly.NCBI.CDD.DbFile.Takes(System.Collections.Generic.IEnumerable{System.String})
```
根据唯一标识符的集合来获取数据库记录数据

|Parameter Name|Remarks|
|--------------|-------|
|lstAccId|-|



### Properties

#### Smp
不存在则返回空值
