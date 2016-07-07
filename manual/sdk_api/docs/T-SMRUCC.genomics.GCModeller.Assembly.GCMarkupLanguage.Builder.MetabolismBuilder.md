---
title: MetabolismBuilder
---

# MetabolismBuilder
_namespace: [SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder](N-SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.html)_





### Methods

#### BuildPathwayMap
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.MetabolismBuilder.BuildPathwayMap(SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder,SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.BacterialModel)
```
依照MetaCyc数据库中的代谢途径对象的定义将相对应的反应对象加入到相对应的代谢途径对象中

#### GetReactionHandles
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.MetabolismBuilder.GetReactionHandles(SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Pathway,SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder,SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.BacterialModel)
```
递归的获取某一个代谢途径对象中的所有的生化反应对象的句柄值

|Parameter Name|Remarks|
|--------------|-------|
|Pathway|-|


#### InsertReaction
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.MetabolismBuilder.InsertReaction(SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Reaction,SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.BacterialModel)
```
将MetaCyc数据库中相对应的反应对象添加进入模型之中并返回模型的代谢组网络在执行添加操作之后的节点数目

#### IsChemicalReaction
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.MetabolismBuilder.IsChemicalReaction(SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Reaction)
```
目标对象不是蛋白质反应，不是转运反应，不是Protein-Ligand-Binding-Reactions，不是Protein-Modification-Reactions则进行添加

|Parameter Name|Remarks|
|--------------|-------|
|rxn|-|


#### Link
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.MetabolismBuilder.Link(SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reactions,SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Enzrxns,SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.BacterialModel)
```


|Parameter Name|Remarks|
|--------------|-------|
|RxnCollections|reactions.dat|
|EnzRxn|enzrxns.dat|


#### TrimMetabolites
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.MetabolismBuilder.TrimMetabolites(SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder,SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.BacterialModel)
```

> 
>  在Model.PreLoad方法之中，程序已经将SBML模型文件之中所列举的代谢物都加载进入模型之中了，
>  在本过程中则是将MetaCyc数据库之中的RNA，蛋白质单体，蛋白质复合物对象都加载进入模型之中
>  


