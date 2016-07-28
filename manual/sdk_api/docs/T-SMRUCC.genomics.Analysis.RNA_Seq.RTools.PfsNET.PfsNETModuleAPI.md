---
title: PfsNETModuleAPI
---

# PfsNETModuleAPI
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET](N-SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.html)_

文件1,2之中所放着的基因是自身所感兴趣的基因突变体的转录组表达值，文件三则是文件1,2之中的两两组合的基因的关系



### Methods

#### CreateList
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.PfsNETModuleAPI.CreateList(System.Collections.Generic.IEnumerable{SMRUCC.genomics.ComponentModel.PathwayBrief},System.Collections.Generic.IEnumerable{System.String})
```
生成与特定表型相关的代谢途径中的所有的基因列表，生成文件1,2所需要的

#### CreateMatrix
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.PfsNETModuleAPI.CreateMatrix(SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.MatrixFrame,System.Collections.Generic.IEnumerable{System.String},System.Collections.Generic.IEnumerable{System.String},System.String,System.Boolean)
```
创建文件1和文件2

|Parameter Name|Remarks|
|--------------|-------|
|chipdata|-|
|Experiments|-|
|lstLocus|感兴趣的表型相关的基因列表，本列表需要专门的一个函数来生成|
|saveTxt|-|


#### Evaluate
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.PfsNETModuleAPI.Evaluate(System.String,System.String,System.String,System.String,System.String,System.String,System.String)
```
使用PfsNET计算突变体表型变化数据

|Parameter Name|Remarks|
|--------------|-------|
|file1|-|
|file2|-|
|file3|-|
|b|-|
|t1|-|
|t2|-|
|n|-|


#### KEGGPathwaysPhenotypeAnalysis
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.PfsNETModuleAPI.KEGGPathwaysPhenotypeAnalysis(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.TabularArchives.SubNetTable},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.KEGG.Archives.Csv.Pathway})
```
Associate the kegg pathways pfsnet calculation result with the kegg pathways phenotypes to see which route that the mutated gene can affected the specific phenotype.

|Parameter Name|Remarks|
|--------------|-------|
|pfsnet|-|
|KEGGPathways|-|


#### LoadResult
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.PfsNETModuleAPI.LoadResult(System.String)
```
从pfsNET所产生的R输出的文本文件之中解析出结果，以用于XML数据或者Csv数据的保存和下一步分析

|Parameter Name|Remarks|
|--------------|-------|
|testfile|-|


#### ParseCsv
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.PfsNETModuleAPI.ParseCsv(System.String,System.Collections.Generic.IEnumerable{SMRUCC.genomics.ComponentModel.PathwayBrief})
```
将原始的计算数据导出为Csv文件，与@"M:SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.PfsNETModuleAPI.ParseCsv(System.String,System.Collections.Generic.IEnumerable{SMRUCC.genomics.ComponentModel.PathwayBrief})"所不同的是，所导出的对象在本函数之中为原始计算数据的log文件，而在另外一个重载函数之中则为解析好的xml文件

|Parameter Name|Remarks|
|--------------|-------|
|imports|-|
|PathwayBriefs|-|


#### PathwayGeneRelationship
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.PfsNETModuleAPI.PathwayGeneRelationship(System.Collections.Generic.IEnumerable{SMRUCC.genomics.ComponentModel.PathwayBrief},System.Collections.Generic.IEnumerable{System.String},System.String)
```
生成文件3
_returns: 返回所保存的文件的路径_

#### SavePfsNET
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.PfsNETModuleAPI.SavePfsNET(SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.PFSNetResultOut,System.String)
```
生成用于启动pfsnet批量分析所使用的批处理脚本

|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|saveCsv|-|



### Properties

#### PFSNet_EvaluateHandle
默认是使用R脚本的计算版本
