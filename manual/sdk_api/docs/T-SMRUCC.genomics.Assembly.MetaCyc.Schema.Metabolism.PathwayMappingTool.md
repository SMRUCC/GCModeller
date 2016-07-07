---
title: PathwayMappingTool
---

# PathwayMappingTool
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.Schema.Metabolism](N-SMRUCC.genomics.Assembly.MetaCyc.Schema.Metabolism.html)_

使用一个汇总的MetaCyc数据库，根据目标物种的基因组以及蛋白质信息进行MetaCyc数据库的重建工作



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Assembly.MetaCyc.Schema.Metabolism.PathwayMappingTool.#ctor(SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder)
```
用于进行参考的MetaCyc数据库

|Parameter Name|Remarks|
|--------------|-------|
|MetaCyc|-|


#### DownloadFromUniprot
```csharp
SMRUCC.genomics.Assembly.MetaCyc.Schema.Metabolism.PathwayMappingTool.DownloadFromUniprot(SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Proteins,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Proteins|-|
|SavedFile|-|

_returns: 返回序列下载结果，当所有的序列结果都下载完成的时候，返回0，当出现没有被下载的序列的情况时，返回未被下载的序列数_


