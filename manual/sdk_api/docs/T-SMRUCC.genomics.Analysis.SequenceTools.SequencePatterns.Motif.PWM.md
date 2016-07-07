---
title: PWM
---

# PWM
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif](N-SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.html)_

Build probability matrix from clustal multiple sequence alignment.



### Methods

#### __hi
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.PWM.__hi(System.Collections.Generic.Dictionary{System.Char,System.Double})
```


|Parameter Name|Remarks|
|--------------|-------|
|f|-|

> 
>  If n equals ZERO, then log2(0) is NaN, n * Math.Log(n, 2) could not be measure,
>  due to the reason of ZERO multiple any number is ZERO, so that if n is ZERO, 
>  then set n * Math.Log(n, 2) its value to Zero directly.
>  

#### __residue
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.PWM.__residue(System.Collections.Generic.Dictionary{System.Char,System.Double},System.Double,System.Double,System.Int32,System.Int32)
```
Construct of the residue model in the PWM

|Parameter Name|Remarks|
|--------------|-------|
|f|ATGC|
|h|-|
|en|-|
|n|-|


#### FromMla
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.PWM.FromMla(SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
```
Build probability matrix from clustal multiple sequence alignment, this matrix model can be 
 used for the downstream sequence logo drawing visualization.
 (从Clustal比对结果之中生成PWM用于SequenceLogo的绘制)

|Parameter Name|Remarks|
|--------------|-------|
|fa|A fasta sequence file from the clustal multiple sequence alignment.|



