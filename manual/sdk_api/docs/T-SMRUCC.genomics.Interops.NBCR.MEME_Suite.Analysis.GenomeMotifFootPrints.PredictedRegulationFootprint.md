---
title: PredictedRegulationFootprint
---

# PredictedRegulationFootprint
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.html)_

Regulation information from the MEME software and regprecise database.(使用MEME软件和regprecise数据库所生成的预测的调控信息)



### Methods

#### __createRegulationObject
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint.__createRegulationObject(SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.HTML.MEMEOutput,SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTTDbLoader,System.Boolean,System.Int32)
```
当具有重叠数据的时候，会返回多个对象拷贝

|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|GenomeSequence|-|
|Ptt|-|
|IgnoreDirection|-|
|ATGDistance|-|


#### Clone
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint.Clone
```
复制出一个新的对象，这个的速度要比@"M:SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint.Copy"函数要高，但是在调整数据结构的时候可能会出现BUG

#### Copy
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint.Copy
```
建议使用这个方法来获取得到副本

#### OperonRegulationCopies
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint.OperonRegulationCopies
```
将操作子的调控转换为一对一的调控描述


