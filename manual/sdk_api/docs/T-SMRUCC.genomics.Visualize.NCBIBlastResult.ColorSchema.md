---
title: ColorSchema
---

# ColorSchema
_namespace: [SMRUCC.genomics.Visualize.NCBIBlastResult](N-SMRUCC.genomics.Visualize.NCBIBlastResult.html)_

Blast结果之中的hit对象的颜色映射



### Methods

#### GetBlastnIdentitiesColor
```csharp
SMRUCC.genomics.Visualize.NCBIBlastResult.ColorSchema.GetBlastnIdentitiesColor(Microsoft.VisualBasic.ComponentModel.Ranges.RangeList{System.Double,Microsoft.VisualBasic.ComponentModel.TagValue{System.Drawing.Color}},System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|p|p的值在0-100之间|
|Schema|-|


#### GetColor
```csharp
SMRUCC.genomics.Visualize.NCBIBlastResult.ColorSchema.GetColor(Microsoft.VisualBasic.ComponentModel.Ranges.RangeList{System.Double,Microsoft.VisualBasic.ComponentModel.TagValue{System.Drawing.Color}},System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|p|p的值在0-1之间|
|Schema|-|


#### IdentitiesBrush
```csharp
SMRUCC.genomics.Visualize.NCBIBlastResult.ColorSchema.IdentitiesBrush(System.Func{SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.Hit,System.Double})
```


|Parameter Name|Remarks|
|--------------|-------|
|scores|需要从这里得到分数|



