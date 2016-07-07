---
title: IGenomicsContextProvider`1
---

# IGenomicsContextProvider`1
_namespace: [SMRUCC.genomics.ContextModel](N-SMRUCC.genomics.ContextModel.html)_





### Methods

#### GetRelatedGenes
```csharp
SMRUCC.genomics.ContextModel.IGenomicsContextProvider`1.GetRelatedGenes(SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation,System.Boolean,System.Int32)
```
获取某一个位点在位置上有相关联系的基因

|Parameter Name|Remarks|
|--------------|-------|
|loci|-|
|unstrand|-|
|ATGDist|-|


#### GetStrandFeatures
```csharp
SMRUCC.genomics.ContextModel.IGenomicsContextProvider`1.GetStrandFeatures(SMRUCC.genomics.ComponentModel.Loci.Strands)
```
Gets all of the feature sites on the specific @"T:SMRUCC.genomics.ComponentModel.Loci.Strands" nucleotide sequence

|Parameter Name|Remarks|
|--------------|-------|
|strand|-|



### Properties

#### AllFeatures
Listing all of the features loci sites on the genome.
