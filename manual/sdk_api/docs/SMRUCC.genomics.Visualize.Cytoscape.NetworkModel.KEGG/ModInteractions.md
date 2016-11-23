# ModInteractions
_namespace: [SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.KEGG](./index.md)_

基因和模块之间的从属关系的示意图



### Methods

#### __modProperty
```csharp
SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.KEGG.ModInteractions.__modProperty(Microsoft.VisualBasic.Data.visualize.Network.FileStream.Network,Microsoft.VisualBasic.Data.visualize.Network.FileStream.NetworkEdge[])
```


|Parameter Name|Remarks|
|--------------|-------|
|edges|Mod -> Gene|


#### AddFootprints
```csharp
SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.KEGG.ModInteractions.AddFootprints(Microsoft.VisualBasic.Data.visualize.Network.FileStream.Network,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat.RegulatesFootprints},System.Boolean)
```
向网络之中添加调控信息

|Parameter Name|Remarks|
|--------------|-------|
|net|-|
|footprints|基因调控信息|



### Properties

#### PathwayGene
Label for interation pathway genes
