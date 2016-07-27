---
title: Tree
---

# Tree
_namespace: [Microsoft.VisualBasic.DataMining.KMeans](N-Microsoft.VisualBasic.DataMining.KMeans.html)_





### Methods

#### __buildNET
```csharp
Microsoft.VisualBasic.DataMining.KMeans.Tree.__buildNET(Microsoft.VisualBasic.DataMining.KMeans.Tree.__edgePath[],Microsoft.VisualBasic.DataVisualization.Network.FileStream.Node,System.Int32,Microsoft.VisualBasic.List{Microsoft.VisualBasic.DataVisualization.Network.FileStream.Node}@)
```
从某一个分支点下来

|Parameter Name|Remarks|
|--------------|-------|
|array|-|
|depth|-|
|nodes|-|


#### __firstCluster``1
```csharp
Microsoft.VisualBasic.DataMining.KMeans.Tree.__firstCluster``1(System.Collections.Generic.IEnumerable{``0},System.Int32)
```
两条线程并行化进行二叉树聚类

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|[stop]|-|


#### bTreeNET
```csharp
Microsoft.VisualBasic.DataMining.KMeans.Tree.bTreeNET(System.Collections.Generic.IEnumerable{Microsoft.VisualBasic.DataMining.KMeans.EntityLDM})
```
Create network model for visualize the binary tree clustering result.

|Parameter Name|Remarks|
|--------------|-------|
|source|-|


#### Partitioning
```csharp
Microsoft.VisualBasic.DataMining.KMeans.Tree.Partitioning(System.Collections.Generic.IEnumerable{Microsoft.VisualBasic.DataMining.KMeans.EntityLDM},System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|cluster|-|
|depth|将会以最短的聚类作为数据分区的深度|


#### TreeCluster
```csharp
Microsoft.VisualBasic.DataMining.KMeans.Tree.TreeCluster(System.Collections.Generic.IEnumerable{Microsoft.VisualBasic.DataMining.KMeans.EntityLDM},System.Boolean)
```
树形聚类

|Parameter Name|Remarks|
|--------------|-------|
|resultSet|-|



