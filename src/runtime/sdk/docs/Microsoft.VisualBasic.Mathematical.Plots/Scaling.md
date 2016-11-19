# Scaling
_namespace: [Microsoft.VisualBasic.Mathematical.Plots](./index.md)_

将数据坐标转换为绘图坐标



### Methods

#### ForEach
```csharp
Microsoft.VisualBasic.Mathematical.Plots.Scaling.ForEach(System.Drawing.Size,System.Drawing.Size)
```
返回的系列是已经被转换过的，直接使用来进行画图

#### ForEach_histSample
```csharp
Microsoft.VisualBasic.Mathematical.Plots.Scaling.ForEach_histSample(System.Drawing.Size,System.Drawing.Size)
```
返回的系列是已经被转换过的，直接使用来进行画图

#### Scaling
```csharp
Microsoft.VisualBasic.Mathematical.Plots.Scaling.Scaling(Microsoft.VisualBasic.Mathematical.Plots.Histogram.HistogramGroup,System.Func{Microsoft.VisualBasic.Mathematical.Plots.Histogram.HistogramData,System.Single[]},System.Single@)
```
返回dx或者dy

#### YScaler
```csharp
Microsoft.VisualBasic.Mathematical.Plots.Scaling.YScaler(System.Drawing.Size,System.Drawing.Size,System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|size|-|
|margin|-|
|avg|当这个参数值是一个有效的数字的时候，返回的Y将会以这个平均值为零点|



