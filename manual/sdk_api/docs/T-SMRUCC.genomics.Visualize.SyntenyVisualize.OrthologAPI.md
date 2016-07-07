---
title: OrthologAPI
---

# OrthologAPI
_namespace: [SMRUCC.genomics.Visualize.SyntenyVisualize](N-SMRUCC.genomics.Visualize.SyntenyVisualize.html)_

直系同源的绘图数据模型



### Methods

#### FromBBH
```csharp
SMRUCC.genomics.Visualize.SyntenyVisualize.OrthologAPI.FromBBH(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BBHIndex},SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,System.Func{SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief,System.Drawing.Color},System.Int32,System.Int32,System.Int32,System.Int32,SMRUCC.genomics.Visualize.SyntenyVisualize.LineStyles)
```
Creates the drawing model from the bbh result.

|Parameter Name|Remarks|
|--------------|-------|
|source|bbh ortholog analysis result|
|query|The genomics context of the query|
|hit|The genomics context of the hit|
|colors|Color profiles, this can be family, COGS, pathways or others|
|h1|-|
|h2|-|
|style|-|
|width|绘图区域的宽度|



