---
title: Extensions
---

# Extensions
_namespace: [SMRUCC.genomics.Assembly.NCBI.GenBank](N-SMRUCC.genomics.Assembly.NCBI.GenBank.html)_





### Methods

#### __protShort
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.Extensions.__protShort(SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature,System.Boolean)
```
假若是新注释的基因组还没有基因号，则默认使用位置来做唯一标示

|Parameter Name|Remarks|
|--------------|-------|
|feature|-|
|onlyLocusTag|-|


#### ExportProteins
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.Extensions.ExportProteins(SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File)
```
Export protein sequence with full annotation.

|Parameter Name|Remarks|
|--------------|-------|
|Gbk|-|


#### ExportProteins_Short
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.Extensions.ExportProteins_Short(SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File,System.Boolean)
```
Locus_tag Product_Description

|Parameter Name|Remarks|
|--------------|-------|
|gb|-|


#### GeneList
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.Extensions.GeneList(SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File)
```

_returns: {locus_tag, gene}_


