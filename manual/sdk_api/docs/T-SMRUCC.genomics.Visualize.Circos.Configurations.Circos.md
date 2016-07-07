---
title: Circos
---

# Circos
_namespace: [SMRUCC.genomics.Visualize.Circos.Configurations](N-SMRUCC.genomics.Visualize.Circos.Configurations.html)_

``circos.conf``
 
 ```
 ____ _
 / ___(_)_ __ ___ ___ ___
 | | | | '__/ __/ _ \/ __|
 | |___| | | | (_| (_) \__ \
 \____|_|_| \___\___/|___/

 round Is good

 circos - generate circularly composited information graphics
 ```
 
 (Circo基因组绘图程序的主配置文件)

> 
>  ![](https://raw.githubusercontent.com/SMRUCC/GCModeller.Circos/master/manual/workflow.png)
>  
>  Typically a central configuration file which defines data track information (circos.conf) imports other 
>  configuration files that store parameters that change less frequently 
>  (tick marks, ideogram size, grid, etc). 
>  
>  Data for each data track Is stored in a file And the same file can be used for multiple tracks.
>  
>  + PNG image output Is ideal For immediate viewing, web-based reporting Or presentation. 
>  + SVG output Is most suitable For generating very high resolution line art For publication And For customizing aspects Of the figure.
>  


### Methods

#### AddPlotElement
```csharp
SMRUCC.genomics.Visualize.Circos.Configurations.Circos.AddPlotElement(SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots.ITrackPlot)
```
函数会根据元素的个数的情况自动的调整在圈内的位置

|Parameter Name|Remarks|
|--------------|-------|
|plotElement|-|


#### ForceAutoLayout
```csharp
SMRUCC.genomics.Visualize.Circos.Configurations.Circos.ForceAutoLayout(SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots.ITrackPlot[])
```
强制所指定的绘图元素自动布局

|Parameter Name|Remarks|
|--------------|-------|
|elements|-|



### Properties

#### chromosomes_color
The color of each ideogram is taken from the karyotype file. To
 change it, use 'chromosomes_color'.
#### chromosomes_display_default
The default behaviour is to display all chromosomes defined in the
 karyotype file. In this example, I Select only a subset.

 The 'chromosomes' parameter has several uses, and selecting which
 chromosomes To show Is one Of them. You can list them

 ```
 hs1;hs2;hs3;hs4
 ```
 
 Or provide a regular expression that selects them based On a successful match
 
 ```
 /hs[1-4]$/
 ```
 
 The ``$`` anchor Is necessary, otherwise chromosomes Like *hs10, hs11 And
 hs20* are also matched.
#### chromosomes_radius
The default radial position for all ideograms is set by 'radius' in
 the ``<ideogram>`` block (see ideogram.conf). To change the value For
 specific ideograms, use chromosomes_radius.
#### chromosomes_reverse
By default, the scale progression is clockwise. You can set the
 Global angle progression Using 'angle_orientation' in the ``<image>``
 block (clockwise Or counterclockwise). To reverse it For one Or
 several ideograms, use 'chromosomes-reverse'
#### chromosomes_scale
The size of the ideogram on the figure can be adjusted using an
 absolute Or relative magnification. Absolute scaling,

 ```
 hs1=0.5
 ```
 
 shrinks Or expands the ideogram by a fixed factor. When the "r"
 suffix Is used, the magnification becomes relative To the
 circumference Of the figure. Thus, 

 ```
 hs1=0.5r
 ```
 
 makes ``hs1`` To occupy 50% Of the figure. To uniformly distribute
 several ideogram within a fraction Of the figure, use a regular
 expression that selects the ideograms And the "rn" suffix (relative
 normalized).

 ```
 /hs[234]/=0.5Rn
 ```
 
 Will match ``hs2, hs3, hs4`` And divide them evenly into 50% Of the figure. 
 Each ideogram will be about **16%** Of the figure.
#### chromosomes_units
The chromosomes_unit value is used as a unit (suffix "u") to shorten
 values In other parts Of the configuration file. Some parameters,
 such As ideogram And tick spacing, accept "u" suffixes, so instead Of

 ```
 spacing = 10000000
 ```
 
 you can write
 
 ```
 spacing = 10u
 ```
#### karyotype
The basically genome structure plots: Chromosome name, size and color definition.(基本的数据文件)
#### Plots
内部元素是有顺序的区别的
#### Size
The genome size.(基因组的大小，当@"P:SMRUCC.genomics.Visualize.Circos.Configurations.Circos.SkeletonKaryotype"为空值的时候返回数值0)
#### SkeletonKaryotype
基因组的骨架信息
