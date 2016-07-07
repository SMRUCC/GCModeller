---
title: VennDiagram
---

# VennDiagram
_namespace: [RDotNet.Extensions.Bioinformatics.VennDiagram.ModelAPI](N-RDotNet.Extensions.Bioinformatics.VennDiagram.ModelAPI.html)_

The data model of the venn diagram.(文氏图的数据模型)



### Methods

#### __R_script
```csharp
RDotNet.Extensions.Bioinformatics.VennDiagram.ModelAPI.VennDiagram.__R_script
```
Convert the data model as the R script for venn diagram drawing.(将本数据模型对象转换为R脚本)

#### op_Addition
```csharp
RDotNet.Extensions.Bioinformatics.VennDiagram.ModelAPI.VennDiagram.op_Addition(RDotNet.Extensions.Bioinformatics.VennDiagram.ModelAPI.VennDiagram,System.Collections.Generic.IEnumerable{System.String[]})
```
Applying the diagram options

|Parameter Name|Remarks|
|--------------|-------|
|venn|-|
|opts|-|


#### RandomColors
```csharp
RDotNet.Extensions.Bioinformatics.VennDiagram.ModelAPI.VennDiagram.RandomColors
```
Assign random colors to the venn diagram partitions


### Properties

#### categoryNames
The partition names
#### partitions
Partitions on the venn diagram
#### plot
The venn.diagram plot API in R language
#### saveTiff
vennDiagram tiff file saved path.(所生成的文氏图的保存文件名)
#### Title
The title of the diagram.
