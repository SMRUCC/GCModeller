---
title: HitCollection
---

# HitCollection
_namespace: [SMRUCC.genomics.Interops.NCBI.Extensions.Analysis](N-SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.html)_

A collection of hits for the target query protein.

> 
>  其实这个就是相当于一个KEGG里面的SSDB BBH结果文件
>  


### Methods

#### __orderBySp
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.HitCollection.__orderBySp
```
按照菌株排序

#### Take
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.HitCollection.Take(System.String[])
```
按照物种编号取出数据构建一个新的bbh集合

|Parameter Name|Remarks|
|--------------|-------|
|spTags|-|



### Properties

#### Description
Query protein functional annotation.
#### Hit
Gets hits protein tag inform by hit protein locus_tag
#### Hits
Query hits protein.
#### QueryName
The locus tag of the query protein.(主键蛋白质名称)
