---
title: CorrsDbAPI
---

# CorrsDbAPI
_namespace: [SMRUCC.genomics.Analysis.Annotations](N-SMRUCC.genomics.Analysis.Annotations.html)_





### Methods

#### __commits
```csharp
SMRUCC.genomics.Analysis.Annotations.CorrsDbAPI.__commits(Oracle.LinuxCompatibility.MySQL.MySQL,SMRUCC.genomics.Analysis.RNA_Seq.WGCNA.WGCNAWeight,SMRUCC.genomics.Analysis.RNA_Seq.PccMatrix,SMRUCC.genomics.Analysis.RNA_Seq.PccMatrix)
```
先排序在进行遍历，不需要再进行随机查找了，太耗费时间了

|Parameter Name|Remarks|
|--------------|-------|
|MySQL|-|
|WGCNA_MAT|-|
|PccMatrix|-|
|sPccMAT|-|


#### WriteDatabase
```csharp
SMRUCC.genomics.Analysis.Annotations.CorrsDbAPI.WriteDatabase(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,System.String,Oracle.LinuxCompatibility.MySQL.ConnectionUri)
```


|Parameter Name|Remarks|
|--------------|-------|
|raw|第一列是基因的编号列表，其他的列都是基因的表达数据|
|uri|-|
|WGCNA|WGCNA脚本所计算出来的Cytoscape的边文件|



