---
title: GenePromoterParser
---

# GenePromoterParser
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.html)_

直接从基因的启动子区选取序列数据以及外加操纵子的第一个基因的启动子序列



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.GenePromoterParser.#ctor(SMRUCC.genomics.SequenceModel.FASTA.FastaToken,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT)
```
基因组的Fasta核酸序列

|Parameter Name|Remarks|
|--------------|-------|
|Fasta|-|


#### CreateObject
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.GenePromoterParser.CreateObject(System.Int32,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader)
```
解析出所有基因前面的序列片段

|Parameter Name|Remarks|
|--------------|-------|
|Length|-|
|PTT|-|
|GenomeSeq|-|


#### DiffExpressionPromoters
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.GenePromoterParser.DiffExpressionPromoters(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.GenePromoterParser,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2.ResultData},System.String)
```
根据DESeq的结果得到启动子区的序列进行MEME分析

|Parameter Name|Remarks|
|--------------|-------|
|Promoter|-|
|DESeq|-|
|EXPORT|-|


#### GenerateExpressionLevelMappings
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.GenePromoterParser.GenerateExpressionLevelMappings(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2.ResultData})
```
等级映射只能够在相同的实验条件下得到的样本之中进行操作

|Parameter Name|Remarks|
|--------------|-------|
|DESeq|-|


#### GetSequenceById
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.GenePromoterParser.GetSequenceById(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.GenePromoterParser,System.Collections.Generic.IEnumerable{System.String},System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|Promoter|-|
|idList|-|
|Length|-|


#### ParsingKEGGModules
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.GenePromoterParser.ParsingKEGGModules(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.GenePromoterParser,System.String,System.String,System.String,SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.GetLocusTags)
```


|Parameter Name|Remarks|
|--------------|-------|
|modsDIR|包含有KEGG Modules的文件夹|


#### ParsingKEGGPathways
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.GenePromoterParser.ParsingKEGGPathways(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.GenePromoterParser,System.String,System.String,System.String,SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.GetLocusTags)
```
可能包含有RNA基因，故而会很容易导致出错

|Parameter Name|Remarks|
|--------------|-------|
|Parser|-|
|DOOR|-|
|PathwaysDIR|-|
|EXPORT|-|


#### ParsingList
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.GenePromoterParser.ParsingList(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.GenePromoterParser,SMRUCC.genomics.Assembly.DOOR.DOOR,System.Collections.Generic.IEnumerable{System.String},System.String,System.String,SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.GetLocusTags)
```


|Parameter Name|Remarks|
|--------------|-------|
|locus|需要进行解析的基因的编号的列表|



