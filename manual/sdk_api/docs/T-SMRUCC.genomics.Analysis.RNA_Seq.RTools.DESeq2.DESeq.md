---
title: DESeq
---

# DESeq
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2](N-SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2.html)_

Differential analysis of count data – the DESeq2 package
 (M. I. Love, W. Huber, S. Anders: Moderated estimation of fold change And dispersion For RNA-Seq data With DESeq2. bioRxiv(2014).doi : 10.1101/002832)
 
 A basic task in the analysis of count data from RNA-Seq is the detection of differentially
 expressed genes. The count data are presented As a table which reports, For Each sample, the
 number Of sequence fragments that have been assigned To Each gene. Analogous data also arise
 
 For other assay types, including comparative ChIP-Seq, HiC, shRNA screening, mass spectrometry.
 An important analysis question Is the quantification And statistical inference Of systematic changes
 between conditions, as compared To within-condition variability. The package DESeq2 provides
 methods To test For differential expression by use Of negative binomial generalized linear models;
 the estimates Of dispersion And logarithmic fold changes incorporate data-driven prior distributions.
 
 This vignette explains the use of the package And demonstrates typical work flows. 
 Another vignette, “Beginner’s guide to using the DESeq2 package”, covers similar material but at a slower
 pace, including the generation Of count tables from FASTQ files.

> 
>  Welcome to 'DESeq'. For improved performance, usability and functionality, please consider migrating to 'DESeq2'.
>  虽然模块的名称是DESeq，但是在R之中实际调用的包确是DESeq2
>  


### Methods

#### DESeq2
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2.DESeq.DESeq2(System.Collections.Generic.IEnumerable{System.Collections.Generic.IEnumerable{System.String}},System.String)
```
HTSeq-Count

|Parameter Name|Remarks|
|--------------|-------|
|TestData|
 samFile, condition;
 samFile, condition;
 
 |


#### FilterDifferentExpression
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2.DESeq.FilterDifferentExpression(System.String,System.Double)
```
导出差异表达的基因

#### HTSeqCount
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2.DESeq.HTSeqCount(System.String,System.String,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|SAM|-|
|GFF|-|
|PairedEnd|假若是Paired-End的比对数据，则还需要首先使用samtools工具进行排序|


#### Initialize
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2.DESeq.Initialize(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|R_HOME|-|

> 
>  For counting aligned reads in genes, the summarizeOverlaps function of GenomicAlignments 
>  With mode="Union" Is encouraged, resulting In a SummarizedExperiment Object(easyRNASeq Is 
>  another Bioconductor package which can prepare SummarizedExperiment objects as input for DESeq2).
>  
>  为了计数基因的比对Reads数目， GenomicAlignments 包之中的summarizeOverlaps方法可以生成DESeq2所需要用到的数据
>  easyRNASeq包也可以用来生成这种数据
>  

#### summarizeOverlaps
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2.DESeq.summarizeOverlaps(System.Collections.Generic.IEnumerable{System.String},System.String)
```
Counting reads with summarizeOverlaps, Perform overlap queries between reads and genomic features

|Parameter Name|Remarks|
|--------------|-------|
|annoGFF|GTF基因组注释文件的文件路径|
|bamList|Mapping所得到的*.bam二进制文件的文件路径的列表|

> 
>  作者推荐使用这个函数方法的模式
>  
>  ## mode funtions
>  Union(features, reads, ignore.strand=FALSE, inter.feature=TRUE)
>  


