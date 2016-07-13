---
title: MatrixSerialization
---

# MatrixSerialization
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq](N-SMRUCC.genomics.Analysis.RNA_Seq.html)_

对PCC矩阵进行快速的二进制序列化

> 由于是一个二维的矩阵，坐标之间有着一一对应的顺序关系，所以这里不可以使用并行化拓展


### Methods

#### Load
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.MatrixSerialization.Load(System.String)
```
加载二进制数据库

|Parameter Name|Remarks|
|--------------|-------|
|from|-|


#### Serialize
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.MatrixSerialization.Serialize(SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.ExprSamples)
```
<TOTAL_BYTES> + <STRING_LENGTH>

|Parameter Name|Remarks|
|--------------|-------|
|sample|-|



