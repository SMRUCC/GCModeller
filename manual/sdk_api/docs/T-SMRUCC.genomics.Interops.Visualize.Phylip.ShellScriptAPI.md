---
title: ShellScriptAPI
---

# ShellScriptAPI
_namespace: [SMRUCC.genomics.Interops.Visualize.Phylip](N-SMRUCC.genomics.Interops.Visualize.Phylip.html)_





### Methods

#### __exportMatrix
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.ShellScriptAPI.__exportMatrix(SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BestHit)
```
矩阵文件的格式要求为：
 行的标题（每一行中的第一个元素）为基因组的名称
 每一列为某一个基因的频率或者其他数值
 例如：
 基因1，基因2，基因3， ...
 基因组1 1 1 0
 基因组2 2 1 4

|Parameter Name|Remarks|
|--------------|-------|
|besthit|-|


#### CreateNodeLabelAnnotation
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.ShellScriptAPI.CreateNodeLabelAnnotation(System.String,System.String,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.gbEntryBrief})
```
{Trimmed_ID, uid}

#### ExportGendistMatrixFromBesthitMeta
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.ShellScriptAPI.ExportGendistMatrixFromBesthitMeta(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BestHit},System.String,System.Boolean,System.Int32)
```
直接使用identities值作为最开始的等位基因的频率值

|Parameter Name|Remarks|
|--------------|-------|
|MetaSource|-|
|Limits|0或者小于零的数值都为不限制,假设做出数量的限制的话，函数只会提取指定数目的基因组数据，都是和外标尺最接近的基因组|


#### LoadHitsVennData
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.ShellScriptAPI.LoadHitsVennData(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|source|Xml数据的文件夹|


#### NeighborMatrixFromVennMatrix
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.ShellScriptAPI.NeighborMatrixFromVennMatrix(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
```
从已经生成的韦恩矩阵之中生成距离矩阵

#### SelfOverviewsMAT
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.ShellScriptAPI.SelfOverviewsMAT(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.Views.Overview)
```
不可以使用并行化拓展，因为有一一对应关系

|Parameter Name|Remarks|
|--------------|-------|
|overview|-|



