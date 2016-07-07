---
title: Conf
---

# Conf
_namespace: [SMRUCC.genomics.Visualize.ChromosomeMap](N-SMRUCC.genomics.Visualize.ChromosomeMap.html)_

配置数据的数据模型，请使用@"M:SMRUCC.genomics.Visualize.ChromosomeMap.Configurations.ToConfigurationModel"方法来初始化本数据模型对象，真正所被使用到的内存之中的配置文件的对象模型




### Properties

#### LineLength
The chromosome segment length of each line on the chromosome map, this property can effects the drawing model scale factor.
 (在ChromosomeMap之中的每一行基因组片段的长度值，通过这个属性可以影响图形的缩放大小)
#### Resolution
This section will configure the drawing size options
 ----------------------------------------------------
 Due to the GDI+ limitations in the .NET Framework, the image size is limited by your computer memory size, if you want to 
 drawing a very large size image, please running this script on a 64bit platform operating system, or you will get a 
 exception about the GDI+ error: parameter is not valid and then you should try a smaller resolution of the drawing output image.
 Value format: <Width(Integer)>[,<Height(Integer)>]
 Example:
 Both specific the size property: 12000,8000
 Which means the drawing script will generate a image file in resolution of width is value 12000 pixels and image height 
 is 8000 pixels.
