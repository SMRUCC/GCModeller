---
title: HtseqCountMethod
---

# HtseqCountMethod
_namespace: [SMRUCC.genomics.Interops.RNA_Seq.BOW](N-SMRUCC.genomics.Interops.RNA_Seq.BOW.html)_

Counting reads in features with htseq-count, Given a file with aligned sequencing reads and a list of genomic features, a common task is to count how many reads map 
 to each feature.

> 为了得到比较好的计算性能，SAM文件之中的Reads数据首先被转换为位置数据进行缓存


### Methods

#### __getFeatures
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.HtseqCountMethod.__getFeatures(SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.GFF,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.Feature[]@,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.Feature[]@,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.FeatureKeys.Features)
```


|Parameter Name|Remarks|
|--------------|-------|
|gff|-|
|forwards|-|
|reversed|-|


#### __htSeqCount
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.HtseqCountMethod.__htSeqCount(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.Feature[]},SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.Feature[],SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.Feature[])
```
[Feature][@"F:Microsoft.VisualBasic.Constants.vbTab"][Counts]

|Parameter Name|Remarks|
|--------------|-------|
|csource|-|


#### DataFrame
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.HtseqCountMethod.DataFrame(System.Collections.Generic.IEnumerable{System.String},SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,System.Boolean)
```
包括重新排序，然后将基因名称重新换回基因号，并且在这里除以序列本身的长度，得到RPKM值

|Parameter Name|Remarks|
|--------------|-------|
|FileList|-|
|PTT|由于有些基因是不表达的，所以htseq-count计数的时候会少了一些基因，使用PTT文件的原因是补全这些基因|

> 
>  ##                 SRR479052.bam SRR479053.bam SRR479054.bam
>  ## ENSG00000000003      0             0              1
>  ## ENSG00000000005      0             0              0
>  ## ENSG00000000419      0             0              0
>  ## ENSG00000000457      0             1              0
>  ## ENSG00000000460      0             0              0
>  ## ENSG00000000938      0             0              0
>  

#### HtseqCount
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.HtseqCountMethod.HtseqCount(System.String,System.String,System.String,System.Boolean,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.FeatureKeys.Features)
```
(函数只是得到了原始计数，还需要与序列的长度相除才可以得到RPKM)

|Parameter Name|Remarks|
|--------------|-------|
|SAM|-|
|GFF|-|
|Mode|-|


#### HtseqCountBatchParallel
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.HtseqCountMethod.HtseqCountBatchParallel(System.String,System.String,System.String,System.String,System.Boolean,System.Boolean)
```
执行脚本调用本身进行批量计算(函数只是得到了原始计数，还需要与序列的长度相除才可以得到RPKM)

|Parameter Name|Remarks|
|--------------|-------|
|GFF|-|
|Mode|-|
|Parallel|内存足够大的时候可以使用这个参数，要不然计算会非常的缓慢|


#### IntersectionNonempty
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.HtseqCountMethod.IntersectionNonempty(SMRUCC.genomics.SequenceModel.SAM.DocumentElements.AlignmentReads,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.Feature[],SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.Feature[])
```
当Feature重叠在一起的时候，在内部的都计数，没有重叠的时候，可以计数

|Parameter Name|Remarks|
|--------------|-------|
|Read|-|
|GFF_Forwards|-|


#### IntersectionStrict
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.HtseqCountMethod.IntersectionStrict(SMRUCC.genomics.SequenceModel.SAM.DocumentElements.AlignmentReads,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.Feature[],SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.Feature[])
```
Read只可以出现在Feature的内部

|Parameter Name|Remarks|
|--------------|-------|
|Read|-|
|GFF_Forwards|-|


#### RPKM
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.HtseqCountMethod.RPKM(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.RNA_Seq.BOW.HtseqCountMethod.CountResult},SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.GFF)
```


|Parameter Name|Remarks|
|--------------|-------|
|dataExpr0|这里的表达量的计数全部都是原始计数|

> 好像有问题？？？

#### TrimGFF
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.HtseqCountMethod.TrimGFF(SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.GFF,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT)
```
有时候假若不需要使用基因名称，而是想要使用基因编号来表示一个基因，则可以通过这个方法将gff文件之中的基因名转换为基因号

|Parameter Name|Remarks|
|--------------|-------|
|gff|-|
|ptt|-|


#### Union
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.HtseqCountMethod.Union(SMRUCC.genomics.SequenceModel.SAM.DocumentElements.AlignmentReads,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.Feature[],SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.Feature[])
```
只要有接触的都进行计数

|Parameter Name|Remarks|
|--------------|-------|
|Read|-|
|GFF_Forwards|-|



