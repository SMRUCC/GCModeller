---
title: API
---

# API
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT](N-SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.html)_

Gene expression chip data.(基因芯片数据)



### Methods

#### __getBirefDicts
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.API.__getBirefDicts(System.Collections.Generic.IEnumerable{SMRUCC.genomics.ComponentModel.PathwayBrief})
```


|Parameter Name|Remarks|
|--------------|-------|
|Briefs|-|

_returns: {GeneId, Pathways}_

#### LoadChipData
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.API.LoadChipData(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|path|CSV document which contains the gene expression value.|


#### MergeDataMatrix
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.API.MergeDataMatrix(System.String,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|DIR|放着多个实验所得到的芯片数据的文件夹|
|RemoveGaps|在芯片数据之中所缺失的部分的数据是否进行移除，否则将会使用数值0来代替|


#### SelectLog2Genes
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.API.SelectLog2Genes(SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.MatrixFrame,System.Collections.Generic.IEnumerable{System.String},System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|dat|-|
|expr|-|
|level|上调或者下调的log2的最低倍数|



