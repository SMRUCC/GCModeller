---
title: MappingBuilder
---

# MappingBuilder
_namespace: [SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder](N-SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.html)_

在模型中的对象间建立连接



### Methods

#### GeneLinkMetabolism
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.MappingBuilder.GeneLinkMetabolism
```
Link the gene object to the specific metabolism reaction using its product property.(将基因对象与相应的代谢反应进行连接)

#### Link
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.MappingBuilder.Link(SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Proteins,SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.ProtLigandCplxes,SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Compounds,SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Regulations)
```
分别生成酶促反应对象的催化关系以及调控关系

|Parameter Name|Remarks|
|--------------|-------|
|Proteins|-|
|ProtLigandCplxes|-|


#### LinkGene
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Builder.MappingBuilder.LinkGene(System.String,Microsoft.VisualBasic.List{SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Protein.IEnzyme},System.Int32)
```
蛋白质复合物对基因对象的连接的递归算法

|Parameter Name|Remarks|
|--------------|-------|
|EnzymeId|-|
|EnzymeList|-|



