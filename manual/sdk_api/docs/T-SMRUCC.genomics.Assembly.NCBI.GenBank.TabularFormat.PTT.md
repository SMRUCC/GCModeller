---
title: PTT
---

# PTT
_namespace: [SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat](N-SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.html)_

The brief information of a genome.(基因组的摘要信息)



### Methods

#### ExistsLocusId
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.ExistsLocusId(System.String)
```
Does the target locus_tag exists in this genome brief data model.
 (通过基因的locus_tag, @"P:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief.Synonym"来获取某一个基因对象是否存在)

|Parameter Name|Remarks|
|--------------|-------|
|locusId|@"P:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief.Synonym"|


#### GetGeneByName
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.GetGeneByName(System.String)
```
Find gene by using genen name.

|Parameter Name|Remarks|
|--------------|-------|
|Name|基因名称，而非基因号|


#### GetObject
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.GetObject(System.Int32,System.Boolean)
```
查看某一个位点之上有哪些基因，假若需要查看某一个位点附近有哪些基因的话，则可以使用
 @"M:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.GetRelatedGenes(SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation,System.Boolean,System.Int32)"方法进行查找

|Parameter Name|Remarks|
|--------------|-------|
|site|基因组序列之上的一个碱基位点|
|ComplementStrand|该目标基因是否位于互补链之上|


#### GetRelatedGenes
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.GetRelatedGenes(SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation,System.Boolean,System.Int32)
```
获取某一个位点在位置上有相关联系的基因

|Parameter Name|Remarks|
|--------------|-------|
|loci|-|
|unstrand|-|
|ATGDist|-|


#### GetStrandGene
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.GetStrandGene(SMRUCC.genomics.ComponentModel.Loci.Strands)
```
Gets the strand specified genes.

|Parameter Name|Remarks|
|--------------|-------|
|strand|@"F:SMRUCC.genomics.ComponentModel.Loci.Strands.Forward"/@"F:SMRUCC.genomics.ComponentModel.Loci.Strands.Reverse",
 @"F:SMRUCC.genomics.ComponentModel.Loci.Strands.Unknown" will be treated as reversed.|


#### Load
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.Load(System.String,System.Boolean)
```
Load a ptt format text file, the ptt file is usually can be found at Ncbi FTP server for each species genome data.

|Parameter Name|Remarks|
|--------------|-------|
|path|Text file format ptt file path|
|fillBlank|Fill blank gene name.|


#### Read
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.Read(System.String,System.Boolean)
```
出错不会被处理，而@"M:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.Load(System.String,System.Boolean)"函数则会处理错误，返回Nothing

#### SaveXml
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.SaveXml(System.String,System.Text.Encoding)
```
Save this ptt document as an xml document.(保存Ptt数据库为Xml文件)

|Parameter Name|Remarks|
|--------------|-------|
|Path|-|
|encoding|-|


#### TryGetGeneObjectValue
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.TryGetGeneObjectValue(System.String,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief@)
```


|Parameter Name|Remarks|
|--------------|-------|
|GeneID|@"P:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief.Synonym"|
|value|-|


#### TryGetGenesId
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.TryGetGenesId(System.String)
```
由于GFF文件之中是按照GeneName来进行标识的，有些时候希望全部使用基因号进行标识，所以通过这个函数将基因名称统一转换为基因号

|Parameter Name|Remarks|
|--------------|-------|
|tag|Tag data matches in fields:
 @"P:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief.Synonym",
 @"P:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief.Gene",
 @"P:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief.PID"
 |



### Properties

#### __innerHash
{@"P:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief.Synonym", @"T:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief"}
#### GeneIDList
Gets a list of all of the gene object in this ptt document.(所有基因的基因号@"P:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief.Synonym"列表)
#### GeneObject
通过基因的Locus_Tag, @"P:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief.Synonym"来获取某一个基因对象，不存在则返回空值
#### GeneObjects
The gene brief information collection data in this genome.(这个基因组之中的所有的基因摘要数据)
#### NumOfProducts
The number of gene product counts in this genome data: @"P:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.GeneObjects".Length .(当前的基因组数据之中的基因对象的数目)
#### Size
The genome original sequence length.(基因组序列的长度)
#### Title
基因组的标题
