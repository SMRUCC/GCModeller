---
title: Output
---

# Output
_namespace: [SMRUCC.genomics.Analysis.SequenceTools](N-SMRUCC.genomics.Analysis.SequenceTools.html)_





### Methods

#### CreateObject``1
```csharp
SMRUCC.genomics.Analysis.SequenceTools.Output.CreateObject``1(SMRUCC.genomics.Analysis.SequenceTools.GSW{``0},Microsoft.VisualBasic.LevenshteinDistance.ToChar{``0},System.Double,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|sw|-|
|toChar|-|
|threshold|0% - 100%|



### Properties

#### Best
best chain, 但是不明白这个有什么用途
#### Directions
The directions pointing to the cells that
 give the maximum score at the current cell.
 The first index is the column index.
 The second index is the row index.
#### DP
Dynmaic programming matrix.(也可以看作为得分矩阵)
#### HSP
最佳的比对结果
