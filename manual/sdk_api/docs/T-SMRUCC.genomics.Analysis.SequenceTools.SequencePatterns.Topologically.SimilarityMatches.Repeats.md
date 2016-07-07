---
title: Repeats
---

# Repeats
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.SimilarityMatches](N-SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.SimilarityMatches.html)_

模糊匹配重复的序列片段



### Methods

#### __generateSeeds
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.SimilarityMatches.Repeats.__generateSeeds(System.Char[],System.String,System.Double)
```
生成和**loci**满足相似度匹配关系的序列的集合

|Parameter Name|Remarks|
|--------------|-------|
|Chars|-|
|loci|-|
|Cutoff|-|


#### __matchLociLocation
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.SimilarityMatches.Repeats.__matchLociLocation(System.String,System.String[])
```


|Parameter Name|Remarks|
|--------------|-------|
|Sequence|-|
|seeds|为了加快计算效率，事先所生成的种子缓存|


#### InvokeSearch
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.SimilarityMatches.Repeats.InvokeSearch(System.String,System.Int32,System.Int32,System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|SequenceData|-|
|Min|-|
|Max|-|
|cutoff|-|

> 为了加快计算，首先生成种子，然后再对种子进行模糊匹配

#### MatchLociLocations
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.SimilarityMatches.Repeats.MatchLociLocations(System.String,System.String,System.Int32,System.Int32,System.Double)
```
模糊匹配相似的位点在目标序列之上的位置

|Parameter Name|Remarks|
|--------------|-------|
|Sequence|-|
|loci|-|
|Min|-|
|Max|-|
|cutoff|-|



