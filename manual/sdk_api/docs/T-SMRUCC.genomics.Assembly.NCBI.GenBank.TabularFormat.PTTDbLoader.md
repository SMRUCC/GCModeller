---
title: PTTDbLoader
---

# PTTDbLoader
_namespace: [SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat](N-SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.html)_





### Methods

#### #ctor
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTTDbLoader.#ctor(System.String)
```
请注意，通过这个构造函数只能够读取一个数据库的数据，假若一个文件夹里面同时包含有了基因组数据和质粒的数据，则不推荐使用这个函数进行构造加载

|Parameter Name|Remarks|
|--------------|-------|
|DIR|-|


#### CreateObject
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTTDbLoader.CreateObject(System.Collections.Generic.IEnumerable{SMRUCC.genomics.ComponentModel.IGeneBrief},SMRUCC.genomics.SequenceModel.FASTA.FastaToken)
```
从其他的数据类型进行数据转换，转换数据格式为PTT格式，以方便用于后续的分析操作用途

#### ExportCOGProfiles``1
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTTDbLoader.ExportCOGProfiles``1
```
从PTT基因组注释数据之中获取COG分类数据

#### GenomeFasta
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTTDbLoader.GenomeFasta
```
(*.fna)(基因组的全长序列)

#### GetGeneUniqueId
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTTDbLoader.GetGeneUniqueId(System.String)
```
基因序列中的pid似乎是无效的，都一样的，只能通过location来获取序列的标识号了

|Parameter Name|Remarks|
|--------------|-------|
|Location|-|


#### GetRelatedGenes
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTTDbLoader.GetRelatedGenes(System.Int32,System.Int32,System.Int32)
```
This function will ignore the nucleoside direction adn founds the gene on both strand of the DNA.(将会忽略DNA链，两条链的位置上都会寻找)

|Parameter Name|Remarks|
|--------------|-------|
|LociStart|-|
|LociEnds|-|


#### ORF_PTT
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTTDbLoader.ORF_PTT
```
*.ptt

#### RNARnt
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTTDbLoader.RNARnt
```
*.rpt


### Properties

#### _genomeContext
整个基因组中的所有基因的集合，包括有蛋白质编码基因和RNA基因
#### RptGenomeBrief
*.rpt
