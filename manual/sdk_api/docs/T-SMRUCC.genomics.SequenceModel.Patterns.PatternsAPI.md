---
title: PatternsAPI
---

# PatternsAPI
_namespace: [SMRUCC.genomics.SequenceModel.Patterns](N-SMRUCC.genomics.SequenceModel.Patterns.html)_





### Methods

#### __frequency
```csharp
SMRUCC.genomics.SequenceModel.Patterns.PatternsAPI.__frequency(System.Collections.Generic.IEnumerable{SMRUCC.genomics.SequenceModel.FASTA.FastaToken},System.Int32,System.Char,System.Int32)
```
Statics of the occurence frequency for the specific alphabet at specific 
 column in the fasta source.
 (因为是大小写敏感的，所以参数@"N:SMRUCC.genomics.SequenceModel.FASTA"里面的所有的序列数据都必须是大写的)

|Parameter Name|Remarks|
|--------------|-------|
|Fasta|-|
|p|The column number.|
|C|Alphabet specific for the frequency statics|
|numOfFasta|The total number of the fasta sequence|


#### __variation
```csharp
SMRUCC.genomics.SequenceModel.Patterns.PatternsAPI.__variation(System.Char,System.Int32,System.Double,SMRUCC.genomics.SequenceModel.Patterns.PatternModel)
```
这个是参考的碱基位点

|Parameter Name|Remarks|
|--------------|-------|
|ch|-|
|index|-|
|cutoff|-|
|fr|-|


#### Frequency
```csharp
SMRUCC.genomics.SequenceModel.Patterns.PatternsAPI.Frequency(System.Collections.Generic.IEnumerable{SMRUCC.genomics.SequenceModel.FASTA.FastaToken})
```
Simple function for statics the alphabet frequency in the fasta source. 
 The returns matrix, alphabet key char is Upper case.
 (返回来的数据之中的残基的字符是大写的)

|Parameter Name|Remarks|
|--------------|-------|
|source|
 Fasta sequence source, and all of the fasta sequence 
 in this source must in the same length.
 |


#### NTVariations
```csharp
SMRUCC.genomics.SequenceModel.Patterns.PatternsAPI.NTVariations(SMRUCC.genomics.SequenceModel.FASTA.FastaFile,System.Int32,System.Double)
```
The conservation percentage (%) Is defined as the number of genomes with the same letter on amultiple sequence alignment normalized to range from 0 to 100% for each site along the chromosome of a specific index genome.

|Parameter Name|Remarks|
|--------------|-------|
|index|参考序列在所输入的fasta序列之中的位置，默认使用第一条序列作为参考序列|



