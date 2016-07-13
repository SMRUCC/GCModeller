---
title: TranscriptUnit
---

# TranscriptUnit
_namespace: [SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME](N-SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.html)_

在进行模型计算的时候，转录过程从这里开始，基因对象仅仅只是模板数据的承载者，
 对于一个转录单元而言，其只有一个基因的时候就仅表示该基因对象，当拥有多个基
 因的时候，则表示为一个操纵元。

> 
>  在形成调控网络的时候，调控因子对启动子的调控作用被转换为对转录单元的调控作用
>  在这里认为一个转录单元仅有一个启动子单元
>  


### Methods

#### ContainsGene
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit.ContainsGene(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|AccessionId|NCBI Accession Id|


#### Link
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit.Link(System.Collections.Generic.IEnumerable{SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.GeneObject})
```
使用MetaCyc数据库中的基因数据，对转录单元中的GeneCluster属性进行赋值

|Parameter Name|Remarks|
|--------------|-------|
|Genes|-|


#### RemoveRegulator
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit.RemoveRegulator(System.String)
```
所有具有这个ID编号的调控因子都会被移除

|Parameter Name|Remarks|
|--------------|-------|
|ID|-|



### Properties

#### BasalLevel
本底表达水平(表示在没有任何调控因子的作用下的转录水平)
#### GeneCluster
An object handle collection of the gene object that defines in the genome namespace of the model 
 file.
 (指向模型文件中的基因列表的基因对象的句柄值的集合, {MetaCycId, NCBI AccessionId})
#### MaxLevel
该转录单元的最大表达水平
#### Name
相当于@"N:SMRUCC.genomics.Assembly.MetaCyc"[MetaCyc数据库]中的@"T:SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.TransUnit"[转录单元]对象之中的@"P:SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object.CommonName"[通用名称]属性
#### PromoterGene
Promoter gene unique-id, point to the item @"P:SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels.T_MetaCycEntity`1.Identifier"
#### RegulatedMotifs
本转录单元的调控因子以及调控位点
