---
title: DataFrameRow
---

# DataFrameRow
_namespace: [SMRUCC.genomics.Analysis.PFSNet.DataStructure](N-SMRUCC.genomics.Analysis.PFSNet.DataStructure.html)_

The gene expression data samples file.(基因的表达数据样本)



### Methods

#### CreateApplyFunctionCache
```csharp
SMRUCC.genomics.Analysis.PFSNet.DataStructure.DataFrameRow.CreateApplyFunctionCache(SMRUCC.genomics.Analysis.PFSNet.DataStructure.DataFrameRow[])
```
以列为单位

|Parameter Name|Remarks|
|--------------|-------|
|data|-|


#### LoadData
```csharp
SMRUCC.genomics.Analysis.PFSNet.DataStructure.DataFrameRow.LoadData(System.String)
```
Load the PfsNET file1 and file2 data into the memory.(加载PfsNET计算数据之中的文件1和文件2至计算机内存之中)

|Parameter Name|Remarks|
|--------------|-------|
|path|-|



### Properties

#### ExperimentValues
This gene's expression value in the different experiment condition.(同一个基因在不同实验之下的表达值)
#### Samples
Gets the sample counts of current gene expression data.(获取基因表达数据样本数目)
