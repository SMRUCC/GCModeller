---
title: Context
---

# Context
_namespace: [SMRUCC.genomics.ContextModel](N-SMRUCC.genomics.ContextModel.html)_

Context model of a specific genomics feature site.



### Methods

#### __relUnstrand
```csharp
SMRUCC.genomics.ContextModel.Context.__relUnstrand(SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation)
```
Get @"T:SMRUCC.genomics.ComponentModel.Loci.SegmentRelationships" ignored of nucleotide @"T:SMRUCC.genomics.ComponentModel.Loci.Strands".

|Parameter Name|Remarks|
|--------------|-------|
|loci|-|


#### GetRelation
```csharp
SMRUCC.genomics.ContextModel.Context.GetRelation(SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation,System.Boolean)
```
Get relationship between target @"T:SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation" with current feature site.

|Parameter Name|Remarks|
|--------------|-------|
|loci|-|
|stranded|Get @"T:SMRUCC.genomics.ComponentModel.Loci.SegmentRelationships" ignored of nucleotide @"T:SMRUCC.genomics.ComponentModel.Loci.Strands"?|


#### ToString
```csharp
SMRUCC.genomics.ContextModel.Context.ToString
```
Get tags data


### Properties

#### Downstream
@"F:SMRUCC.genomics.ContextModel.Context.Feature" its downstream region with a specific length
#### Feature
Current feature site
#### Tag
The user custom tag data for this feature site.
#### Upstream
@"F:SMRUCC.genomics.ContextModel.Context.Feature" its upstream region with a specific length
