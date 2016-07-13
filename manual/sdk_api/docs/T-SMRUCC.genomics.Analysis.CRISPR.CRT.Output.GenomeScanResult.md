---
title: GenomeScanResult
---

# GenomeScanResult
_namespace: [SMRUCC.genomics.Analysis.CRISPR.CRT.Output](N-SMRUCC.genomics.Analysis.CRISPR.CRT.Output.html)_

CRISPR位点的基因组搜索的结果，可以使用这个对象将CRISPR的结果保存为XML格式的结果文件，最后通过xlst将结果以html的形式格式化显示出来



### Methods

#### ExportFasta
```csharp
SMRUCC.genomics.Analysis.CRISPR.CRT.Output.GenomeScanResult.ExportFasta
```
导出每一个位点之间的重复片段的序列

#### ExportSpacerFasta
```csharp
SMRUCC.genomics.Analysis.CRISPR.CRT.Output.GenomeScanResult.ExportSpacerFasta
```
导出每一个位点之中的重复片段之间的间隔序列


### Properties

#### Tag
可以使用本标签信息进行基因组的LocusID的信息的存储
