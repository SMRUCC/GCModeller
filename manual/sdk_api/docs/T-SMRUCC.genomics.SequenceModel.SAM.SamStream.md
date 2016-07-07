---
title: SamStream
---

# SamStream
_namespace: [SMRUCC.genomics.SequenceModel.SAM](N-SMRUCC.genomics.SequenceModel.SAM.html)_





### Methods

#### #ctor
```csharp
SMRUCC.genomics.SequenceModel.SAM.SamStream.#ctor(System.String,System.Text.Encoding)
```


|Parameter Name|Remarks|
|--------------|-------|
|handle|The file path of the *.sam file.|


#### __parsingSAMReads
```csharp
SMRUCC.genomics.SequenceModel.SAM.SamStream.__parsingSAMReads(System.String[])
```
由于这个函数是在后台背景线程之中被调用的，所以这里不再使用并行化了，以提高计算效率

|Parameter Name|Remarks|
|--------------|-------|
|source|-|


#### ReadBlock
```csharp
SMRUCC.genomics.SequenceModel.SAM.SamStream.ReadBlock(System.Int32)
```
调用的时候请不要使用并行化拓展

|Parameter Name|Remarks|
|--------------|-------|
|chunkSize|-|



### Properties

#### Head
If present, the header must be prior to the alignments. Header lines start With `@', while alignment lines do not.
 (文件的可选头部区域必须要在比对数据区域的前面并且每一行以@符号开始)
