---
title: MotifPM
---

# MotifPM
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel.html)_

Motif序列之中的一个位点



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel.MotifPM.#ctor(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|s|MEME文本文件结果数据之中的概率表之中的一行文本|


#### CreateObject
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel.MotifPM.CreateObject(SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel.MotifPM[][])
```


|Parameter Name|Remarks|
|--------------|-------|
|MAT|长度必须要相等|



### Properties

#### A
@"F:SMRUCC.genomics.SequenceModel.NucleotideModels.DNA.dAMP"碱基在这个位点之上出现的概率
#### C
@"F:SMRUCC.genomics.SequenceModel.NucleotideModels.DNA.dCMP"碱基在这个位点之上出现的概率
#### G
@"F:SMRUCC.genomics.SequenceModel.NucleotideModels.DNA.dGMP"碱基在这个位点之上出现的概率
#### MostProperly
当前的位点之上最有可能出现的碱基及其概率，即可能出现的概率最高的碱基
#### T
@"F:SMRUCC.genomics.SequenceModel.NucleotideModels.DNA.dTMP"碱基在这个位点之上出现的概率
