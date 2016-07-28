---
title: Perl
---

# Perl
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.TSSAR](N-SMRUCC.genomics.Analysis.RNA_Seq.TSSAR.html)_

dRNA-seq
 
 TSSAR Perl invoke module
 
 DESCRIPTION
 *T*ranscription *S*tart *S*ites *A*nnotation *R*egime for dRNA-seq data,
 based on a Skellam distribution with parameter estimation by
 zero-inflated-poisson model regression analysis. The input are two
 mapped sequencing files in SAM file formate (library[+] and library[-]),
 the output is a *.BED file with an entry for each position which is
 annotated as a TSS, writen to STDOUT. Addtionally, a file named Dump.bed
 is created. It specifies regions where the applied regression model does
 not converge. Hence, those regions are omitted from analysis.

> 
>  
>  CONSIDERATIONS
>   This is only a beta-version which was not thoroughly tested.
> 
>  VERSION
>   Version 0.9.6 beta -- Distribution is modeled locally, by assuming a
>   mixed model between
> 
>   Poisson-Part -> Transcribed Region (sampling zeros)
> 
>   Zero-Part -> Not Transcribed Region (structural zeros)
> 
>   The Poisson-Part is seperated from the Zero-Part by
>   Zero-Inflated-Poisson-Model Regression Analysis. The Parameters for
>   Skellam is the winzorized mean over the Poisson-Part.
> 
>  AUTHOR
>   Fabian Amman, afabian@bioinf.uni-leipzig.de
> 
>  LICENCE
>   TSSAR itself comes under GNU General Public License v2.0
> 
>   Please note that TSSAR uses the R libraries Skellam and VGAM. Both
>   libraries are not our property and might have altering licencing. Please
>   cite independantly.
>  
>  


### Methods

#### Invoke
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSAR.Perl.Invoke(System.String,System.String,System.String,System.String,System.Int32,System.Int32,System.String,System.Int32,System.Boolean,System.Boolean,System.Boolean,System.Int32,System.Boolean)
```
*T*ranscription *S*tart *S*ites *A*nnotation *R*egime for dRNA-seq data,
 based on a Skellam distribution with parameter estimation by
 zero-inflated-poisson model regression analysis. The input are two
 mapped sequencing files in SAM file formate (library[+] and library[-]),
 the output is a *.BED file with an entry for each position which is
 annotated as a TSS, writen to STDOUT. Addtionally, a file named Dump.bed
 is created. It specifies regions where the applied regression model does
 not converge. Hence, those regions are omitted from analysis.

|Parameter Name|Remarks|
|--------------|-------|
|prorata|
 If set, the information from the SAM file how many times a read was
 mapped to the genome is used, if present. If the read maps *n* times
 to the genome, each position is counted only *1/n* times. Usefull in
 combination with e.g. segemehl mapper, which can report suboptimal
 mapping positions and/or reports all location where a read maps
 optimally. Default is off.|
|libP|
 Input library (P .. Plus; M .. Minus) in SAM format. The plus library is the one with enriched TSS 
 (for dRNA-seq this means that the plus library is the treated library, while the minus library is
 the untreated library)
 |
|libM|
 Input library (P .. Plus; M .. Minus) in SAM format. The plus library is the one with enriched TSS 
 (for dRNA-seq this means that the plus library is the treated library, while the minus library is
 the untreated library)
 |
|fasta|
 Either the location of reference genome sequence in fasta file
 format OR the genome size in *INT*. The fasta file is only used to
 parse the genome size so just one of the two must be specified.
 |
|g_size|
 Either the location of reference genome sequence in fasta file
 format OR the genome size in *INT*. The fasta file is only used to
 parse the genome size so just one of the two must be specified.
 |
|minPeak| 
 Minimal Peak size in *INT*. Only positions where read start count in
 the (+)library is greater or equal then *INT* are evaluated to be a
 TSS. Positions with less reads are seen as backgroound noise and not
 considered. Default is *3*.
 |
|pval|Maximal P-value for each position to be annotated as a TSS. Default is *1e-04*.|
|winSize|Size of the window which slides over the genome and defines the statistical properties of the local model. Default is *1,000*.|
|verbose|If set, some progress reports are printed to STDERR during computation.|
|score|
 If score mode is *p* the p-value is used as score in the TSS BED
 file. If score mode is *d* the peak difference is used as score in
 the TSS BED file. Default is *d*. Also used for clustering, which
 advices to use 'd', since the p-value often becomes zero for
 consecutive positions, thus disabling a proper merging of
 consecutive positions to the best one.
 |
|nocluster|
 If --nocluster is set all positions annotated as TSS are reported.
 If --cluster is set consecutive TSS positions are clustered and only
 the 'best' position is reported. 'Best' position depends on the
 setting of --score (see above). Either the position with the lowest
 p-Value or the position with the highest peak difference between
 plus and minus library is reported. Default is --cluster. The option
 --range defines the maximal distance for two significant positions
 to be called 'consecutive'.
 |
|range|
 The maximal distance for two significant positions to be be
 clustered together if option --cluster is set. Default is *3* nt. If
 --cluster is set to --nocluster, --range is ignored.|
|clean|
 If --clean is set, all temporary files which are created during the
 computation are deleted afterwards. With --noclean they are stored.
 Mainly for debugging purpose. Default setting is --clean.|

> 
>  这个Perl和R脚本的执行效率太低了！
>  

#### Located
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSAR.Perl.Located(System.Collections.Generic.IEnumerable{SMRUCC.genomics.SequenceModel.SAM.AlignmentReads},SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT)
```


|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|Ptt|参考基因组之中的基因的摘要信息|



### Properties

#### R
R程序的路径
