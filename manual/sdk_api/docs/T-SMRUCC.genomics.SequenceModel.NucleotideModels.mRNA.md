---
title: mRNA
---

# mRNA
_namespace: [SMRUCC.genomics.SequenceModel.NucleotideModels](N-SMRUCC.genomics.SequenceModel.NucleotideModels.html)_





### Methods

#### Putative_mRNA
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.mRNA.Putative_mRNA(System.String,System.Int32@,System.Int32@,System.String@)
```
尝试从一段给定的核酸序列之中寻找出可能的最长的ORF阅读框

|Parameter Name|Remarks|
|--------------|-------|
|Nt|请注意这个函数总是从左往右进行计算的，所以请确保这个参数是正义链的或者反义链的已经反向互补了|
|ATG|-|
|TGA|-|



