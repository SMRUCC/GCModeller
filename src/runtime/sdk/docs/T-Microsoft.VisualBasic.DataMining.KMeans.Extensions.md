---
title: Extensions
---

# Extensions
_namespace: [Microsoft.VisualBasic.DataMining.KMeans](N-Microsoft.VisualBasic.DataMining.KMeans.html)_





### Methods

#### Kmeans
```csharp
Microsoft.VisualBasic.DataMining.KMeans.Extensions.Kmeans(System.Collections.Generic.IEnumerable{Microsoft.VisualBasic.DataMining.KMeans.EntityLDM},System.Int32)
```
Performance the clustering operation on the entity data model.

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|n|-|


#### ValueGroups
```csharp
Microsoft.VisualBasic.DataMining.KMeans.Extensions.ValueGroups(System.Collections.Generic.IEnumerable{System.Double},System.Int32)
```
Grouping the numeric values by using the kmeans cluserting operations.
 (对一组数字进行聚类操作，其实在这里就是将这组数值生成Entity数据对象，然后将数值本身作为自动生成的Entity对象的一个唯一属性)

|Parameter Name|Remarks|
|--------------|-------|
|array|-|
|nd|-|



