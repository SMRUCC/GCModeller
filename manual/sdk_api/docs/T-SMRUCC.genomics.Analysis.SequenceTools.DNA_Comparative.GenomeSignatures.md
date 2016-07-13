---
title: GenomeSignatures
---

# GenomeSignatures
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative](N-SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.html)_

在本模块之中，所有的计算过程都是基于@"T:SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid"核酸对象的



### Methods

#### __counts
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.GenomeSignatures.__counts(SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid,SMRUCC.genomics.SequenceModel.NucleotideModels.DNA[])
```
计算某些连续的碱基片段在序列之中的出现频率

|Parameter Name|Remarks|
|--------------|-------|
|Sequence|-|
|ns|-|


#### __counts_p
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.GenomeSignatures.__counts_p(SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid,SMRUCC.genomics.SequenceModel.NucleotideModels.DNA[])
```
计算某些连续的碱基片段在序列之中的出现频率(并行版本)

|Parameter Name|Remarks|
|--------------|-------|
|Sequence|-|
|ns|-|


#### CodonSignature
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.GenomeSignatures.CodonSignature(SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid,SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.Codon)
```
CODON SIGNATURE
 
 For a given collection of genes, let fX(1); fY(2); fZ(3) denote frequencies of the indicated nucleotide at codon sites 1, 2, and 3, respectively, 
 and let f(XYZ) indicate codon frequency. The embedded dinucleotide frequencies are denoted fXY(1, 2); fYZ(2, 3); and fXZ(1, 3). Dinucleotide 
 contrasts are assessed through the odds ratio pXY = f(XY)/f(X)f(Y). 
 In the context of codons, we define
 
 ```
 pXY(1, 2) = fXY(1, 2)/fX(1)fY(2)
 pYZ(2, 3) = fYZ(2, 3)/fY(2)fZ(3)
 pXZ(1, 3) = fXZ(1, 3)/fX(1)fZ(3)
 ```
 
 We refer to the profiles {pXY(1, 2)}; {pXZ(1, 3)}; {pYZ(2, 3)}, and also {pZW(3, 4)}, where 4(=1) is the first position of the next codon, as the 
 codon signature to be distinguished from the global genome signature

|Parameter Name|Remarks|
|--------------|-------|
|Sequence|-|
|Codon|-|


#### DinucleotideBIAS
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.GenomeSignatures.DinucleotideBIAS(SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid,SMRUCC.genomics.SequenceModel.NucleotideModels.DNA,SMRUCC.genomics.SequenceModel.NucleotideModels.DNA)
```
Dinucleotide relative abundance values (dinucleotide bias) are assessed through the odds ratio ``p(XY) = f(XY)/f(X)f(Y)``, 
 where ``fX`` denotes the frequency of the nucleotide ``X`` and ``fXY`` is the frequency of the dinucleotide ``XY`` in the 
 sequence under study.

#### DinucleotideBIAS_p
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.GenomeSignatures.DinucleotideBIAS_p(SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid,SMRUCC.genomics.SequenceModel.NucleotideModels.DNA,SMRUCC.genomics.SequenceModel.NucleotideModels.DNA)
```
Dinucleotide relative abundance values (dinucleotide bias) are assessed through the 
 odds ratio ``p(XY) = f(XY)/f(X)f(Y)``, where fX denotes the frequency of 
 the nucleotide X and fXY is the frequency of the dinucleotide XY in the 
 sequence under study.(并行版本)


