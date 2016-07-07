---
title: DifferenceMeasurement
---

# DifferenceMeasurement
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative](N-SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.html)_

MEASURES OF DIFFERENCES WITHIN AND BETWEEN GENOMES.(比较两条核酸序列之间的差异性)



### Methods

#### __bias
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DifferenceMeasurement.__bias(SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.NucleicAcid,SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.NucleicAcid,SMRUCC.genomics.SequenceModel.NucleotideModels.DNA,SMRUCC.genomics.SequenceModel.NucleotideModels.DNA)
```
使用计算缓存进行计算

|Parameter Name|Remarks|
|--------------|-------|
|f|计算缓存|
|g|-|
|X|-|
|Y|-|


#### Sigma
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DifferenceMeasurement.Sigma(SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.NucleicAcid,SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid)
```


|Parameter Name|Remarks|
|--------------|-------|
|f|计算缓存：窗口片段数据或者整条DNA链|
|g|当需要比对序列上面的某一个片段与整条序列之间的差异性的时候，这个参数可以为该需要进行计算比对的序列片段对象|


#### SimilarDescription
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DifferenceMeasurement.SimilarDescription(System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|sigma|value from the calculation of function @"M:SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DifferenceMeasurement.Sigma(SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.NucleicAcid,SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.NucleicAcid)"|



