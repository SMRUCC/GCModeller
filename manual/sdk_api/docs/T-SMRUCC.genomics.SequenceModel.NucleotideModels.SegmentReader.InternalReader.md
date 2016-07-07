---
title: InternalReader
---

# InternalReader
_namespace: [SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader](N-SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader.html)_





### Methods

#### __readSpanningCircularDNA
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader.InternalReader.__readSpanningCircularDNA(System.Int32,System.Int32)
```
当读取器读取到DNA片段的末尾的时候，可能会长度不够读取目标长度了，假若序列对象为环状分子，则可以使用这个方法来读取连续的序列数据

#### GetSegmentSequenceValue
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader.InternalReader.GetSegmentSequenceValue(System.Int64,System.Int64,System.Boolean,System.Boolean)
```
A method for getting a DNA segment on this DNA sequence.(获取本DNA序列之上的一个序列片段的方法)

|Parameter Name|Remarks|
|--------------|-------|
|Start|The segment start point.(开始的位点)|
|Length|The length of this segment.(片段的长度)|
|DirectionDownstream|可选参数，向Start位点的上游取片段还是向Start位点的下游取片段，默认取下游片段|


#### ReadDownStream
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader.InternalReader.ReadDownStream(System.Int64,System.Int64,System.Boolean)
```
在取序列的时候请注意位置：对于Mid函数与数组操作而言，Mid函数的开始下标是从1开始的，所以对于第一个碱基，其在字符数组之中为第一个元素，下表为零，但是在字符串之中，下标为1

|Parameter Name|Remarks|
|--------------|-------|
|Start|-|
|Length|-|



