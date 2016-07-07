---
title: IdentifyUTRs
---

# IdentifyUTRs
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs](N-SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.html)_

For each gene, identify its 5'UTR and 3'UTR based on the expression data.



### Methods

#### __analysis
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.IdentifyUTRs.__analysis(System.Int32,System.Boolean,System.Int32,System.Int64,SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.DocumentFormat.Transcript[],System.Collections.Generic.SortedDictionary{System.String,SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.DocumentFormat.Transcript}@,SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.Replicate@)
```
For each IG region

|Parameter Name|Remarks|
|--------------|-------|
|x|-|
|unstranded|-|
|replicate|-|
|genome|真实的基因组上下文或者假基因种子，请注意ORF一定要填满ATG和TGA位点的值|


#### __dataPartitionings
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.IdentifyUTRs.__dataPartitionings(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.ReadsCount},System.Int32,System.Int32,System.Int64,System.Int32,System.Boolean,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|Transcripts|-|
|sharedReads|-|
|genomeSize|-|
|readsLen|-|
|unstrand|-|
|siRNAPredicts|筛选的模式会反转|


#### __genomeAssumption
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.IdentifyUTRs.__genomeAssumption(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.DocumentFormat.Transcript},System.Int64)
```
从转录组数据之中生成假基因

|Parameter Name|Remarks|
|--------------|-------|
|Transcripts|-|


#### __getPoissonPDF
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.IdentifyUTRs.__getPoissonPDF(System.Int32,System.Double)
```
Returns the PDF value at x for the specified Poisson distribution.

#### __setBoundary
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.IdentifyUTRs.__setBoundary(SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.DocumentFormat.Transcript,System.Collections.Generic.SortedDictionary{System.String,Microsoft.VisualBasic.ComponentModel.Value{System.Int32}},System.Collections.Generic.SortedDictionary{System.String,Microsoft.VisualBasic.ComponentModel.Value{System.Int32}})
```
设置左右端起始和终止的位点，非ATG和TGA

|Parameter Name|Remarks|
|--------------|-------|
|Transcript|-|
|gStart|-|
|gStop|-|


#### GenomicsContext
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.IdentifyUTRs.GenomicsContext(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.DocumentFormat.Transcript},SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,System.Int32)
```
这个函数则会将TSSs和TTS组装在一个构成完整的基因结构的信息

|Parameter Name|Remarks|
|--------------|-------|
|Sites|includes TSSs/TTS|
|PTT|-|


#### identifyUTRs
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.IdentifyUTRs.identifyUTRs(SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,System.Boolean,System.String,System.Int32,System.Double,System.String)
```
For each gene, identify its 5'UTR and 3'UTR based on the expression data.(使用现有的基因组上下文数据)

|Parameter Name|Remarks|
|--------------|-------|
|Transcripts|File path of the RNA-seq @"T:SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.DocumentFormat.Transcript" csv document|
|minExpression|0-1之间的一个数|


#### siRNAPredictes
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.IdentifyUTRs.siRNAPredictes(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.ReadsCount},System.Int64,System.Boolean,System.Int32,System.Int32,System.Int32)
```
和TSS不同的是，小RNA分子的表达量一般较低，所以在这里对原始数据是反向筛选的

|Parameter Name|Remarks|
|--------------|-------|
|readsCount|-|
|genomeSize|-|
|unstranded|-|
|sharedReads|-|
|minIGD|-|


#### TestSites
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.IdentifyUTRs.TestSites(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.ReadsCount},System.Int64,System.Boolean,System.Int32,System.Int32)
```
Testing if the site can be identified as a TSSs.(使用种子来鉴定)

|Parameter Name|Remarks|
|--------------|-------|
|Transcripts|-|

_returns: 程序会尝试延伸，假若不能够继续延伸，则认为是转录边界
 由于序列片段之间会存在重叠的情况，所以在计算之前需要先分区，将序列分割为非重叠的状态，即序列片段之间的最小距离要满足一个用户自定义的基因间隔区的最小距离
 
 _


