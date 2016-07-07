---
title: ModelAPI
---

# ModelAPI
_namespace: [SMRUCC.genomics.Visualize.SyntenyVisualize](N-SMRUCC.genomics.Visualize.SyntenyVisualize.html)_





### Methods

#### __getName
```csharp
SMRUCC.genomics.Visualize.SyntenyVisualize.ModelAPI.__getName(SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.HitCollection,System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|hits|-|
|hitsTag|-|
|query|基因组标识符|


#### GetDrawsModel
```csharp
SMRUCC.genomics.Visualize.SyntenyVisualize.ModelAPI.GetDrawsModel(System.String,SMRUCC.genomics.Visualize.SyntenyVisualize.LineStyles)
```
Convert data model @"T:SMRUCC.genomics.Visualize.SyntenyVisualize.DeviceModel" to drawing object model @"T:SMRUCC.genomics.Visualize.SyntenyVisualize.DrawingModel"

|Parameter Name|Remarks|
|--------------|-------|
|path|The json file path of the drawing data model @"T:SMRUCC.genomics.Visualize.SyntenyVisualize.DeviceModel"|
|style|The link line style.(假若设置了这个参数的话，就会将模型里面的数据给覆盖掉)|


#### IsOrtholog
```csharp
SMRUCC.genomics.Visualize.SyntenyVisualize.ModelAPI.IsOrtholog(System.String,System.String,SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.HitCollection,System.String)
```
空值表示没有同源关系

|Parameter Name|Remarks|
|--------------|-------|
|query|基因组标识符|
|hit|基因组标识符|
|hits|-|
|hitsTag|基因组标识符|



