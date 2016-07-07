---
title: BioAssemblyExtensions
---

# BioAssemblyExtensions
_namespace: [SMRUCC.genomics](N-SMRUCC.genomics.html)_





### Methods

#### DirectCast``1
```csharp
SMRUCC.genomics.BioAssemblyExtensions.DirectCast``1(System.Collections.Generic.IEnumerable{``0})
```
Generate @"T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile" from a specific fasta source collection.

|Parameter Name|Remarks|
|--------------|-------|
|data|Target fasta source collection which its elements base type is @"T:SMRUCC.genomics.SequenceModel.FASTA.FastaToken"|


#### GetBriefCode
```csharp
SMRUCC.genomics.BioAssemblyExtensions.GetBriefCode(SMRUCC.genomics.ComponentModel.Loci.Strands)
```
Convert the nucleotide sequence strand direction enumeration as character brief code. [@"T:SMRUCC.genomics.ComponentModel.Loci.Strands" => +, -, ?]

|Parameter Name|Remarks|
|--------------|-------|
|strand|-|


#### GetBriefStrandCode
```csharp
SMRUCC.genomics.BioAssemblyExtensions.GetBriefStrandCode(System.String)
```
Convert the nucleotide seuqnece strand description word as character brief code.
 (获取核酸链链方向的描述简要代码)

|Parameter Name|Remarks|
|--------------|-------|
|strand|-|


#### GetCOGCategory
```csharp
SMRUCC.genomics.BioAssemblyExtensions.GetCOGCategory(System.String)
```
将COG字符串进行修剪，返回的是大写的COG符号
 COG4771P -> P; 
 P -> P; 
 <SPACE> -> -; 
 - -> -;

|Parameter Name|Remarks|
|--------------|-------|
|str|-|


#### GetStrands
```csharp
SMRUCC.genomics.BioAssemblyExtensions.GetStrands(System.Char)
```
Convert the string value type nucleotide strand information description data into a strand enumerate data.

|Parameter Name|Remarks|
|--------------|-------|
|c|从文本文件之中所读取出来关于链方向的字符串描述数据|


#### Group``1
```csharp
SMRUCC.genomics.BioAssemblyExtensions.Group``1(System.Collections.Generic.IEnumerable{``0},System.Int32)
```
对位点进行分组操作

|Parameter Name|Remarks|
|--------------|-------|
|contigs|-|
|offsets|-|


#### IsPure
```csharp
SMRUCC.genomics.BioAssemblyExtensions.IsPure(System.Char)
```
Is this nt base is pure

|Parameter Name|Remarks|
|--------------|-------|
|base|-|


#### IsReversed
```csharp
SMRUCC.genomics.BioAssemblyExtensions.IsReversed(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel)
```
Is this ORF is in the reversed strand direction?(判断一段ORF核酸序列是否为反向的)

|Parameter Name|Remarks|
|--------------|-------|
|nt|
 This function parameter is only allowed nucleotide sequence.
 (请注意，这个只允许核酸序列)
 |


#### IsUnknown
```csharp
SMRUCC.genomics.BioAssemblyExtensions.IsUnknown(System.Char)
```
Current nt base is a unknown base?

|Parameter Name|Remarks|
|--------------|-------|
|base|-|



