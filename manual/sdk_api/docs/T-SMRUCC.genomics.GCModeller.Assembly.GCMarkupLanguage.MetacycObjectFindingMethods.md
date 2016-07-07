---
title: MetacycObjectFindingMethods
---

# MetacycObjectFindingMethods
_namespace: [SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage](N-SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.html)_

An extension method collection of the metacyc object query for the model compiling processing.
 (对MetaCyc数据库中的目标对象的查找扩展方法的集合)



### Methods

#### GetHandles
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.MetacycObjectFindingMethods.GetHandles(SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object,SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.BacterialModel)
```
获取目标MetaCyc对象在模型中的对象句柄值集合

|Parameter Name|Remarks|
|--------------|-------|
|Object|对于模型中已经指定的类型，直接进行查找然后返回句柄值，对于在模型之中不存在的类型，则先查找出相应的对象，再返回句柄值|
|Model|-|


#### GetRegulatedObject
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.MetacycObjectFindingMethods.GetRegulatedObject(SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Regulation,SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder)
```
从MetaCyc数据库之中，查询出目标调控对象

|Parameter Name|Remarks|
|--------------|-------|
|Regulation|-|
|MetaCyc|-|


#### IndexOf
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.MetacycObjectFindingMethods.IndexOf(Microsoft.VisualBasic.List{SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction},System.String)
```
获取指定UniqueId的生化反应对象的句柄值

|Parameter Name|Remarks|
|--------------|-------|
|UniqueId|-|

_returns: Object Handle_

#### IndexOf``2
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.MetacycObjectFindingMethods.IndexOf``2(System.Collections.Generic.IEnumerable{``1},System.String)
```
MetabolismMap中的对象类型列表元素的通用查找方法

|Parameter Name|Remarks|
|--------------|-------|
|ListCollection|-|
|UniqueId|-|


#### Select``2
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.MetacycObjectFindingMethods.Select``2(SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object,System.Collections.Generic.IEnumerable{``1})
```
根据目标对象的唯一标识符，查找出模型中相应对象的句柄值

|Parameter Name|Remarks|
|--------------|-------|
|Object|-|
|Table|-|



