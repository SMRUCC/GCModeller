---
title: Extensions
---

# Extensions
_namespace: [SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage](N-SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.html)_





### Methods

#### AsMetabolites
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Extensions.AsMetabolites(Microsoft.VisualBasic.List{System.String})
```
将Unique列表转换为代谢物的引用类型列表

|Parameter Name|Remarks|
|--------------|-------|
|List|-|


#### IndexOf
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Extensions.IndexOf(System.Collections.Generic.IEnumerable{SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite},System.String)
```
依照标号查询列表中的某一个代谢物的句柄值

|Parameter Name|Remarks|
|--------------|-------|
|List|-|
|UniqueId|-|


#### IsRegulator
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Extensions.IsRegulator(SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.Regulator,SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Regulations)
```
判断某一个实体对象是否为调控因子

|Parameter Name|Remarks|
|--------------|-------|
|Entity|-|
|Regulations|-|


#### Select
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Extensions.Select(Microsoft.VisualBasic.List{SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Protein.IEnzyme},System.String)
```
Query target enzyme object using its unique id property from a enzyme list.(依照对象的UniqueId属性在酶催化剂分子列表之中查找出目标对象)

|Parameter Name|Remarks|
|--------------|-------|
|List|-|
|UniqueId|-|


#### Takes
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.Extensions.Takes(SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.GeneObject[],Microsoft.VisualBasic.List{System.String})
```
使用基因标号从基因列表之中提取出一个基因对象的集合

|Parameter Name|Remarks|
|--------------|-------|
|Genes|-|
|IdCollection|-|



### Properties

#### RegulationTypes
All of the gene expression regulation types that listed in the MetaCyc database.(在MetaCyc数据库中所列举的所有基因表达调控类型)
