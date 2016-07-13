---
title: BidirectionalBesthit_BLAST
---

# BidirectionalBesthit_BLAST
_namespace: [SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH](N-SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.html)_

A tight link between orthologs and bidirectional best hits in bacterial and archaeal genomes. BBH.(通过BLASTP操作来获取两个基因组之间的相同的蛋白质对象)



### Methods

#### Paralogs
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BidirectionalBesthit_BLAST.Paralogs(System.String,Microsoft.VisualBasic.Text.TextGrepMethod)
```
自身进行比较去除最佳比对获取旁系同源

|Parameter Name|Remarks|
|--------------|-------|
|Fasta|-|


#### Peformance
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BidirectionalBesthit_BLAST.Peformance(System.String,System.String,Microsoft.VisualBasic.Text.TextGrepMethod,Microsoft.VisualBasic.Text.TextGrepMethod,System.String,System.Boolean)
```
执行BLASTP操作，返回双向最佳匹配的蛋白质列表

|Parameter Name|Remarks|
|--------------|-------|
|Query|-|
|Subject|-|
|HitsGrepMethod|对Hit蛋白质序列的FASTA数据库中的基因号的解析方法|
|QueryGrepMethod|对Query蛋白质序列的FASTA数据库中的基因号的解析方法|
|ExportAll|假若为真的话，则会导出所有的最佳结果，反之，则会导出直系同源的基因|

_returns: 返回双相匹配的BestHit列表_


### Properties

#### _LocalBLASTService
本地BLAST的中间服务
