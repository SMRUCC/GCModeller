---
title: ColorSchema
---

# ColorSchema
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo](N-SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo.html)_

Define two prefix color schema for the sequence logo: @"P:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo.ColorSchema.NT" and @"P:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo.ColorSchema.AA".
 (包含有两种默认的颜色模式：核酸序列和蛋白质序列)

> 由于可能会涉及到并行化的原因，
>  多线程操作图片对象很可能会出现@"T:System.InvalidOperationException": Object is currently in use elsewhere.的错误
>  所以在这里不再使用只读属性的简写形式
>  


### Methods

#### __getTexture
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo.ColorSchema.__getTexture(System.Drawing.Color,System.String)
```
Creates the image cache for the alphabet.

|Parameter Name|Remarks|
|--------------|-------|
|color|-|
|alphabet|-|



### Properties

#### AA
Enumeration for amino acid.
#### NT
Enumeration for nucleotide residues
#### NucleotideSchema
Color schema for the nucleotide sequence.(核酸Motif的profiles)
#### ProteinSchema
Color schema for the protein residues alphabets.(蛋白质Motif的profiles)
