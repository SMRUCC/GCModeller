---
title: SegmentObject
---

# SegmentObject
_namespace: [SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels](N-SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels.html)_

染色体上面的一个基因的绘图模型



### Methods

#### Draw
```csharp
SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels.SegmentObject.Draw(System.Drawing.Graphics,System.Drawing.Point,System.Double,System.Int32,SMRUCC.genomics.Visualize.ChromosomeMap.Conf)
```


|Parameter Name|Remarks|
|--------------|-------|
|Gr|-|
|Location|图形的左上角的坐标|

_returns: 返回绘制的图形的大小_

#### LeftAligned
```csharp
SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels.SegmentObject.LeftAligned(System.Int32,System.Int32,System.Int32,System.Int32,System.Drawing.Point)
```


|Parameter Name|Remarks|
|--------------|-------|
|segnmentLength|基因对象的图形的绘制长度|
|textLength|使用MeasureString获取得到的字符串的绘制长度|
|p|基因对象额绘制坐标|



### Properties

#### CommonName
基因名称
#### LocusTag
基因号
#### Product
基因功能注释文字
