---
title: IntergenicSigma70
---

# IntergenicSigma70
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.html)_

应用于Sigma70的预测分析的序列片段的解析模块
 
 只解析出在基因间隔区的，ORF上游100bp的序列片段，假若片段和其他的ORF发生了重叠，就缩短片段，但是不得短于25bp

> 
>  Initially, all 100 nt regions upstream of all protein encoding genes were selected from the genome. 
>  
>  Subsequently, these sequences were evaluated for their potential overlap With a preceding gene, 
>  And In such cases, only the intergenic sequence was used For analysis, provided they
>  were 25 nt Or longer.
>  
>  The resulting Set Of selected intergenic sequences was searched For conserved motifs using MEME, applying standard DNA
>  parameter settings. 
>  
>  Only motifs reported by MEME With E-values below 10-4 were considered relevant for further analysis.
>  
>  Additional parameters used For selection Of candidate sequences were: 
>  
>  1) Zero Or One Occurrence Per Sequence (ZOOPS mode),
>  2) a maximum of ten different motifs per sequence, And 
>  3) each motif should be found In at least thirty-five different sequences.
>  
>  PWM models were constructed For the most abundantly encountered motifs, including those resembling the canonical 235 And
>  210 elements known from general s70-dependent promoters.
>  
>  黄单胞菌应该长一点150bp，非严格重叠？？？？
>  


### Methods

#### DIPAssociation
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.IntergenicSigma70.DIPAssociation(System.String,System.String)
```
The canonical sigma factor sigma70 is commonly involved in transcription of the cell’s housekeeping genes, which is mediated by the conserved sigma70 promoter sequence motifs

|Parameter Name|Remarks|
|--------------|-------|
|MEMECSV|-|
|DIPCsv|-|


#### MEMEPredictedTSSsAssociations
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.IntergenicSigma70.MEMEPredictedTSSsAssociations(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.IntergenicSigma70.Transcript},SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.LDM.Motif},System.Int32)
```
将MEME的-10区预测结果和转录组装配所得到的结果进行整合以了解二者的分析结果的一致性如何

|Parameter Name|Remarks|
|--------------|-------|
|Transcripts|-|
|PTT|-|
|Length|-|

> 函数只关联在MEME之中出现的基因，其他的都会留下空值

#### OverlapCommon
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.IntergenicSigma70.OverlapCommon(SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief[],SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief,System.Int32)
```
严格或者非严格实际上就只是数据源不同：
 严格的灰同时检查两条链，所以数据是整个PTT文件
 非严格的则是与目标基因相同链的基因数据

|Parameter Name|Remarks|
|--------------|-------|
|Genes|-|
|Reader|-|
|GeneObject|-|


#### Sigma70Parser
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.IntergenicSigma70.Sigma70Parser(SMRUCC.genomics.SequenceModel.FASTA.FastaToken,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.LDM.Motif},System.Boolean,System.Int32,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|genomeOS|The genome origin sequence.|
|PTT|-|
|Motifs|-|
|StrictOverlap|-|
|Length|-|
|EXPORT|-|


#### TrimNotStrictOverlap
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.IntergenicSigma70.TrimNotStrictOverlap(SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief,System.Int32)
```
只要目标位点和基因不在同一条链之上，就不算是重叠
 
 这个函数只需要对同一条链之上的基因进行计算就可以了

|Parameter Name|Remarks|
|--------------|-------|
|Reader|-|
|PTT|-|
|GeneObject|-|


#### TrimStrictOverlap
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.IntergenicSigma70.TrimStrictOverlap(SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief,System.Int32)
```
不管链的方向，只要发生了重叠就必须要剪裁

|Parameter Name|Remarks|
|--------------|-------|
|Reader|-|
|PTT|-|
|GeneObject|-|


#### VirtualFootprintDIP
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser.IntergenicSigma70.VirtualFootprintDIP(System.String,System.String)
```
为什么解释器就是找不到这个函数的入口点？？？？？

|Parameter Name|Remarks|
|--------------|-------|
|Csv|-|
|DIPCsv|-|



