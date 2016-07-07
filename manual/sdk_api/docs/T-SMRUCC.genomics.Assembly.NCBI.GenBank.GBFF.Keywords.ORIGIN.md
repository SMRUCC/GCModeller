---
title: ORIGIN
---

# ORIGIN
_namespace: [SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords](N-SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.html)_

This GenBank keyword section stores the sequence data for this database.



### Methods

#### GetFeatureSegment
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.ORIGIN.GetFeatureSegment(SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature)
```
获取该Feature位点的序列数据

#### ToFasta
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.ORIGIN.ToFasta
```
Returns the whole genome sequence which was records in this GenBank database file.
 (返回记录在本Genbank数据库文件之中的全基因组核酸序列)


### Properties

#### GCSkew
是整条序列的GC偏移
#### SequenceData
The sequence data that stores in this GenBank database, which can be a genomics DNA sequence, protein sequence or RNA sequence.(序列数据，类型可以包括基因组DNA序列，蛋白质序列或者RNA序列)
#### Size
基因组的大小
