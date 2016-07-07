---
title: TomTOm
---

# TomTOm
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery.html)_

various motif column comparison functions and score combination methods



### Methods

#### Compare
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery.TomTOm.Compare(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.AnnotationModel,SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.AnnotationModel,SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery.TomTOm.ColumnCompare,SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery.Parameters)
```


|Parameter Name|Remarks|
|--------------|-------|
|query|-|
|LDM|GCModeller的数据库里面的Motif模型|
|method|-|


#### CompareBest
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery.TomTOm.CompareBest(System.String,System.String,System.Double,System.Double,System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|query|query的MEME.txt|


#### ED
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery.TomTOm.ED(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.ResidueSite,SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.ResidueSite)
```


|Parameter Name|Remarks|
|--------------|-------|
|X|-|
|Y|-|


#### PCC
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery.TomTOm.PCC(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.ResidueSite,SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.ResidueSite)
```
PCC, Pearson correlation coefficient;

|Parameter Name|Remarks|
|--------------|-------|
|X|-|
|Y|-|


#### ToChar
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery.TomTOm.ToChar(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.ResidueSite)
```
大写字母表示概率很高的，小写字母表示概率比较低的，N表示任意碱基

|Parameter Name|Remarks|
|--------------|-------|
|x|-|



