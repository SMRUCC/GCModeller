---
title: SegmentReader
---

# SegmentReader
_namespace: [SMRUCC.genomics.SequenceModel.NucleotideModels](N-SMRUCC.genomics.SequenceModel.NucleotideModels.html)_

The reader of the nucleotide sequence loci segment.(核酸链上面的一个片段区域的读取对象，注意，这个数据结构都是以正义链为标准的)



### Methods

#### #ctor
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader.#ctor(SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|SequenceData|-|
|LinearMolecule|
 Does this DNA sequence is a circular DNA molecule or a linear DNA molecule, default it is a linear DNA molecule.(是否为环状的DNA分子，默认为线形)
 |


#### FrameMoveForward
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader.FrameMoveForward(System.Int32)
```
以当前的位置为参考向前移动一段**internal**距离

|Parameter Name|Remarks|
|--------------|-------|
|internal|-|


#### GetSegmentSequence
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader.GetSegmentSequence(System.Int32,System.Int32)
```
按照给定的位置来获取序列片段，请注意，这个函数仅仅是针对正义链的
 例如：Left=10, Right=50，则函数会取出10-50bp这个区间的长度为41个碱基的序列片段

|Parameter Name|Remarks|
|--------------|-------|
|Left|-|
|Right|-|


#### ReadComplement
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader.ReadComplement(System.Int32,System.Int32,System.Boolean)
```
首先按照正向链的方法取出序列片段，然后在进行互补反向得到反向链的序列片段数据，这个方法是专门用于读取反向序列的

|Parameter Name|Remarks|
|--------------|-------|
|Start|-|
|Length|-|
|directionDownstream|-|


#### ReadJoinLocation
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader.ReadJoinLocation(System.Int32,System.Int32)
```
**St** to End Join 1 to **Sp**

|Parameter Name|Remarks|
|--------------|-------|
|St|-|
|Sp|-|


#### TryParse
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader.TryParse(SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation)
```
Try parsing the DNA sequence using a specific nucleotide loci value.(假若序列是在反向链之上，在反向链之上，则会自动进行互补反向)

|Parameter Name|Remarks|
|--------------|-------|
|Location|位点的位置，由于一般情况之下右边的位置的值是大于左边的位置的值的|



### Properties

#### OriginalSequence
这个属性是原始的完整的序列数据
#### SequenceData
返回当前阅读区域之中的序列数据，请尽量使用本属性读取序列数据，但是请小心的通过本属性写入数据，因为每一次写入数据，都会重置内部的@"F:SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader._innerNTsource"对象的值
