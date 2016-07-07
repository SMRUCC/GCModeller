---
title: Genes
---

# Genes
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles](N-SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.html)_

Each frame in the class Genes describes a single gene, meaning a region of DNA that defines a 
 coding region for one or more gene products. Multiple gene products may be produced because 
 of modification of an RNA or protein.



### Methods

#### GetAllGeneIds
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Genes.GetAllGeneIds
```
获取所有基因对象的UniqueId所组成的集合

#### TryParsePromoters
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Genes.TryParsePromoters(SMRUCC.genomics.SequenceModel.FASTA.FastaToken)
```
尝试着取出所有的基因对象的启动子序列，结果不是很准确，请慎用！！！


