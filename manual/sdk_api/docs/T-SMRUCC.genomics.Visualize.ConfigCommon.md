---
title: ConfigCommon
---

# ConfigCommon
_namespace: [SMRUCC.genomics.Visualize](N-SMRUCC.genomics.Visualize.html)_






### Properties

#### Resolution
Due to the GDI+ limitations in the .NET Framework, the image size is limited by your computer memory size, if you want to
 drawing a very large size image, please running this script on a 64bit platform operating system, or you will get a 
 exception about the GDI+ error: parameter is not valid and then you should try a smaller resolution of the drawing output image.
 Value format: <Width(Integer)>[,<Height(Integer)>]
 
 Example:
 Both specific the size property: 12000,8000
 Which means the drawing script will generate a image file in resolution of width is value 12000 pixels and image height is 8000 pixels.
