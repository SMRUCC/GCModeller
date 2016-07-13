---
title: MEME_TEXT
---

# MEME_TEXT
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.Text](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.Text.html)_

MEME - Motif discovery tool

> 
>  ********************************************************************************
>  MEME - Motif discovery tool
>  ********************************************************************************
>  MEME version 3.5.4 (Release date: 3.5.4)
>  
>  For further information on how to interpret these results or to get
>  a copy of the MEME software please access http://meme.nbcr.net.
>  
>  This file may be used as input to the MAST algorithm for searching
>  sequence databases for matches to groups of motifs.  MAST is available
>  for interactive use and downloading at http://meme.nbcr.net.
>  ********************************************************************************
>  
>  
>  ********************************************************************************
>  REFERENCE
>  ********************************************************************************
>  If you use this program in your research, please cite:
>  
>  Timothy L. Bailey and Charles Elkan,
>  "Fitting a mixture model by expectation maximization to discover
>  motifs in biopolymers", Proceedings of the Second International
>  Conference on Intelligent Systems for Molecular Biology, pp. 28-36,
>  AAAI Press, Menlo Park, California, 1994.
>  ********************************************************************************
>  


### Methods

#### __createBlockDiagrams
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.Text.MEME_TEXT.__createBlockDiagrams(System.String[])
```
可能会有完全一样的出现

|Parameter Name|Remarks|
|--------------|-------|
|str|-|


#### __tryParseMotif
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.Text.MEME_TEXT.__tryParseMotif(System.String,System.String)
```
MOTIF 1width = 21 sites = 12 llr = 178 E-value = 4.6e-004

|Parameter Name|Remarks|
|--------------|-------|
|strData|-|


#### DistanceNormalization
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.Text.MEME_TEXT.DistanceNormalization(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,System.String,System.String)
```
计算基因组之间的距离，使用这个文件之中的数据利用飞利浦软件里面的Distance matrix methods方法进行进化树的构建

|Parameter Name|Remarks|
|--------------|-------|
|csv|-|
|faDIR|-|
|queryref|参照的基因组的编号|


#### ExportMotif
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.Text.MEME_TEXT.ExportMotif(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|MEME_Text|MEME text motif 文档的文件路径|


#### GetLength
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.Text.MEME_TEXT.GetLength(System.String)
```
函数自动从meme.text文档里面解析出序列数据源的长度参数，假若你不太方便手工输入序列长度的话

|Parameter Name|Remarks|
|--------------|-------|
|path|-|


#### Load
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.Text.MEME_TEXT.Load(System.String,System.Boolean)
```
Load the motif data from the meme text format calculation result

|Parameter Name|Remarks|
|--------------|-------|
|path|-|


#### Normalization
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.Text.MEME_TEXT.Normalization(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,System.String,System.Int32)
```
计算单位段长度之内的motif出现的频率的高低

|Parameter Name|Remarks|
|--------------|-------|
|csv|-|
|faDIR|-|
|scale|-1表示使用自动配置的scale参数，其他的非零的正数则表示指定扩大的级别|

> 
>  可以使用本方法所生成的矩阵进行Gene Frequencies and Continuous Character Data Programs的方法进行进化树的绘制
>  
>  phylip软件之中的帮助说明
>  
>  The programs in this group use gene frequencies and quantitative character values. One (Contml) constructs maximum likelihood estimates of the phylogeny, another (Gendist) computes genetic distances for use in the distance matrix programs, and the third (Contrast) examines correlation of traits as they evolve along a given phylogeny.
>  
>  When the gene frequencies data are used in Contml or Gendist, this involves the following assumptions:
>  
>  Different lineages evolve independently.
>  After two lineages split, their characters change independently.
>  Each gene frequency changes by genetic drift, with or without mutation (this varies from method to method).
>  Different loci or characters drift independently.
>  How these assumptions affect the methods will be seen in my papers on inference of phylogenies from gene frequency and continuous character data (Felsenstein, 1973b, 1981c, 1985c).
>  

#### SafelyLoad
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.Text.MEME_TEXT.SafelyLoad(System.String,System.Boolean)
```
发生错误会返回空值

|Parameter Name|Remarks|
|--------------|-------|
|path|-|


#### Statics
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.Text.MEME_TEXT.Statics(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.LDM.Motif},System.Func{System.String,System.String})
```


|Parameter Name|Remarks|
|--------------|-------|
|Motifs|-|
|getsId|获取基因组编号的函数指针|

> 
>           motif1 motif2 motif3
>  genome1
>  genome2
>  genome3
>  


