---
title: AlignmentColumn
---

# AlignmentColumn
_namespace: [SMRUCC.genomics.Analysis.ProteinTools.Interactions.SequenceAssembler](N-SMRUCC.genomics.Analysis.ProteinTools.Interactions.SequenceAssembler.html)_





### Methods

#### Compute
```csharp
SMRUCC.genomics.Analysis.ProteinTools.Interactions.SequenceAssembler.AlignmentColumn.Compute(System.String[])
```


|Parameter Name|Remarks|
|--------------|-------|
|SequenceCollection|所有的序列数据都必须是等长的|


#### GetFrequency
```csharp
SMRUCC.genomics.Analysis.ProteinTools.Interactions.SequenceAssembler.AlignmentColumn.GetFrequency(System.Char)
```
获取特定残基在本列中的出现概率

|Parameter Name|Remarks|
|--------------|-------|
|Residue|目标残基对象的字母代号|


#### GetResidueCollection
```csharp
SMRUCC.genomics.Analysis.ProteinTools.Interactions.SequenceAssembler.AlignmentColumn.GetResidueCollection
```
获取所有的残基的集合


### Properties

#### Alphabets
这个表记录了每一个残基对象在该列种的出现频数
#### CharArray
原始的序列数据，每一个元素代表一行中的某一个位置的残基元素
#### Handle
即本列比对列在整个表中的列标号，在初始化开始必须要赋值
#### ProteinAlphabetDictionary
氨基酸残基的代码名称和其在统计列种的位置，-符号表示比对结果中的空缺
#### SequenceCount
序列数目，即统计的样本数
