---
title: KMeansCluster`1
---

# KMeansCluster`1
_namespace: [Microsoft.VisualBasic.DataMining.Framework.KMeans](N-Microsoft.VisualBasic.DataMining.Framework.KMeans.html)_

A class containing a group of data with similar characteristics (cluster), KMeans Cluster



### Methods

#### Add
```csharp
Microsoft.VisualBasic.DataMining.Framework.KMeans.KMeansCluster`1.Add(`0)
```
Adds a single dimension array data to the cluster

|Parameter Name|Remarks|
|--------------|-------|
|data|A 1-dimensional array containing data that will be added to the cluster|


#### refresh
```csharp
Microsoft.VisualBasic.DataMining.Framework.KMeans.KMeansCluster`1.refresh
```
Will keep the center member variable, but clear the list of points
 within the cluster.


### Properties

#### ClusterMean
The mean of all the data in the cluster
#### ClusterSum
The sum of all the data in the cluster
#### Item
Returns the one dimensional array data located at the index
