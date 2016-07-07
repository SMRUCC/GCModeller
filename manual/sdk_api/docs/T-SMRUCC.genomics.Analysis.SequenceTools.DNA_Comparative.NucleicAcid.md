---
title: NucleicAcid
---

# NucleicAcid
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative](N-SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.html)_

为了加快计算速度而生成的窗口计算缓存，请注意，在生成缓存的时候已经进行了并行化，所以在内部生成缓存的时候，不需要再进行并行化了



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.NucleicAcid.#ctor(SMRUCC.genomics.SequenceModel.FASTA.FastaToken)
```
Fasta序列会自动使用@"P:SMRUCC.genomics.SequenceModel.FASTA.FastaToken.Title"来作为序列的@"P:SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid.UserTag"

|Parameter Name|Remarks|
|--------------|-------|
|SequenceData|-|


#### GetValue
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.NucleicAcid.GetValue(SMRUCC.genomics.SequenceModel.NucleotideModels.DNA,SMRUCC.genomics.SequenceModel.NucleotideModels.DNA)
```
Get value by using a paired of base.

|Parameter Name|Remarks|
|--------------|-------|
|X|-|
|Y|-|



