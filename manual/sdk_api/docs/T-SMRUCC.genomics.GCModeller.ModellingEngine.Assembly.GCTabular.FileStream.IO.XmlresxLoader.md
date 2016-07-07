---
title: XmlresxLoader
---

# XmlresxLoader
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO](N-SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO.html)_

The GCModeller cellular network xml model component resource entry loader.(GCModeller虚拟细胞计算模型的资源加载器)



### Methods

#### #ctor
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO.XmlresxLoader.#ctor(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|CellSystemPath|@"T:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat.CellSystemXmlModel"[Cell system model xml] file path.|


#### CreateObject
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO.XmlresxLoader.CreateObject
```
Create a new and empty gcml csvx resource loader object.(创建一个全新的空的资源加载器Loader对象)

#### Encryption
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO.XmlresxLoader.Encryption
```
模型本身+最后一行的MD5校验码

#### InternalLoadResourceData
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO.XmlresxLoader.InternalLoadResourceData
```
加载Xml文件之中的属性所指向的Excel资源文件之中的数据至网络模型之中

#### SetExportDirectory
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO.XmlresxLoader.SetExportDirectory(System.String)
```
设置模型数据 @"P:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO.XmlresxLoader.CellSystemModel" 的导出路径

|Parameter Name|Remarks|
|--------------|-------|
|dirPath|-|



### Properties

#### _InternalCellSystemResourceManager
资源管理器的内部入口点的挂载对象
#### _InternalModelParentDirRoot
资源数据的总入口
#### CheBMethylesterase
[MCP][CH3] -> MCP + -CH3 Enzyme:[CheB][PI]
 
 Protein L-glutamate O(5)-methyl ester + H(2)O = protein L-glutamate + methanol
 C00132

 METOH
#### CheBPhosphate
CheB + [ChA][PI] -> [CheB][PI] + CheA
#### ChemotaxisSensing
[MCP][CH3] + Inducer <--> [MCP][CH3][Inducer]
#### CheRMethyltransferase
MCP + -CH3 -> [MCP][CH3] Enzyme:CheR
 S-adenosyl-L-methionine
 S-ADENOSYLMETHIONINE
 C00019
 
 S-ADENOSYLMETHIONINE ADENOSYL-HOMO-CYS
 S-adenosyl-L-methionine + protein L-glutamate = S-adenosyl-L-homocysteine + protein L-glutamate methyl ester.
#### CrossTalk
[CheAHK][PI] + RR -> [RR][PI] + CheAHK
 [CheAHK][PI] + OCS -> CheAHK + [OCS][PI]
#### HkAutoPhosphorus
CheAHK + ATP -> [CheAHK][PI] + ADP Enzyme: [MCP][CH3][Inducer]
#### KEGG_Pathways
编译好的KEGG代谢网络模型
