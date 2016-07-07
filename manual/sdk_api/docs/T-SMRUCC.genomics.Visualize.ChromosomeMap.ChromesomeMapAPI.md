---
title: ChromesomeMapAPI
---

# ChromesomeMapAPI
_namespace: [SMRUCC.genomics.Visualize.ChromosomeMap](N-SMRUCC.genomics.Visualize.ChromosomeMap.html)_

This module contains the required API function for create the chromosomes map of a specific bacteria genome.



### Methods

#### AddMotifSites
```csharp
SMRUCC.genomics.Visualize.ChromosomeMap.ChromesomeMapAPI.AddMotifSites(SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels.ChromesomeDrawingModel,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat.VirtualFootprints})
```


|Parameter Name|Remarks|
|--------------|-------|
|model|-|
|data|可以使用@"M:SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.MotifFootPrintsGenerates.GroupMotifs(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint},SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid,System.Int32)"方法来合并一些重复的motif数据|


#### AddTSSs
```csharp
SMRUCC.genomics.Visualize.ChromosomeMap.ChromesomeMapAPI.AddTSSs(SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels.ChromesomeDrawingModel,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.Transcript})
```
将TSS位点以Motif位点的形式添加到绘图模型之上

|Parameter Name|Remarks|
|--------------|-------|
|Model|-|
|Sites|-|


#### ApplyCogColorProfile
```csharp
SMRUCC.genomics.Visualize.ChromosomeMap.ChromesomeMapAPI.ApplyCogColorProfile(SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels.ChromesomeDrawingModel,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST.MyvaCOG})
```
按照COG分类来赋值COG颜色的

|Parameter Name|Remarks|
|--------------|-------|
|Model|-|
|MyvaCOG|-|


#### CreateDevice
```csharp
SMRUCC.genomics.Visualize.ChromosomeMap.ChromesomeMapAPI.CreateDevice(SMRUCC.genomics.Visualize.ChromosomeMap.Configurations)
```
请注意，在宽度上面是4倍的Margin

|Parameter Name|Remarks|
|--------------|-------|
|Config|-|


#### FromPttElements
```csharp
SMRUCC.genomics.Visualize.ChromosomeMap.ChromesomeMapAPI.FromPttElements(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief},SMRUCC.genomics.Visualize.ChromosomeMap.Configurations,System.Int32)
```
通常使用这个方法从PTT构件之中生成部分基因组的绘制模型数据

|Parameter Name|Remarks|
|--------------|-------|
|PTTGeneObjects|-|
|conf|-|


#### LoadConfig
```csharp
SMRUCC.genomics.Visualize.ChromosomeMap.ChromesomeMapAPI.LoadConfig(System.String)
```
使用这个函数进行绘图设备的配置参数的读取操作

|Parameter Name|Remarks|
|--------------|-------|
|file|-|


#### SaveImage
```csharp
SMRUCC.genomics.Visualize.ChromosomeMap.ChromesomeMapAPI.SaveImage(System.Collections.Generic.KeyValuePair{System.Drawing.Imaging.ImageFormat,System.Drawing.Bitmap[]},System.String,System.String)
```
Image formats can be one of the value: jpg,bmp,emf,exif,gif,png,wmf,tiff

|Parameter Name|Remarks|
|--------------|-------|
|res|@"M:SMRUCC.genomics.Visualize.ChromosomeMap.DrawingDevice.InvokeDrawing(SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels.ChromesomeDrawingModel)"函数所生成的绘图图形输出资源数据|
|Export|将要进行数据保存的文件夹|
|Format|Value variant in jpg,bmp,emf,exif,gif,png,wmf,tiff|



