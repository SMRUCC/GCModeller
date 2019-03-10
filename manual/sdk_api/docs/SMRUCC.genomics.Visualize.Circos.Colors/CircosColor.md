# CircosColor
_namespace: [SMRUCC.genomics.Visualize.Circos.Colors](./index.md)_

Crcos程序的所支持的颜色



### Methods

#### __loadResource
```csharp
SMRUCC.genomics.Visualize.Circos.Colors.CircosColor.__loadResource
```
从资源文件之中加载可以被使用的CIRCOS颜色映射数据，这个函数会在模块的构造函数之中自动调用

#### ColorProfiles
```csharp
SMRUCC.genomics.Visualize.Circos.Colors.CircosColor.ColorProfiles(System.Collections.Generic.IEnumerable{System.String})
```
Mappings each item in the categories into the Circos color name to generates a color profiles for drawing the elements in the circos plot.

|Parameter Name|Remarks|
|--------------|-------|
|categories|-|


#### ColorProfiles``1
```csharp
SMRUCC.genomics.Visualize.Circos.Colors.CircosColor.ColorProfiles``1(``0[])
```
Mappings each item in the categories into the Circos color name to generates a color profiles for drawing the elements in the circos plot.
 (生成颜色谱, {**`categories`**, Circos @``T:System.Drawing.Color`` Code})

|Parameter Name|Remarks|
|--------------|-------|
|categories|-|


#### FromColor
```csharp
SMRUCC.genomics.Visualize.Circos.Colors.CircosColor.FromColor(System.Drawing.Color)
```
将VB.NET的颜色映射为Perl之中的颜色

|Parameter Name|Remarks|
|--------------|-------|
|Color|-|


#### FromHsv
```csharp
SMRUCC.genomics.Visualize.Circos.Colors.CircosColor.FromHsv(System.Double,System.Double,System.Double)
```
将hsv颜色转换为Circos里面的颜色名称

|Parameter Name|Remarks|
|--------------|-------|
|H|-|
|S|-|
|V|-|


#### FromKnownColorName
```csharp
SMRUCC.genomics.Visualize.Circos.Colors.CircosColor.FromKnownColorName(System.String)
```
Gets the .NET color object from the circos color name. If the function failed, then the Color.Black value will be return.

|Parameter Name|Remarks|
|--------------|-------|
|Name|-|


#### FromRGB
```csharp
SMRUCC.genomics.Visualize.Circos.Colors.CircosColor.FromRGB(System.Int32,System.Int32,System.Int32)
```
Gets circos color name from the .NET color object R,G,B value.

|Parameter Name|Remarks|
|--------------|-------|
|R|-|
|G|-|
|B|-|



### Properties

#### ColorNames
在这里是需要根据RGB的数值将其映射为文本
