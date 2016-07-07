---
title: MatrixAPI
---

# MatrixAPI
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq](N-SMRUCC.genomics.Analysis.RNA_Seq.html)_





### Methods

#### CreateCorrelationMatrix
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.MatrixAPI.CreateCorrelationMatrix(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,System.Double,System.Double,System.Double,System.Double,System.Boolean)
```
计算PCC和SPCC的混合矩阵，这个函数会首先计算PCC，当PCC的值过低的时候，会计算SPCC的值来替代

|Parameter Name|Remarks|
|--------------|-------|
|ChipData|-|
|pcc_th1|-|
|pcc_th2|-|
|spcc_th1|-|
|spcc_th2|-|
|FirstLineTitle|-|


#### CreatePccMAT
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.MatrixAPI.CreatePccMAT(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,System.Boolean)
```
对每一个基因对之间计算皮尔森相关系数，并返回得到的矩阵

|Parameter Name|Remarks|
|--------------|-------|
|rawExpr|-|
|FirstLineTitle|-|


#### CreateSPccMAT
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.MatrixAPI.CreateSPccMAT(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,System.Boolean)
```
计算得到斯皮尔曼相关性矩阵

|Parameter Name|Remarks|
|--------------|-------|
|ChipData|-|
|FirstLineTitle|-|


#### J
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.MatrixAPI.J(SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.ExprSamples,SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.ExprSamples,System.Double)
```
Paiwise sample redundancy

|Parameter Name|Remarks|
|--------------|-------|
|sample1|-|
|sample2|-|
|C|cut-off threshold, We used 0.4 for this threshold, which is roughly optimized|


#### Sampling
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.MatrixAPI.Sampling(System.String,System.Int32,System.Boolean)
```
从虚拟细胞计算出来的转录组数据之中进行采样

|Parameter Name|Remarks|
|--------------|-------|
|datas|数据文件夹，里面应该包含有不同的实验条件之下所得到的转录组计算数据|
|TimeId|采样的时间编号|
|FirstLineTitle|第一行是否为标题行|


#### SavePccMatrix
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.MatrixAPI.SavePccMatrix(SMRUCC.genomics.Analysis.RNA_Seq.PccMatrix,System.String)
```
(ShellScript API) @"M:SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.ExprSamples.CreateFile(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.ExprSamples})"

|Parameter Name|Remarks|
|--------------|-------|
|Pccmatrix|-|
|saveto|-|


#### Similarity
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.MatrixAPI.Similarity(SMRUCC.genomics.Analysis.RNA_Seq.PccMatrix,SMRUCC.genomics.Analysis.RNA_Seq.PccMatrix)
```
比较芯片数据和计算数据的相似性

|Parameter Name|Remarks|
|--------------|-------|
|BenchmarkQuery|由实验所获取得到的基因芯片基准数据|
|VcellValidate|所计算出来的需要进行验证的计算数据|


#### ToSamples
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.MatrixAPI.ToSamples(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,System.Boolean)
```
将目标芯片数据转换为每一个基因的计算样本，在本方法之中没有涉及到将目标数据集计算为相关系数矩阵的操作

|Parameter Name|Remarks|
|--------------|-------|
|Data|-|
|FirstRowTitle|Is first row in the table is the title description row, not a valid data row?(数据集中的第一行是否为标题行)|



