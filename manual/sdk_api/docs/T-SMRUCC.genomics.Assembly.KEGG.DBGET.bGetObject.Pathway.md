---
title: Pathway
---

# Pathway
_namespace: [SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject](N-SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.html)_

The kegg pathway annotation data.



### Methods

#### __parseHTML_ModuleList
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway.__parseHTML_ModuleList(System.String,SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway.LIST_TYPES)
```
Pathway和Module的格式都是一样的，所以在这里通过**type**参数来控制对象的类型

|Parameter Name|Remarks|
|--------------|-------|
|s_Value|-|
|type|-|


#### Download
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway.Download(System.String,System.String,System.String)
```
Downloads all of the available pathway information for the target species genome.(下载目标基因组对象之中的所有可用的代谢途径信息)

|Parameter Name|Remarks|
|--------------|-------|
|sp|The brief code of the target genome species in KEGG database.(目标基因组在KEGG数据库之中的简写编号.)|
|EXPORT|-|

_returns: 返回成功下载的代谢途径的数目_

#### DownloadPage
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway.DownloadPage(System.String)
```
从某一个页面url或者文件路径所指向的网页文件之中解析出模型数据

|Parameter Name|Remarks|
|--------------|-------|
|url|-|


#### GetCompoundCollection
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway.GetCompoundCollection(System.String)
```
Imports KEGG compounds from pathways model.

|Parameter Name|Remarks|
|--------------|-------|
|ImportsDIR|-|


#### GetPathwayGenes
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway.GetPathwayGenes
```
获取这个代谢途径之中的所有的基因。这个是安全的函数，当出现空值的基因集合的时候函数会返回一个空集合而非空值

#### IsContainsModule
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway.IsContainsModule(System.String)
```
Is current pathway object contains the specific module information?(当前的代谢途径对象是否包含有目标模块信息.)

|Parameter Name|Remarks|
|--------------|-------|
|ModuleId|-|



### Properties

#### Compound
The kegg compound entry collection data in this pathway.
 (可以通过这个代谢物的列表得到可以出现在当前的这个代谢途径之中的所有的非酶促反应过程，
 将整个基因组里面的化合物合并起来则可以得到整个细胞内可能存在的非酶促反应过程)
#### Modules
The module entry collection data in this pathway.
#### Name
The name value of this pathway object
