---
title: TransUnit
---

# TransUnit
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots](N-SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.html)_

Frames in this class encode transcription units, which are defined as a set of genes and
 associated control regions that produce a single transcript. Thus, there is a one-to-one
 correspondence between transcription start sites and transcription units. If a set of genes
 is controlled by multiple transcription start sites, then a PGDB should define multiple
 transcription-unit frames, one for each transcription start site.
 (在本类型中所定义的对象编码一个转录单元，一个转录单元定义了一个基因及与其相关联的转录调控DNA片段
 的集合，故而，在本对象中有一个与转录单元相一一对应的转录起始位点。假若一个基因簇是由多个转录起始
 位点所控制的，那么将会在MetaCyc数据库之中分别定义与这些转录起始位点相对应的转录单元【即，每一个
 本类型的对象的属性之中，仅有一个转录起始位点属性】)



### Methods

#### GetGeneIds
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.TransUnit.GetGeneIds(System.String[])
```
从所有的基因标号的列表中查询出本转录单元对象中是基因对象的标识号的集合

|Parameter Name|Remarks|
|--------------|-------|
|GeneList|All Gene List|



### Properties

#### Components
The Components slot of a transcription unit lists the DNA segments within the transcription
 unit, including transcription start sites (Promoters), Terminators, DNA binding sites,
 and genes.
#### ExtentUnknown
The value of this slot should be True when it is not known to how many genes the transcription
 unit extends; that is, it is not known which is the last gene in the transcription
 unit.
