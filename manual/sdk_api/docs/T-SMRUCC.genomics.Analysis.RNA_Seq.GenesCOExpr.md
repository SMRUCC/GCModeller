---
title: GenesCOExpr
---

# GenesCOExpr
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq](N-SMRUCC.genomics.Analysis.RNA_Seq.html)_

Gene co-expression analysis.(基因共表达分析)



### Methods

#### Calculate
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.GenesCOExpr.Calculate(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
```
Calculate the coefficient matrix of the gene expression profile data for co-expression analysis.(计算基因表达的相关性矩阵，用于基因共转录分析)

|Parameter Name|Remarks|
|--------------|-------|
|ChipData|基因芯片数据|


#### CalculateRegulations
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.GenesCOExpr.CalculateRegulations(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
```
首先进行关联分析，然后根据转录组数据筛选出调控事件
_returns: 有或者没有，高或者低_

#### MergeChipData
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.GenesCOExpr.MergeChipData(System.String[],System.Int32,System.Int32[])
```
Merge a group set of gene chip data.(合并多个基因芯片数据)

|Parameter Name|Remarks|
|--------------|-------|
|FileList|-|
|IdCol|-|
|ExprDataCols|-|


#### MergeChipData2
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.GenesCOExpr.MergeChipData2(System.String[],System.Int32,System.Int32[])
```
WGCNA包对芯片数据的格式要求，芯片数据矩阵的行表示为样本，列表示基因

|Parameter Name|Remarks|
|--------------|-------|
|FileList|-|
|IdCol|-|
|ExprDataCols|-|


#### Reconstruction
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.GenesCOExpr.Reconstruction(SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.ExprSamples[],System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|Data|-|
|CutOffValue|范围在0-1之间的最小阈值，建议取值0.6|


#### SortMaxliklyhood
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.GenesCOExpr.SortMaxliklyhood(SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.ExprSamples[],System.String,System.Double,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|pccData|-|
|Id|-|
|cutOff|Pcc value cut off, all of the item have the value large than cutoff value will be return.|



