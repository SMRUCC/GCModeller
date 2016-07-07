---
title: SR
---

# SR
_namespace: [SMRUCC.genomics.SequenceModel.Patterns.Clustal](N-SMRUCC.genomics.SequenceModel.Patterns.Clustal.html)_

Semi-Residue(半残基，出现概率不是百分之百的)



### Methods

#### __allocBlock
```csharp
SMRUCC.genomics.SequenceModel.Patterns.Clustal.SR.__allocBlock(SMRUCC.genomics.SequenceModel.Patterns.Clustal.SR[][],System.Double)
```
做了一次矩阵转置

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|cutoff|-|


#### __getSite
```csharp
SMRUCC.genomics.SequenceModel.Patterns.Clustal.SR.__getSite(System.Char[][],System.Int32,System.Int32,System.Int32)
```
返回前10的位点
 位点不足的话会使用最后一个位点进行补充
 
 这里是构建特征序列的核心算法部分

|Parameter Name|Remarks|
|--------------|-------|
|chMAT|频率矩阵|
|index|-|
|width|-|


#### FromAlign
```csharp
SMRUCC.genomics.SequenceModel.Patterns.Clustal.SR.FromAlign(System.Collections.Generic.IEnumerable{SMRUCC.genomics.SequenceModel.FASTA.FastaToken},System.Double,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|aln|-|
|block|生成类似于domain的参数所需求的，假若这个值太低，则计算MPAlignment的时候会得分很低，除非特别应用请不要修改这个数值|
|levels|-|



### Properties

#### Block
区块的序列号
#### Frq
出现的频率
#### Index
在多重比对之中的序列上面的位置
#### Residue
残基符号
