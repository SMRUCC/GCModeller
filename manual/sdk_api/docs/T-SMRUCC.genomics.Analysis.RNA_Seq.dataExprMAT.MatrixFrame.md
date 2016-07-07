---
title: MatrixFrame
---

# MatrixFrame
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT](N-SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.html)_

用来表述一个RNA-Seq结果数据的集合



### Methods

#### GetCurrentRPKMsVector
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.MatrixFrame.GetCurrentRPKMsVector
```
获取当前实验标号设置之下的所有的实验数据
 在使用本函数获取返回值之前，请先试用@"M:SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.MatrixFrame.SetColumn(System.Int32)"或者@"M:SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.MatrixFrame.SetColumnAuto(System.String)"设置实验编号

#### GetValue
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.MatrixFrame.GetValue(System.String,System.Boolean)
```
当目标基因编号不存在的时候返回0

|Parameter Name|Remarks|
|--------------|-------|
|locusTag|-|
|DEBUGInfo|当发生错误的时候是否显示出调试信息|


#### Load
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.MatrixFrame.Load(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
```
Load expression data from a csv docuemnt stream.

|Parameter Name|Remarks|
|--------------|-------|
|chipDataCsv|首行是标题行，第一列是基因的编号|


#### Log2
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.MatrixFrame.Log2(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.Experiment})
```


|Parameter Name|Remarks|
|--------------|-------|
|samples|{分子, 分母}()|

> 请勿再随意修改本函数之中的并行定义，以免照成混乱

#### SetColumn
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.MatrixFrame.SetColumn(System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|ColIndex|大于零的编号|


#### ToDictionary
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.MatrixFrame.ToDictionary
```
{GeneID, Expressions}


### Properties

#### __reader
原始数据
