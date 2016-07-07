---
title: ExprSamples
---

# ExprSamples
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT](N-SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.html)_

每一个基因的表达量的实验样本，{GeneId, value1, value2, value3, ...}



### Methods

#### ConvertObject
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.ExprSamples.ConvertObject(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject)
```
Convert the data line in the csv file into a object model in the pcc matrix(将Csv文件之中的数据行转换为对象模型)

|Parameter Name|Remarks|
|--------------|-------|
|rowData|-|


#### CreateFile
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.ExprSamples.CreateFile(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.ExprSamples})
```
Convert the Pcc matrix object into a csv document for save the data into filesystem.(将Pcc矩阵转换为Csv数据文件以进行保存)

|Parameter Name|Remarks|
|--------------|-------|
|DataSet|-|



