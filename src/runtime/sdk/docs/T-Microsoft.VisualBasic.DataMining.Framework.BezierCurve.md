---
title: BezierCurve
---

# BezierCurve
_namespace: [Microsoft.VisualBasic.DataMining.Framework](N-Microsoft.VisualBasic.DataMining.Framework.html)_





### Methods

#### __interpolation
```csharp
Microsoft.VisualBasic.DataMining.Framework.BezierCurve.__interpolation(System.Double[],System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|Elements|-|
|iteration|-|


#### BezierSmoothInterpolation
```csharp
Microsoft.VisualBasic.DataMining.Framework.BezierCurve.BezierSmoothInterpolation(System.Double[],System.Int32,System.Int32,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|Parallel|²¢ÐÐ°æ±¾µÄ|
|WindowSize|Êý¾Ý²ÉÑùµÄ´°¿Ú´óÐ¡£¬Ä¬ÈÏ´óÐ¡ÊÇ**data**µÄ°Ù·ÖÖ®1|

> ÏÈ¶ÔÊý¾Ý½øÐÐ²ÉÑù£¬È»ºó²åÖµ£¬×îºó·µ»Ø²åÖµºóµÄÆ½»¬ÇúÏßÊý¾ÝÒÔÓÃÓÚÏÂÒ»²½·ÖÎö

#### CreateBezier
```csharp
Microsoft.VisualBasic.DataMining.Framework.BezierCurve.CreateBezier(System.Drawing.PointF,System.Drawing.PointF,System.Drawing.PointF)
```
create a bezier curve

|Parameter Name|Remarks|
|--------------|-------|
|ctrl1|first initial point|
|ctrl2|second initial point|
|ctrl3|third initial point|


#### MidPoint
```csharp
Microsoft.VisualBasic.DataMining.Framework.BezierCurve.MidPoint(System.Drawing.PointF,System.Drawing.PointF)
```
Find mid point

|Parameter Name|Remarks|
|--------------|-------|
|controlPoint1|first control point|
|controlPoint2|second control point|


#### PopulateBezierPoints
```csharp
Microsoft.VisualBasic.DataMining.Framework.BezierCurve.PopulateBezierPoints(System.Drawing.PointF,System.Drawing.PointF,System.Drawing.PointF,System.Int32)
```
Recursivly call to construct the bezier curve with control points

|Parameter Name|Remarks|
|--------------|-------|
|ctrl1|first control point of bezier curve segment|
|ctrl2|second control point of bezier curve segment|
|ctrl3|third control point of bezier curve segment|
|currentIteration|the current interation of a branch|


#### ReCalculate
```csharp
Microsoft.VisualBasic.DataMining.Framework.BezierCurve.ReCalculate(System.Drawing.PointF,System.Drawing.PointF,System.Drawing.PointF,System.Int32)
```
recreate the bezier curve.

|Parameter Name|Remarks|
|--------------|-------|
|ctrl1|first initial point|
|ctrl2|second initial point|
|ctrl3|third initial point|
|iteration|number of iteration of the algorithm|

_returns: the list of points in the curve_


### Properties

#### BezierPointList
store the list of points in the bezier curve
#### InitPointsList
store the list of initial points
#### Iterations
store the number of iterations
