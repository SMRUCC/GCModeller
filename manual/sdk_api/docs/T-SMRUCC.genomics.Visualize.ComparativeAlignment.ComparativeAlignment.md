---
title: ComparativeAlignment
---

# ComparativeAlignment
_namespace: [SMRUCC.genomics.Visualize.ComparativeAlignment](N-SMRUCC.genomics.Visualize.ComparativeAlignment.html)_





### Methods

#### BuildModel
```csharp
SMRUCC.genomics.Visualize.ComparativeAlignment.ComparativeAlignment.BuildModel(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame,System.Collections.Generic.IEnumerable{System.String},System.Collections.Generic.IEnumerable{System.String},System.String,System.Boolean,System.Boolean)
```
数据框之中的每一行数据都表示同源基因，列表示为基因组

|Parameter Name|Remarks|
|--------------|-------|
|DF|生成GeneLink数据|
|ColumnList|假若本参数值为空，则默认取出所有的数据|
|PttSource|请注意，这个值的顺序是与数据框之中的列是一一对应的|


#### BuildMultipleAlignmentModel
```csharp
SMRUCC.genomics.Visualize.ComparativeAlignment.ComparativeAlignment.BuildMultipleAlignmentModel(System.String,System.String,System.String,System.String,SMRUCC.genomics.SequenceModel.FASTA.FastaToken,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo})
```


|Parameter Name|Remarks|
|--------------|-------|
|source|bbh结果的文件夹|
|query|query的编号|
|PTT|存放多个物种的ptt文件的文件夹|


#### InvokeDrawing
```csharp
SMRUCC.genomics.Visualize.ComparativeAlignment.ComparativeAlignment.InvokeDrawing(SMRUCC.genomics.Visualize.ComparativeAlignment.DrawingModel,System.Drawing.Color,System.Boolean,System.Boolean,System.String)
```
绘制对比对图

|Parameter Name|Remarks|
|--------------|-------|
|model|-|
|defaultColor|-|
|type2Arrow|-|


#### TCSVisualization
```csharp
SMRUCC.genomics.Visualize.ComparativeAlignment.ComparativeAlignment.TCSVisualization(SMRUCC.genomics.Visualize.ComparativeAlignment.DrawingModel,System.Drawing.Color,System.Drawing.Color,System.Drawing.Color)
```
对双组分系统进行颜色赋值，其他的基因不变

|Parameter Name|Remarks|
|--------------|-------|
|model|-|



