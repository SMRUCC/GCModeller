---
title: CRTMotifSearchTool
---

# CRTMotifSearchTool
_namespace: [SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel](N-SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.html)_

CRT's search for CRISPRs is based on finding a series of
 short exact repeats of length k that are separated by a similar
 distance and then extending these exact k-mer
 matches to the actual repeat length.



### Methods

#### ExactKMerMatches
```csharp
SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.CRTMotifSearchTool.ExactKMerMatches(SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid,SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.KmerProfile,System.Double,System.Int32)
```
The approach taken is this paper is to read the characters to the left or right of all repeats and compute
 occurrence percentages for each base, ACGT. If there is a character that has an occurrence percentage greater
 than or equal to some preset value, p, the repeats are extended.
 
 搜索的方法通过统计一个窗口在延展的时候，对某个碱基的出现频率进行统计，假若频率变化异常，则可能检测到了一个motif或者CRISPR位点
 
 
 假若没有搜索到位点，则会向前移动继续进行搜索

|Parameter Name|Remarks|
|--------------|-------|
|p|
 This method of extending repeats works well for CRISPRs, give an appropriate value for p.(CRT uses a default value of 75%).
 |

> 
>  The value in the search window represents
>  a candidate repeat, and each time the window reads a new
>  k-mer, the algorithm searches forward for exact k-mer
>  matches. When searching for each successive match, the
>  search space can be restricted to a small range, called
>  search range.
>  每一个窗口数据都会被当作为候选的CRISPR位点
>  当达到匹配条件的时候，搜索空间会被限制在一个很小的范围内
>  


