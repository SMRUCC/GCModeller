---
title: GeneInteractions
---

# GeneInteractions
_namespace: [SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET](N-SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET.html)_

绘制KEGG数据为主的基因互作网络

> 
>  1. 对于有代谢反应的网络，表现为酶分子与酶分子之间所组成的网络
>  


### Methods

#### __exportPathwayGraph
```csharp
SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET.GeneInteractions.__exportPathwayGraph(SMRUCC.genomics.Assembly.KEGG.Archives.Xml.XmlModel,System.Boolean)
```
这个方法所导出的网络模型可以用于PfsNEt的文件3

|Parameter Name|Remarks|
|--------------|-------|
|FromPathway|-|
|Trim|-|


#### CreateNetworkObject
```csharp
SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET.GeneInteractions.CreateNetworkObject(SMRUCC.genomics.Assembly.KEGG.Archives.Xml.XmlModel)
```
对外的接口

#### ExportPathwayGraph
```csharp
SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET.GeneInteractions.ExportPathwayGraph(SMRUCC.genomics.Assembly.KEGG.Archives.Xml.XmlModel,System.Boolean)
```
函数导出kegg——module的信息，组成pathway还需要进行拼接

|Parameter Name|Remarks|
|--------------|-------|
|FromPathway|-|
|Trim|-|


#### ExportPathwayGraphFile
```csharp
SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET.GeneInteractions.ExportPathwayGraphFile(SMRUCC.genomics.Assembly.KEGG.Archives.Xml.XmlModel,System.String,System.Boolean)
```
创建KEGG数据库所编译的cytoscape网络，并导出为文件

|Parameter Name|Remarks|
|--------------|-------|
|FromPathway|-|
|saveDIR|-|
|Trim|-|


#### PfsNETNetwork
```csharp
SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET.GeneInteractions.PfsNETNetwork(SMRUCC.genomics.Assembly.KEGG.Archives.Xml.XmlModel,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.TabularArchives.SubNETCsvObject},System.String)
```
从模型之中导出网络数据并按照模块编号分别保存到文件系统之中

|Parameter Name|Remarks|
|--------------|-------|
|FromPathway|-|
|PfsNet|-|
|ExportDIR|-|


#### PfsNETNetwork_assemble_keggpathways
```csharp
SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET.GeneInteractions.PfsNETNetwork_assemble_keggpathways(SMRUCC.genomics.Assembly.KEGG.Archives.Xml.XmlModel,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.TabularArchives.SubNETCsvObject},System.String)
```
按照代谢途径来可视化pfsnet的计算数据

|Parameter Name|Remarks|
|--------------|-------|
|FromPathway|-|
|PfsNet|-|
|ExportDIR|-|



