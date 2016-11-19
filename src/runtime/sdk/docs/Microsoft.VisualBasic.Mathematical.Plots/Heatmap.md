# Heatmap
_namespace: [Microsoft.VisualBasic.Mathematical.Plots](./index.md)_





### Methods

#### DrawString
```csharp
Microsoft.VisualBasic.Mathematical.Plots.Heatmap.DrawString(System.Drawing.Graphics,System.String,System.Drawing.Font,System.Drawing.Brush,System.Single,System.Single,System.Single)
```
绘制按照任意角度旋转的文本

|Parameter Name|Remarks|
|--------------|-------|
|g|-|
|text|-|
|font|-|
|brush|-|
|x!|-|
|y!|-|
|angle!|-|


#### LoadDataSet
```csharp
Microsoft.VisualBasic.Mathematical.Plots.Heatmap.LoadDataSet(System.String,System.String)
```
假若使用这个直接加载数据来进行heatmap的绘制，请先要确保数据集之中的所有数据都是经过归一化的

|Parameter Name|Remarks|
|--------------|-------|
|path|-|
|uidMap$|-|


#### Plot
```csharp
Microsoft.VisualBasic.Mathematical.Plots.Heatmap.Plot(System.Collections.Generic.IEnumerable{Microsoft.VisualBasic.ComponentModel.DataSourceModel.NamedValue{System.Collections.Generic.Dictionary{System.String,System.Double}}},System.Drawing.Color[],System.Int32,System.String,System.Boolean,System.Drawing.Size,System.Drawing.Size,System.String,System.String,System.String,System.Single)
```


|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|colors|-|
|mapLevels%|-|
|mapName$|-|
|kmeans|Reorder datasets by using kmeans clustering|
|size|-|
|margin|-|
|bg$|-|



