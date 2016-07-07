---
title: Installer
---

# Installer
_namespace: [SMRUCC.genomics.Data](N-SMRUCC.genomics.Data.html)_





### Methods

#### BuildLocusHash
```csharp
SMRUCC.genomics.Data.Installer.BuildLocusHash(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Data.GeneInfo},SMRUCC.genomics.Data.GenbankIndex)
```
key: @"P:SMRUCC.genomics.Data.GeneInfo.locus_tag"

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|x|-|


#### BuildNameHash
```csharp
SMRUCC.genomics.Data.Installer.BuildNameHash(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Data.GeneInfo},SMRUCC.genomics.Data.GenbankIndex)
```
key: @"P:SMRUCC.genomics.Data.GeneInfo.name", @"P:SMRUCC.genomics.Data.GeneInfo.locus_tag"

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|x|-|


#### GetsiRNATargetSeqs
```csharp
SMRUCC.genomics.Data.Installer.GetsiRNATargetSeqs(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.Bac_sRNA.org.Interaction},SMRUCC.genomics.Data.Genbank)
```
Query target nt sequence

|Parameter Name|Remarks|
|--------------|-------|
|siRNAtarget|
 这个应该是通过对@"P:SMRUCC.genomics.Assembly.Bac_sRNA.org.Interaction.Organism"Group之后所得到的数据
 |
|repo|-|


#### Install
```csharp
SMRUCC.genomics.Data.Installer.Install(System.String,System.Boolean)
```
这个函数主要是进行创建数据库的索引文件

|Parameter Name|Remarks|
|--------------|-------|
|DIR|Extract location of file: all.gbk.tar.gz from NCBI FTP website.|



