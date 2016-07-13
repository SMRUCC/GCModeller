---
title: BeliefNetwork
---

# BeliefNetwork
_namespace: [SMRUCC.genomics.Analysis.ProteinTools.Interactions](N-SMRUCC.genomics.Analysis.ProteinTools.Interactions.html)_





### Methods

#### Convert
```csharp
SMRUCC.genomics.Analysis.ProteinTools.Interactions.BeliefNetwork.Convert(SMRUCC.genomics.Analysis.ProteinTools.Interactions.SequenceAssembler.AlignmentColumn[],SMRUCC.genomics.Analysis.ProteinTools.Interactions.SequenceAssembler.AlignmentColumn)
```


|Parameter Name|Remarks|
|--------------|-------|
|SubjectColumns|Entity.Properties|
|TargetColumn|Entity.Class|


#### GenerateBlankVector
```csharp
SMRUCC.genomics.Analysis.ProteinTools.Interactions.BeliefNetwork.GenerateBlankVector(System.Int32)
```
使用-1来填充空白的序列区块

|Parameter Name|Remarks|
|--------------|-------|
|Width|-|


#### GenerateNetwork
```csharp
SMRUCC.genomics.Analysis.ProteinTools.Interactions.BeliefNetwork.GenerateNetwork(SMRUCC.genomics.Analysis.ProteinTools.Interactions.SequenceAssembler.AlignmentColumn[])
```


|Parameter Name|Remarks|
|--------------|-------|
|Data|-|

_returns: 假设其网络结构为线性的_

#### GetBelief
```csharp
SMRUCC.genomics.Analysis.ProteinTools.Interactions.BeliefNetwork.GetBelief(System.String[],System.String[])
```


|Parameter Name|Remarks|
|--------------|-------|
|Proteins|蛋白质序列的排列顺序必须与计算网络时候所使用的比对序列的位置相一致，对于空白的部分请使用一个空字符串|
|ProteinsConditions|蛋白质序列的排列顺序必须与计算网络时候所使用的比对序列的位置相一致，对于空白的部分请使用一个空字符串|

> 假设没有缺失的序列部分，并且在这里将空缺转换为-1


