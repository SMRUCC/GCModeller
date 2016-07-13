---
title: Module
---

# Module
_namespace: [SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry](N-SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.html)_

KEGG里面的模块的入口点的定义数据



### Methods

#### DownloadModules
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Module.DownloadModules(System.String,System.String,System.String)
```
会按照分类来组织文件夹结构

|Parameter Name|Remarks|
|--------------|-------|
|SpeciesCode|-|
|Export|-|

_returns: 返回成功下载的代谢途径的数目_

#### GetDictionary
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Module.GetDictionary
```
从内部资源之中加载数据然后生成字典返回

#### LoadFromResource
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Module.LoadFromResource
```
从资源文件之中加载模块的入口点的定义数据

#### TrimPath
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Module.TrimPath(System.String)
```
防止文件夹的名称过长而出错

|Parameter Name|Remarks|
|--------------|-------|
|pathToken|-|



### Properties

#### Category
B
#### Class
A
#### Entry
KO
#### SubCategory
C
