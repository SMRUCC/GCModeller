---
title: Renderer
---

# Renderer
_namespace: [Microsoft.VisualBasic.DataVisualization.Network.Canvas](N-Microsoft.VisualBasic.DataVisualization.Network.Canvas.html)_





### Methods

#### #ctor
```csharp
Microsoft.VisualBasic.DataVisualization.Network.Canvas.Renderer.#ctor(System.Func{System.Drawing.Graphics},System.Func{System.Drawing.Rectangle},Microsoft.VisualBasic.DataVisualization.Network.Layouts.Interfaces.IForceDirected)
```
这个构造函数会生成一些静态数据的缓存

|Parameter Name|Remarks|
|--------------|-------|
|canvas|-|
|regionProvider|-|
|iForceDirected|-|


#### GraphToScreen
```csharp
Microsoft.VisualBasic.DataVisualization.Network.Canvas.Renderer.GraphToScreen(Microsoft.VisualBasic.DataVisualization.Network.Layouts.FDGVector2,System.Drawing.Rectangle)
```
Projects the data model to our screen for display.

|Parameter Name|Remarks|
|--------------|-------|
|iPos|-|


#### ScreenToGraph
```csharp
Microsoft.VisualBasic.DataVisualization.Network.Canvas.Renderer.ScreenToGraph(System.Drawing.Point)
```
Projects the client graphics data to the data model.

|Parameter Name|Remarks|
|--------------|-------|
|iScreenPos|-|



### Properties

#### __graphicsProvider
Gets the graphics source
#### __regionProvider
gets the graphics region for the projections: @"M:Microsoft.VisualBasic.DataVisualization.Network.Canvas.Renderer.GraphToScreen(Microsoft.VisualBasic.DataVisualization.Network.Layouts.FDGVector2,System.Drawing.Rectangle)" and @"M:Microsoft.VisualBasic.DataVisualization.Network.Canvas.Renderer.ScreenToGraph(System.Drawing.Point)"
#### radiushash
The node drawing radius cache
#### widthHash
The edge drawing width cache
