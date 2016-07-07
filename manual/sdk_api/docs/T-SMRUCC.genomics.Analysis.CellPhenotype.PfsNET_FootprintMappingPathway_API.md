---
title: PfsNET_FootprintMappingPathway_API
---

# PfsNET_FootprintMappingPathway_API
_namespace: [SMRUCC.genomics.Analysis.CellPhenotype](N-SMRUCC.genomics.Analysis.CellPhenotype.html)_





### Methods

#### __createNetwork
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.PfsNET_FootprintMappingPathway_API.__createNetwork(SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO.XmlresxLoader)
```
除了KEGG pathway的信息，还会在这里面包含有完整的细胞网络之中的调控信息和互作信息

|Parameter Name|Remarks|
|--------------|-------|
|Model|-|


#### AnalysisFootprintPathway
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.PfsNET_FootprintMappingPathway_API.AnalysisFootprintPathway(SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO.XmlresxLoader,SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.ExprSamples[][],SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.ExprSamples[][],System.String,System.String,System.String,System.String)
```
footprint途径是经过比较野生型和突变体在同一个滑窗的时间段之内的表达差异而得到的

|Parameter Name|Remarks|
|--------------|-------|
|WT|必须是经过@"M:SMRUCC.genomics.Analysis.CellPhenotype.PfsNET_FootprintMappingPathway_API.CreateExpressionWindows(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,System.Int32,System.Int32)"方法所创建出来的滑窗数据|
|netModel|用于生成文件三|
|Mutation|必须是经过@"M:SMRUCC.genomics.Analysis.CellPhenotype.PfsNET_FootprintMappingPathway_API.CreateExpressionWindows(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,System.Int32,System.Int32)"方法所创建出来的滑窗数据|


#### CreateExpressionWindows
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.PfsNET_FootprintMappingPathway_API.CreateExpressionWindows(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,System.Int32,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|chipdata|-|
|windowSize|-|
|offset|大于0的一个数|


#### GenomeProgrammingAnalysis
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.PfsNET_FootprintMappingPathway_API.GenomeProgrammingAnalysis(SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT.ExprSamples[][],SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO.XmlresxLoader,System.String,System.String,System.String,System.String,System.String,System.String,System.Int32)
```
基因组的编程信息则是比较同一个细胞之内的不同滑窗时间点之间的表达差异性而得到的，在计算出差异性之后在比较野生型和突变体之间的差异既可以得到基因组因为motif突变而产生的重编程的现象了

|Parameter Name|Remarks|
|--------------|-------|
|chipdata|-|
|NetworkModel|-|



### Properties

#### PID
Current process id to avoid the disruption of the PfsNET calculation data.
