---
title: ResourceNode
---

# ResourceNode
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat](N-SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat.html)_





### Methods

#### CopyFile
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat.ResourceNode.CopyFile(System.String,System.String,SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat.HrefLink)
```
将目标资源文件**hreflink**复制到目标目录**subject**之中

|Parameter Name|Remarks|
|--------------|-------|
|subject|这个参数是模型的主Xml文件所在的文件夹，即root文件夹|
|hreflink|-|
|source|模型文件的复制原的root文件夹|


#### WriteResource``1
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat.ResourceNode.WriteResource``1(``0,SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat.HrefLink@)
```
在保存数据之前，请设置好工作目录

|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|hreflink|-|



### Properties

#### ResourceCategory
The data storage directory name.(数据文件的存储目录)
#### TYPE_ID
The file reader required this property to located the resources
