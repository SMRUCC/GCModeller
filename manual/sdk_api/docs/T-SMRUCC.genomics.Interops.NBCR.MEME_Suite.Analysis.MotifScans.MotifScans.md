---
title: MotifScans
---

# MotifScans
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.html)_

MEME操作实际上是将motif位点的序列进行分组，则某一个符合条件的位点应该不仅仅和PWM可以匹配，和生成那些位点的序列也应该匹配



### Methods

#### Mast
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.MotifScans.Mast(SMRUCC.genomics.SequenceModel.FASTA.FastaToken)
```
函数返回可能的匹配上的位点的位置以及序列片段，链的方向

|Parameter Name|Remarks|
|--------------|-------|
|Nt|-|


#### Match
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.MotifScans.Match(Microsoft.VisualBasic.ComponentModel.DataStructures.SlideWindowHandle{SMRUCC.genomics.SequenceModel.NucleotideModels.DNA},System.Double,System.Boolean,System.Int64)
```


|Parameter Name|Remarks|
|--------------|-------|
|sequence|基因组上面的滑窗位点片段|
|PWMDelta|-|
|complement|-|



