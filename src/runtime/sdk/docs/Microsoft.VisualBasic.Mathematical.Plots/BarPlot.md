# BarPlot
_namespace: [Microsoft.VisualBasic.Mathematical.Plots](./index.md)_

这个不像@``T:Microsoft.VisualBasic.Mathematical.Plots.Histogram``用于描述若干组连续的数据，这个是将数据按照标签分组来表述出来的



### Methods

#### __plot1
```csharp
Microsoft.VisualBasic.Mathematical.Plots.BarPlot.__plot1(System.Drawing.Graphics@,Microsoft.VisualBasic.Imaging.Drawing2D.GraphicsRegion,Microsoft.VisualBasic.Mathematical.Plots.BarDataGroup,System.String,System.Boolean,System.Boolean,System.Boolean,System.Boolean,System.Drawing.Point,Microsoft.VisualBasic.Imaging.Drawing2D.Vector.Shapes.Border)
```


|Parameter Name|Remarks|
|--------------|-------|
|g|gdi+|
|grect|-|
|data|-|
|bg$|-|
|showGrid|-|
|stacked|-|
|stackReorder|-|
|showLegend|-|
|legendPos|-|
|legendBorder|-|


#### FromData
```csharp
Microsoft.VisualBasic.Mathematical.Plots.BarPlot.FromData(System.Collections.Generic.IEnumerable{System.Double})
```
Creates sample groups from a data vector

|Parameter Name|Remarks|
|--------------|-------|
|data|-|


#### FromODE
```csharp
Microsoft.VisualBasic.Mathematical.Plots.BarPlot.FromODE(Microsoft.VisualBasic.Mathematical.Calculus.ODE[])
```
Plot ODEs result using bar plot

|Parameter Name|Remarks|
|--------------|-------|
|odes|-|


#### Plot
```csharp
Microsoft.VisualBasic.Mathematical.Plots.BarPlot.Plot(Microsoft.VisualBasic.Mathematical.Plots.BarDataGroup,System.Drawing.Size,System.Drawing.Size,System.String,System.Boolean,System.Boolean,System.Nullable{System.Boolean},System.Boolean,System.Drawing.Point,Microsoft.VisualBasic.Imaging.Drawing2D.Vector.Shapes.Border)
```
Bar data plot

|Parameter Name|Remarks|
|--------------|-------|
|data|Data groups|
|size|image output size|
|margin|margin blank of the plots region|
|bg$|Background color expression|
|showGrid|Show axis grid?|
|stacked|Bar plot is stacked of each sample?|
|stackReordered|reorder bar data? Only works in stacked mode|
|showLegend|Show data legend?|
|legendPos|Position of the data legend on the image|
|legendBorder|legend shape border style.|


#### Plot2
```csharp
Microsoft.VisualBasic.Mathematical.Plots.BarPlot.Plot2(Microsoft.VisualBasic.Mathematical.Plots.BarDataGroup,System.Drawing.Size,System.Drawing.Size,System.String,System.Boolean,System.Boolean,System.Boolean,System.Drawing.Point,Microsoft.VisualBasic.Imaging.Drawing2D.Vector.Shapes.Border)
```
Plot bar plot in different direction compare with @``M:Microsoft.VisualBasic.Mathematical.Plots.BarPlot.Plot(Microsoft.VisualBasic.Mathematical.Plots.BarDataGroup,System.Drawing.Size,System.Drawing.Size,System.String,System.Boolean,System.Boolean,System.Nullable{System.Boolean},System.Boolean,System.Drawing.Point,Microsoft.VisualBasic.Imaging.Drawing2D.Vector.Shapes.Border)``

|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|size|-|
|margin|-|
|bg$|-|
|showGrid|-|
|stacked|-|
|showLegend|-|
|legendPos|-|
|legendBorder|-|



