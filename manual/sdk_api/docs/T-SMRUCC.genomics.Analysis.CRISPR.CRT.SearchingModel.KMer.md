---
title: KMer
---

# KMer
_namespace: [SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel](N-SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.html)_

An occurrence of a CRISPR. Repetitive sequences are detected by reading a small search window 
 and then scanning ahead for exact k-mer matches separated by a similar distance.
 
 (一个可能的CRISPR位点)

> 
>  Given a k-mer that begins at position i, any
>  exact k-mer match, if one exists, should occur in the range:
>  
>  (在任何一个k-mer候选位点之中，假若存在目标位点的话，这个位点应该会出现在下面所示的搜索范围之内)
>  
>       [i + minR + minS .. i + maxR + maxS + k]
>  
>  Here, minR and maxR refer to the lengths of the smallest
>  and largest repeats to be detected.
>  
>  The lengths of spacers, which are the similarly sized non-repeating regions
>  between repeats, are referred to by minS and maxS. 
>  
>  Since CRISPRs are to some degree evenly spaced, the distance between the initial repeats can be
>  used to approximate the spacing between subsequent
>  exact k-mer matches. Thus the size of the search range can
>  be reduced further, resulting in faster processing time.
>  The size of the search range has a direct effect on the
>  processing time of the algorithm, with smaller ranges
>  being more desirable. Thus, the algorithm runs fastest
>  when there is little variation between the sizes of the
>  smallest/largest repeats and the smallest/largest spacers.
>  
>  @"F:SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.KmerProfile.minR"和@"F:SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.KmerProfile.maxR"分别为所检测到的重复位点的最小的和最大的范围
>  @"F:SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.KmerProfile.minS"和@"F:SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.KmerProfile.maxS"间隔的长度的最大值和最小值：间隔长度为非重复序列片段区域的长度
>  
>  
>  Reference:
>  
>  Jansen, R., et al. (2002). "Identification of genes that are associated with DNA repeats in prokaryotes." Molecular Microbiology 43(6): 1565-1575.
> 	Using in silico analysis we studied a novel family of repetitive DNA sequences that is present among both domains of the prokaryotes (Archaea and Bacteria), but absent from eukaryotes or viruses. This family is characterized by direct repeats, varying in size from 21 to 37 bp, interspaced by similarly sized nonrepetitive sequences. To appreciate their characteristic structure, we will refer to this family as the clustered regularly interspaced short palindromic repeats (CRISPR). In most species with two or more CRISPR loci, these loci were flanked on one side by a common leader sequence of 300-500 b. The direct repeats and the leader sequences were conserved within a species, but dissimilar between species. The presence of multiple chromosomal CRISPR loci suggests that CRISPRs are mobile elements. Four CRISPR-associated (cas) genes were identified in CRISPR-containing prokaryotes that were absent from CRISPR-negative prokaryotes. The cas genes were invariably located adjacent to a CRISPR locus, indicating that the cas genes and CRISPR loci have a functional relationship. The cas3 gene showed motifs characteristic for helicases of the superfamily 2, and the cas4 gene showed motifs of the RecB family of exonucleases, suggesting that these genes are involved in DNA metabolism or gene expression. The spatial coherence of CRISPR and cas genes may stimulate new research on the genesis and biological role of these repeats and genes.
> 
> 
>  


### Methods

#### __sequenceScan
```csharp
SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.KMer.__sequenceScan(SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid,System.Int32,SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.CRISPR,System.Int32,System.Int32,System.Double)
```
scan to the right and left of the first and last repeat to see if there is a region
that is similar to the repeats. necessary in case we missed a repeat because of
inexact matches or a result of one of the filters

|Parameter Name|Remarks|
|--------------|-------|
|side|-|
|candidateCRISPR|-|
|minSpacerLength|-|
|scanRange|-|
|confidence|-|


#### GetActualRepeatLength
```csharp
SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.KMer.GetActualRepeatLength(SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid,SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.CRISPR,SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.KmerProfile,System.Double)
```
identified repeats may represent only a subset of a larger repeat. this method extends these
 repeats as long as they continue to match within some range. assumes there are at least two repeats

|Parameter Name|Remarks|
|--------------|-------|
|nt|-|
|candidateCRISPR|-|


#### HasNonRepeatingSpacers
```csharp
SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.KMer.HasNonRepeatingSpacers(SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.CRISPR,System.Double)
```
Checks first five spacers

|Parameter Name|Remarks|
|--------------|-------|
|candidateCRISPR|-|
|maxSimilarity|-|



