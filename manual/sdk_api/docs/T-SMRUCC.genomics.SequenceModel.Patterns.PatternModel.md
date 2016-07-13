---
title: PatternModel
---

# PatternModel
_namespace: [SMRUCC.genomics.SequenceModel.Patterns](N-SMRUCC.genomics.SequenceModel.Patterns.html)_

一个经过多序列比对对齐操作的序列集合之中所得到的残基的出现频率模型，可以用这个模型来计算突变率以及SNP位点



### Methods

#### GetVariation
```csharp
SMRUCC.genomics.SequenceModel.Patterns.PatternModel.GetVariation(SMRUCC.genomics.SequenceModel.FASTA.FastaToken,System.Double)
```
通过比较参考序列得到每一个残基上面的突变率

|Parameter Name|Remarks|
|--------------|-------|
|ref|序列的长度必须要和@"P:SMRUCC.genomics.SequenceModel.Patterns.PatternModel.Residues"的长度相等|
|cutoff|-|



