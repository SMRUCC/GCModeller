---
title: GeneObject
---

# GeneObject
_namespace: [SMRUCC.genomics.Visualize.ComparativeGenomics](N-SMRUCC.genomics.Visualize.ComparativeGenomics.html)_





### Methods

#### __nextLeft
```csharp
SMRUCC.genomics.Visualize.ComparativeGenomics.GeneObject.__nextLeft(System.Int32,System.Drawing.Point,System.Int32,System.Double)
```
对于两个没有交叉的基因，不做任何附加处理。对于两个有相交部分的基因，则前一个基因会缩短以防止重叠，假若某一个基因完全的包裹另外一个基因，则也将不会做任何处理

|Parameter Name|Remarks|
|--------------|-------|
|NextLeft|这个是基因组上面的位置，不是画图的位置|
|RefPoint|参数里面的@"P:System.Drawing.Point.X"参数就是当前的这个基因在绘图的时候的@"P:SMRUCC.genomics.Visualize.MapModelCommon.Left"在图上面的位置|


#### CreateBackwardModel
```csharp
SMRUCC.genomics.Visualize.ComparativeGenomics.GeneObject.CreateBackwardModel(System.Drawing.Point,System.Int32)
```
假若所绘制出来的模型的右部分的坐标超过了**RightLimit**这个参数，则会被缩短

|Parameter Name|Remarks|
|--------------|-------|
|refLoci|-|
|RightLimit|-|


#### CreateForwardModel
```csharp
SMRUCC.genomics.Visualize.ComparativeGenomics.GeneObject.CreateForwardModel(System.Drawing.Point,System.Int32)
```
假若所绘制出来的模型的右部分的坐标超过了**RightLimit**这个参数，则会被缩短

|Parameter Name|Remarks|
|--------------|-------|
|refLoci|-|
|RightLimit|-|


#### InvokeDrawing
```csharp
SMRUCC.genomics.Visualize.ComparativeGenomics.GeneObject.InvokeDrawing(System.Drawing.Graphics,System.Drawing.Point,System.Int32,System.Double,System.Drawing.Rectangle@,System.Boolean,System.Drawing.Font,System.Boolean,System.Drawing.Rectangle@)
```
对于两个没有交叉的基因，不做任何附加处理。对于两个有相交部分的基因，则前一个基因会缩短以防止重叠，假若某一个基因完全的包裹另外一个基因，则也将不会做任何处理

|Parameter Name|Remarks|
|--------------|-------|
|Gr|-|
|RefPoint|-|
|IdGrawingPositionDown|基因标号是否绘制与基因图形的下方|
|Region|当前的基因对象所绘制的区域从这个参数进行返回|

_returns: 函数返回下一个基因对象的左端的坐标的@"P:System.Drawing.Point.X"_


### Properties

#### geneName
基因名
#### locus_tag
基因号
#### offsets
编号标签与ORF图形之间在水平位置上面的位移
