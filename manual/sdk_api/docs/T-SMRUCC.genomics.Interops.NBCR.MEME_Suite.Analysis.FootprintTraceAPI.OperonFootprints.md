---
title: OperonFootprints
---

# OperonFootprints
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.FootprintTraceAPI](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.FootprintTraceAPI.html)_

操作Operon调控相关的信息



### Methods

#### __copy
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.FootprintTraceAPI.OperonFootprints.__copy(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint,SMRUCC.genomics.Assembly.DOOR.GeneBrief,SMRUCC.genomics.Analysis.RNA_Seq.Correlation2)
```
这里主要是拓展Trace信息

|Parameter Name|Remarks|
|--------------|-------|
|x|-|
|g|-|
|corrs|操纵子的数据可能会有预测错误的，所以在这里任然需要转录组数据进行筛选|


#### __expands
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.FootprintTraceAPI.OperonFootprints.__expands(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint,SMRUCC.genomics.Assembly.DOOR.Operon,SMRUCC.genomics.Analysis.RNA_Seq.Correlation2)
```


|Parameter Name|Remarks|
|--------------|-------|
|x|-|
|operon|-|

_returns: 原来的数据将不会被添加_

#### AssignDOOR
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.FootprintTraceAPI.OperonFootprints.AssignDOOR(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint},SMRUCC.genomics.Assembly.DOOR.DOOR)
```
为调控关系之中的基因联系上操纵子的信息

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|DOOR|-|


#### ExpandDOOR
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.FootprintTraceAPI.OperonFootprints.ExpandDOOR(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint},SMRUCC.genomics.Assembly.DOOR.DOOR,SMRUCC.genomics.Analysis.RNA_Seq.Correlation2,System.Double)
```
假若被调控的基因是操纵子的第一个基因，则后面的基因假设都会被一同调控

|Parameter Name|Remarks|
|--------------|-------|
|footprints|-|
|DOOR|-|

_returns: 新拓展出来的数据以及原来的数据_


