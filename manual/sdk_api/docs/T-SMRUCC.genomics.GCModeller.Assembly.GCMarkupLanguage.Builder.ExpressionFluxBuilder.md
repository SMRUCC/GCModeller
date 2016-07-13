---
title: ExpressionFluxBuilder
---

# ExpressionFluxBuilder
_namespace: [SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder](N-SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.html)_

表达流对象构建器，重建出目标模型的基因组、转录组

> 生成模型文件中的基因、转录单元和转录组分这三张表


### Methods

#### #ctor
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.ExpressionFluxBuilder.#ctor(SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder,SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.BacterialModel)
```


|Parameter Name|Remarks|
|--------------|-------|
|MetaCyc|-|
|Model|在模型对象之中的代谢组必须是已经构建好了的|


#### CreateTranscripts
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.ExpressionFluxBuilder.CreateTranscripts(SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder,SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.BacterialModel)
```
创建RNA分子对象，然后添加进入代谢组对象之中

|Parameter Name|Remarks|
|--------------|-------|
|MetaCyc|-|
|Model|-|


#### CreateTransUnits
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.ExpressionFluxBuilder.CreateTransUnits(SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder,SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.BacterialModel)
```
根据MetaCyc数据库模型生成转录单元对象列表

|Parameter Name|Remarks|
|--------------|-------|
|MetaCyc|-|


#### GetAllUnmodifiedProduct
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.ExpressionFluxBuilder.GetAllUnmodifiedProduct(SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder,SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.BacterialModel)
```
获取所有未经过化学修饰的蛋白质多肽链单体对象

|Parameter Name|Remarks|
|--------------|-------|
|MetaCyc|-|
|Model|-|


#### Link
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.ExpressionFluxBuilder.Link(SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.GeneObject,SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.Transcript[],Microsoft.VisualBasic.List{SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.Transcript})
```
将一个基因对象与相应的转录产物想联系起来

|Parameter Name|Remarks|
|--------------|-------|
|Gene|-|
|Transcripts|-|
|List|-|

> !!!请注意这里！！！


