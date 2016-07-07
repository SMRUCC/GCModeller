---
title: SegmentObject
---

# SegmentObject
_namespace: [SMRUCC.genomics.Visualize.PlasmidMap.DrawingModels](N-SMRUCC.genomics.Visualize.PlasmidMap.DrawingModels.html)_





### Methods

#### CreateBackwardModel
```csharp
SMRUCC.genomics.Visualize.PlasmidMap.DrawingModels.SegmentObject.CreateBackwardModel(System.Drawing.Point,System.Int32,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|ReferenceLocation|-|
|r1|外圈|
|r2|内圆|


#### Draw
```csharp
SMRUCC.genomics.Visualize.PlasmidMap.DrawingModels.SegmentObject.Draw(System.Drawing.Graphics,System.Drawing.Point,System.Int32,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|Gr|-|
|CenterLocation|图形的左上角的坐标|

_returns: 返回绘制的图形的大小_

#### DrawingStringMethod
```csharp
SMRUCC.genomics.Visualize.PlasmidMap.DrawingModels.SegmentObject.DrawingStringMethod(System.Drawing.Graphics,System.Drawing.Point)
```
绘制基因编号与基因功能注释

|Parameter Name|Remarks|
|--------------|-------|
|Gr|-|
|refLoc|-|


#### Internal_createArc
```csharp
SMRUCC.genomics.Visualize.PlasmidMap.DrawingModels.SegmentObject.Internal_createArc(System.Int32,System.Drawing.Point,System.Int32,System.Int32,System.Int32,System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|r|-|
|refPoint|-|
|startArc|-|
|endsArc|-|
|d|步进角度|


#### Internal_createArcBase
```csharp
SMRUCC.genomics.Visualize.PlasmidMap.DrawingModels.SegmentObject.Internal_createArcBase(System.Int32,System.Drawing.Point)
```


|Parameter Name|Remarks|
|--------------|-------|
|r|圆的半径|
|refPoint|正方形的中心的坐标|


#### Internal_getCircleRelativeLocation
```csharp
SMRUCC.genomics.Visualize.PlasmidMap.DrawingModels.SegmentObject.Internal_getCircleRelativeLocation(System.Int32,System.Int32,System.Int32,System.Drawing.Point)
```


|Parameter Name|Remarks|
|--------------|-------|
|p|序列片段的在基因组序列之上的位点|
|TotalLength|整个基因组的序列总长度|
|r|弧所处的圆的半径|



### Properties

#### Direction
0表示没有方向，1表示正向，-1表示反向
