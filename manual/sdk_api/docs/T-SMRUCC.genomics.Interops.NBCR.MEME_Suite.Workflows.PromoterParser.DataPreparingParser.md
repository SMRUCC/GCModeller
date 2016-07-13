---
title: DataPreparingParser
---

# DataPreparingParser
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.html)_

按照给定的代谢途径或者其他的规则分组取出启动子序列，这个对象是考虑了操纵子的情况的



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.DataPreparingParser.#ctor(System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|os|全基因组核酸序列的@"T:SMRUCC.genomics.SequenceModel.FASTA.FastaToken"单序列文件的文件路径|
|Door|-|


#### ExtractPromoterRegion_Pathway
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.DataPreparingParser.ExtractPromoterRegion_Pathway(System.String)
```
代谢途径的

|Parameter Name|Remarks|
|--------------|-------|
|ExportLocation|-|


#### StringDbInteractions
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.DataPreparingParser.StringDbInteractions(System.String,System.String)
```
解析出string-db之中的蛋白质互作的网络之中的蛋白质基因的ATG上游的调控区序列

|Parameter Name|Remarks|
|--------------|-------|
|stringDIR|-|
|ExportDir|-|



