---
title: DESeq2Diff
---

# DESeq2Diff
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2](N-SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2.html)_

没有基因的表达数据，只有变化值

> 
>  请注意在这里面的treated vs untreated就是~condition的对比，可以看作为NY vs MMX
>  



### Properties

#### baseMean
The base mean over all rows.
 (表达量变化是identical的基因可以直接使用这个值来作为表达量)
#### lfcSE
standard error: condition treated vs untreated
#### log2FoldChange
log2 fold change (MAP): condition treated vs untreated
#### padj
BH adjusted p-values
#### pvalue
Wald test p-value: condition treated vs untreated
#### stat
Wald statistic: condition treated vs untreated
