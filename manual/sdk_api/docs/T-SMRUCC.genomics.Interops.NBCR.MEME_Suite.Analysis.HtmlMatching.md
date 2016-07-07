---
title: HtmlMatching
---

# HtmlMatching
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.html)_





### Methods

#### __match
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.HtmlMatching.__match(SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel.MotifSite,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief,System.Int32)
```
从位点上面的相对位置得到基因组上面的绝对位置

|Parameter Name|Remarks|
|--------------|-------|
|site|-|
|gene|-|
|Length|-|


#### FilteringPcc
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.HtmlMatching.FilteringPcc(SMRUCC.genomics.Interops.NBCR.MEME_Suite.MatchedResult[],System.Double)
```
不适用的WGCNA权重进行筛选的原因是WGCNA不能够表示出负调控关系

|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|pccCutoff|-|


#### Invoke
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.HtmlMatching.Invoke(System.String,System.String,System.String,System.String,System.String,System.String)
```
导出所分析出的所有数据，不加任何筛选操作

|Parameter Name|Remarks|
|--------------|-------|
|MEME_out|-|
|MAST_out|-|
|FastaFileDir|-|
|bh|-|
|Door|-|


#### Match
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.HtmlMatching.Match(System.String,System.String,System.String,System.String)
```
**MastSourceDir**之中的文件夹名称应该和**GbkSourceDir**之中的文件名是一一对应的

|Parameter Name|Remarks|
|--------------|-------|
|MEME_Xml|-|
|MastSourceDir|-|
|export|-|


#### NovelSites
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.HtmlMatching.NovelSites(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.LDM.Motif},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel.MotifSite})
```
使用函数@"M:SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.HtmlMatching.Match(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.LDM.Motif},SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.XmlOutput.MAST.MAST)"得到匹配的位点之后可以使用这个函数得到未被匹配到的可能的新的位点

|Parameter Name|Remarks|
|--------------|-------|
|Motifs|-|
|matches|-|



