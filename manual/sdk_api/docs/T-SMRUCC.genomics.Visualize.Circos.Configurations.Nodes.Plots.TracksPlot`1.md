---
title: TracksPlot`1
---

# TracksPlot`1
_namespace: [SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots](N-SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots.html)_





### Methods

#### #ctor
```csharp
SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots.TracksPlot`1.#ctor(SMRUCC.genomics.Visualize.Circos.TrackDatas.data{`0})
```
Creates plot element from the tracks data file.

|Parameter Name|Remarks|
|--------------|-------|
|data|-|


#### GetProperties
```csharp
SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots.TracksPlot`1.GetProperties
```
SimpleConfig.GenerateConfigurations(Of <PlotType>)(Me)，需要手工复写以得到正确的类型


### Properties

#### file
输入的路径会根据配置情况转换为相对路径或者绝对路径
#### orientation
圈的朝向，是@"F:SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots.TracksPlot`1.ORIENTATION_IN"向内还是@"F:SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots.TracksPlot`1.ORIENTATION_OUT"向外
#### r0
圈内径(单位 r，请使用格式"<double>r")
#### r1
圈外径(单位 r，请使用格式"<double>r")
#### thickness
To turn off default outline, set the outline thickness to zero. 
 If you want To permanently disable this Default, edit
 ``etc/tracks/histogram.conf`` In the Circos distribution.
#### TracksData
data文件夹之中的绘图数据
