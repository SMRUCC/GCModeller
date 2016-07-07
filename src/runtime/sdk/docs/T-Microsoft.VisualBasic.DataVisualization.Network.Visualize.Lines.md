---
title: Lines
---

# Lines
_namespace: [Microsoft.VisualBasic.DataVisualization.Network.Visualize](N-Microsoft.VisualBasic.DataVisualization.Network.Visualize.html)_





### Methods

#### PointOnCurve
```csharp
Microsoft.VisualBasic.DataVisualization.Network.Visualize.Lines.PointOnCurve(System.Drawing.Point,System.Drawing.Point,System.Drawing.Point,System.Drawing.Point,System.Double)
```
Calculates interpolated point between two points using Catmull-Rom Spline///

|Parameter Name|Remarks|
|--------------|-------|
|p0|First Point|
|p1|Second Point|
|p2|Third Point|
|p3|Fourth Point|
|t|
 Normalised distance between second and third point /// where the spline point will be calculated/// |

_returns: Calculated Spline Point/// _
> 
>  Points calculated exist on the spline between points two and three./// 


