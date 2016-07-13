---
title: LogAnalysis
---

# LogAnalysis
_namespace: [SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI](N-SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI.html)_

BLAST日志分析模块

> This module its code is too old, obsolete!


### Methods

#### Merge
```csharp
SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI.LogAnalysis.Merge(System.Collections.Generic.IEnumerable{Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File})
```
将多个分析出来的最佳匹配的文件合并成一个文件，这个所得到的文件将会用于文氏图的绘制

|Parameter Name|Remarks|
|--------------|-------|
|dataset|-|


#### TakeBestHits
```csharp
SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI.LogAnalysis.TakeBestHits(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.Standard.BLASTOutput,SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.Standard.BLASTOutput)
```
从已经分析好的日志文件之中生成最佳匹配表

|Parameter Name|Remarks|
|--------------|-------|
|query|-|
|subject|-|



