---
title: MapModelCommon
---

# MapModelCommon
_namespace: [SMRUCC.genomics.Visualize](N-SMRUCC.genomics.Visualize.html)_

绘图模型的通用基本类型结构



### Methods

#### CreateBackwardModel
```csharp
SMRUCC.genomics.Visualize.MapModelCommon.CreateBackwardModel(System.Drawing.Point,System.Int32)
```
假若所绘制出来的模型的右部分的坐标超过了**RightLimit**这个参数，则会被缩短

|Parameter Name|Remarks|
|--------------|-------|
|refLoci|-|
|RightLimit|-|


#### CreateForwardModel
```csharp
SMRUCC.genomics.Visualize.MapModelCommon.CreateForwardModel(System.Drawing.Point,System.Int32)
```
假若所绘制出来的模型的右部分的坐标超过了**RightLimit**这个参数，则会被缩短

|Parameter Name|Remarks|
|--------------|-------|
|refLoci|-|
|RightLimit|-|


#### CreateNoneDirectionModel
```csharp
SMRUCC.genomics.Visualize.MapModelCommon.CreateNoneDirectionModel(System.Drawing.Point,System.Int32)
```
假若所绘制出来的模型的右部分的坐标超过了**RightLimit**这个参数，则会被缩短

|Parameter Name|Remarks|
|--------------|-------|
|refLoci|-|
|RightLimit|-|



### Properties

#### Direction
0表示没有方向，1表示正向，-1表示反向；默认为正向
#### HeadLength
所绘制的基因对象的箭头的长度，其单位与@"P:SMRUCC.genomics.Visualize.MapModelCommon.Length"属性一致，都是像素，即这个属性值是已经换算过的
#### HeadLengthLimits
箭头长度的最大限制
#### HeadLengthLowerBound
箭头长度的最低限制
#### Height
基因片段对象在绘制的时候的高度
#### Left
片段最左端的位置,这个位置和方向无关
#### Length
当前的基因对想在图形上面的绘制长度
#### Right
片段最右端的位置，这个位置和方向无关
