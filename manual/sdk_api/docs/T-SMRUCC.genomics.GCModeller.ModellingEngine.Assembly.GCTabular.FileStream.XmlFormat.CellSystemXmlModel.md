---
title: CellSystemXmlModel
---

# CellSystemXmlModel
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat](N-SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat.html)_

本计算模型中的所构建的细胞中的基本系统：代谢组和转录组，请注意，对于本对象的属性中的路径对象，当在编译器阶段的时候为一个绝对路径，但是当执行了保存动作之后，都将变为相对路径



### Methods

#### LoadXml
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat.CellSystemXmlModel.LoadXml(System.String)
```
请使用本方法来加载资源数据

|Parameter Name|Remarks|
|--------------|-------|
|sPath|-|


#### Save
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat.CellSystemXmlModel.Save(System.String)
```
这个方法仅仅是保存当前的这个xml文件对象

|Parameter Name|Remarks|
|--------------|-------|
|FilePath|-|


#### SaveOrCopy
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat.CellSystemXmlModel.SaveOrCopy(System.String)
```
由于本文件仅仅是一个资源的连接文件，故而在保存数据的时候，是不知道所要进行保存的数据的具体格式的，
 故而在本方法之中，仅仅是根据@"P:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat.CellSystemXmlModel.ResourceMapper"之中的连接指针
 将所指向的目标资源对象复制到**FilePath**的文件目录之中。参数
 **FilePath**仅仅适用于表示主XML连接文件的文件路径，故而当该参数的父文件夹
 与当前的模型文件所处的父文件夹一致的时候，仅仅会保存XML主文件，当不同的时候，会进行资源文件的复制操作

|Parameter Name|Remarks|
|--------------|-------|
|FilePath|-|



### Properties

#### ResourceCollection
XML模型文件之中的资源连接数据都是存储在这个属性之中的，当加载的时候，就会通过本属性来讲值赋值给其他的属性
