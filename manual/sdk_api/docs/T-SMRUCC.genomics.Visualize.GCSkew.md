---
title: GCSkew
---

# GCSkew
_namespace: [SMRUCC.genomics.Visualize](N-SMRUCC.genomics.Visualize.html)_





### Methods

#### DrawAixs
```csharp
SMRUCC.genomics.Visualize.GCSkew.DrawAixs(Microsoft.VisualBasic.Imaging.IGraphics,System.Drawing.Point,System.Drawing.Size,System.Drawing.Font,System.Double,System.Double)
```
绘制基本的坐标轴

|Parameter Name|Remarks|
|--------------|-------|
|g|-|
|location|-|
|size|-|
|tagFont|-|


#### InvokeDrawing
```csharp
SMRUCC.genomics.Visualize.GCSkew.InvokeDrawing(System.Drawing.Image,SMRUCC.genomics.SequenceModel.FASTA.FastaToken,System.Drawing.Point,System.Int32)
```
将GC偏移曲线绘制到目标比对图形**source**之上

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|nt|-|
|Location|坐标轴原点的位置|
|Width|坐标轴纵轴的宽度|


#### InvokeDrawingGCContent
```csharp
SMRUCC.genomics.Visualize.GCSkew.InvokeDrawingGCContent(System.Drawing.Image,SMRUCC.genomics.SequenceModel.FASTA.FastaToken,System.Drawing.Point,System.Int32)
```
将GC含量曲线绘制到目标比对图形**source**之上

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|nt|Attributes的 1 和 2 分别为nt的开始和结束的位置|
|Location|坐标轴原点的位置|
|Width|坐标轴纵轴的宽度|



