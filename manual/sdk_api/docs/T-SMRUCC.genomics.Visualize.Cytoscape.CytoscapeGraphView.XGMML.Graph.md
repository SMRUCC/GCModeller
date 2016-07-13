---
title: Graph
---

# Graph
_namespace: [SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML](N-SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.html)_

The Cytoscape software XML format network visualization model.(Cytoscape软件的网络XML模型文件)

> 请注意，由于在Cytoscape之中，每一个Xml元素都是小写字母的，所以在这个类之中的所有的Xml序列化的标记都不可以再更改大小写了


### Methods

#### CreateObject
```csharp
SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.Graph.CreateObject(System.String,System.String,System.String)
```
Creates a default cytoscape network model xml file with specific title and description.(创建一个初始默认的网络文件)

#### GetNode
```csharp
SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.Graph.GetNode(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Label|Synonym|


#### Load
```csharp
SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.Graph.Load(System.String)
```
使用这个方法才能够正确的加载一个cytoscape的网络模型文件

|Parameter Name|Remarks|
|--------------|-------|
|path|-|


#### Save
```csharp
SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.Graph.Save(System.String,System.Text.Encoding)
```
Save this cytoscape network visualization model using this function.
 (请使用这个方法进行Cytoscape网络模型文件的保存)

|Parameter Name|Remarks|
|--------------|-------|
|FilePath|The file path of the xml file saved location.|
|encoding|The text encoding of saved text file.|



### Properties

#### Directed
The edges between these nodes have the direction from one node to another node?
 (这个网络模型文件之中的相互作用的节点之间的边是否是具有方向性的)
#### Label
The brief title information of this cytoscape network model.(这个Cytoscape网络模型文件的摘要标题信息)
#### NetworkMetaData
在这个属性里面会自动设置Graph对象的属性列表里面的数据
