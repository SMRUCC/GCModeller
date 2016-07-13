---
title: MotifLoci
---

# MotifLoci
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.html)_

对MEME text里面的位点在整个基因组上面的定位



### Methods

#### __located
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifLoci.__located(SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief,SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.LDM.Site,System.Collections.Generic.Dictionary{System.String,System.Collections.Generic.KeyValuePair{System.Double,System.Int32}})
```


|Parameter Name|Remarks|
|--------------|-------|
|GeneObject|-|
|site|位点都描绘的是在所输入的@"T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile"文件之中的序列上面你的左端的起始位置|


#### Located
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifLoci.Located(SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief,SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.LDM.Site,System.Int32)
```
获取Motif位点在基因组上面的位置

|Parameter Name|Remarks|
|--------------|-------|
|gene|-|
|site|-|
|len|MEME序列的长度|



