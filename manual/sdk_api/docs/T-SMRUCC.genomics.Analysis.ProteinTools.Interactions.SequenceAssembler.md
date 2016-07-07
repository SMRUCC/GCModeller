---
title: SequenceAssembler
---

# SequenceAssembler
_namespace: [SMRUCC.genomics.Analysis.ProteinTools.Interactions](N-SMRUCC.genomics.Analysis.ProteinTools.Interactions.html)_

在计算贝叶斯网络所需要的矩阵之前所进行的比对序列的拼接并进行向量化的程序模块



### Methods

#### Assemble
```csharp
SMRUCC.genomics.Analysis.ProteinTools.Interactions.SequenceAssembler.Assemble(System.String[][])
```
执行序列拼接

|Parameter Name|Remarks|
|--------------|-------|
|alignSeq|要求比对上的序列的集合中的序列的数目必须要一致|


#### FileIO
```csharp
SMRUCC.genomics.Analysis.ProteinTools.Interactions.SequenceAssembler.FileIO(System.String[])
```


|Parameter Name|Remarks|
|--------------|-------|
|FileList|FASTA文件列表|


#### initialize
```csharp
SMRUCC.genomics.Analysis.ProteinTools.Interactions.SequenceAssembler.initialize(System.String[])
```
每一个AlignmentColumn对象可以看作为贝叶斯网络中的一个节点

|Parameter Name|Remarks|
|--------------|-------|
|alignFiles|FASTA格式的蛋白质比对序列数据|



