---
title: PartialBestBLAST
---

# PartialBestBLAST
_namespace: [SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application](N-SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.html)_

部分最佳比对BLAST



### Methods

#### Peformance
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.PartialBestBLAST.Peformance(System.String,System.String,Microsoft.VisualBasic.Text.TextGrepMethod,Microsoft.VisualBasic.Text.TextGrepMethod,System.String,System.Boolean)
```
执行BLASTP操作，返回单项部分匹配的最佳匹配的蛋白质列表

|Parameter Name|Remarks|
|--------------|-------|
|Query|-|
|Subject|匹配上Query的部分区域但是Subject对象为完全匹配的目标|
|HitsGrepMethod|对Hit蛋白质序列的FASTA数据库中的基因号的解析方法|
|QueryGrepMethod|对Query蛋白质序列的FASTA数据库中的基因号的解析方法|

_returns: 返回双相匹配的BestHit列表_


### Properties

#### LocalBLAST
本地BLAST的中间服务
