---
title: TranscriptionFactors
---

# TranscriptionFactors
_namespace: [SMRUCC.genomics.Data.Regprecise](N-SMRUCC.genomics.Data.Regprecise.html)_

[Regprecise database] [Collections of regulogs classified by transcription factors]
 Each transcription factor collection organizes all reconstructed regulogs for a given set of orthologous
 regulators across different taxonomic groups of microorganisms. These collections represent regulons for
 a selected subset of transcription factors. The collections include both widespread transcription factors
 such as NrdR, LexA, and Zur, that are present in more than 25 diverse taxonomic groups of Bacteria, and
 narrowly distributed transcription factors such as Irr and PurR. The TF regulon collections are valuable
 for comparative and evolutionary analysis of TF binding site motifs and regulon content for orthologous
 transcription factors.



### Methods

#### BuildRegulatesHash
```csharp
SMRUCC.genomics.Data.Regprecise.TranscriptionFactors.BuildRegulatesHash
```
生成映射{site, TF()}

#### Get_Regulators
```csharp
SMRUCC.genomics.Data.Regprecise.TranscriptionFactors.Get_Regulators(SMRUCC.genomics.Data.Regprecise.Regulator.Types)
```
选择所有的调控因子请使用@"M:SMRUCC.genomics.Data.Regprecise.TranscriptionFactors.Get_Regulators(SMRUCC.genomics.Data.Regprecise.Regulator.Types)"

|Parameter Name|Remarks|
|--------------|-------|
|Type|-|


#### GetRegulators
```csharp
SMRUCC.genomics.Data.Regprecise.TranscriptionFactors.GetRegulators(System.String)
```
相较于@"M:SMRUCC.genomics.Data.Regprecise.TranscriptionFactors.GetRegulatorId(System.String,System.Int32)"方法，其只是获取得到的第一个调控因子的编号，推荐使用这个方法来获取完整的调控因子的信息

|Parameter Name|Remarks|
|--------------|-------|
|trace|locus_tag:position|



