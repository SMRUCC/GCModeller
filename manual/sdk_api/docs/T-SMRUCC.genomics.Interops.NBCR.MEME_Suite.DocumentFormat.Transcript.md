---
title: Transcript
---

# Transcript
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.html)_

转录本对象，包含有基本的基因结构：ATG-TGA，TSSs，TTS以及链的方向，表达量的高低



### Methods

#### Copy``1
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.Transcript.Copy``1(SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief)
```
单个的ORF

#### CreateObject``1
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.Transcript.CreateObject``1(SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation)
```
要确保左小右大

|Parameter Name|Remarks|
|--------------|-------|
|contig|-|



### Properties

#### Left
@"T:SMRUCC.genomics.SequenceModel.NucleotideModels.Contig".Left (The transcription start coordinate.)
#### Position
位点和基因对象之间的位置关系的简要描述
#### Raw
Htseq-Count raw/GeneLength
#### Right
@"T:SMRUCC.genomics.SequenceModel.NucleotideModels.Contig".Right (The transcription stop coordinate.)
#### Strand
@"T:SMRUCC.genomics.SequenceModel.NucleotideModels.Contig".Strands
#### TSSsShared
5'UTR左端的共享的reads计数
