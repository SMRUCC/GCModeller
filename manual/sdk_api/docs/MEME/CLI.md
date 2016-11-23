# CLI
_namespace: [MEME](./index.md)_





### Methods

#### #cctor
```csharp
MEME.CLI.#cctor
```
初始化应用程序模块的时候自动执行初始化代码

#### __buildRegulates
```csharp
MEME.CLI.__buildRegulates(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.AnnotationModel,SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.AnnotationModel,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,SMRUCC.genomics.Analysis.RNA_Seq.ICorrelations,System.Collections.Generic.Dictionary{System.String,System.String[]})
```


|Parameter Name|Remarks|
|--------------|-------|
|query|-|
|subject|-|
|correlates|-|
|mapsRegulates|subjects -> uid|


#### __loadMEME
```csharp
MEME.CLI.__loadMEME(System.String,System.String,System.Collections.Generic.Dictionary{System.String,System.String},System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|ID|-|
|memeFile|-|
|source|Fasta source|
|LocusFromFasta|-|


#### __siteMatchesCommon
```csharp
MEME.CLI.__siteMatchesCommon(System.Collections.Generic.Dictionary{System.String,System.String},System.Collections.Generic.Dictionary{System.String,System.String},System.String,System.String,System.String)
```
利用meme得到的motif数据，不是从数据库之中匹配出来的数据

|Parameter Name|Remarks|
|--------------|-------|
|MEMESets|-|
|MastSets|-|
|out|-|
|PTTFile|-|
|FastaDir|-|


#### __siteToFootprint
```csharp
MEME.CLI.__siteToFootprint(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.Site,System.String,System.String,SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.AnnotationModel,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT)
```
基本的位点信息

|Parameter Name|Remarks|
|--------------|-------|
|site|-|
|subject|-|
|PTT|-|


#### __siteToRegulation
```csharp
MEME.CLI.__siteToRegulation(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint,System.String,SMRUCC.genomics.Analysis.RNA_Seq.ICorrelations)
```


|Parameter Name|Remarks|
|--------------|-------|
|site|copy|
|TF|-|
|correlations|-|


#### Build
```csharp
MEME.CLI.Build(Microsoft.VisualBasic.CommandLine.CommandLine)
```
从mast sites之中得到调控信息

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### CompileMast
```csharp
MEME.CLI.CompileMast(Microsoft.VisualBasic.CommandLine.CommandLine)
```
对单个的mast文档进行数据导出

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### CompileMastBuck
```csharp
MEME.CLI.CompileMastBuck(Microsoft.VisualBasic.CommandLine.CommandLine)
```
批量汇编mast结果，导出调控位点的信息

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### CORN
```csharp
MEME.CLI.CORN(Microsoft.VisualBasic.CommandLine.CommandLine)
```
Cluster of co-regulated orthologous operons

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### DiffHits
```csharp
MEME.CLI.DiffHits(Microsoft.VisualBasic.CommandLine.CommandLine)
```
求query - subject得到的差集

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### DownloadRegprecise
```csharp
MEME.CLI.DownloadRegprecise(Microsoft.VisualBasic.CommandLine.CommandLine)
```
下载数据库

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### Expand
```csharp
MEME.CLI.Expand(System.Collections.Generic.IEnumerable{SMRUCC.genomics.SequenceModel.NucleotideModels.SimpleSegment},System.String)
```
拓展简单位点的信息为Motif位点信息

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|motifs|MotifSiteLog.Xml 文件夹|


#### ExportMotifDraw
```csharp
MEME.CLI.ExportMotifDraw(Microsoft.VisualBasic.CommandLine.CommandLine)
```
生成论文表格

#### HitContext
```csharp
MEME.CLI.HitContext(Microsoft.VisualBasic.CommandLine.CommandLine)
```
2

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### KEGGFamilyDump
```csharp
MEME.CLI.KEGGFamilyDump(Microsoft.VisualBasic.CommandLine.CommandLine)
```
Regprecise之中的家族注释好像有些是错误的，使用这个来从KEGG数据库之中推测家族

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### LDMMaxLen
```csharp
MEME.CLI.LDMMaxLen(Microsoft.VisualBasic.CommandLine.CommandLine)
```
可以根据这个来设置meme的maxw参数，因为tomquery里面的相似度的结果是和长度相关的：coverage

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### LoadSimilarityHits
```csharp
MEME.CLI.LoadSimilarityHits(Microsoft.VisualBasic.CommandLine.CommandLine)
```
导出通过MAST程序锁分析出来的Motif之间相似度的结果
 文件夹的组织结构是Motif.uid -> Motifs

#### MEMEPlantSimilarity
```csharp
MEME.CLI.MEMEPlantSimilarity(Microsoft.VisualBasic.CommandLine.CommandLine)
```
计算出Motif的相似度，然后方便分组归纳新的Motif数据，这里只是计算出一个模块的

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### MEMETOM_MotifSimilarity
```csharp
MEME.CLI.MEMETOM_MotifSimilarity(Microsoft.VisualBasic.CommandLine.CommandLine)
```
导出tomtom程序的分析结果

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### ModuleRegulates
```csharp
MEME.CLI.ModuleRegulates(Microsoft.VisualBasic.CommandLine.CommandLine)
```
需要事先已经填上了代谢途径的信息在里面

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### MotifMatch
```csharp
MEME.CLI.MotifMatch(Microsoft.VisualBasic.CommandLine.CommandLine)
```
1

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### MotifScan
```csharp
MEME.CLI.MotifScan(Microsoft.VisualBasic.CommandLine.CommandLine)
```
使用片段相似性来扫描Motif位点

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### ParserRegPreciseOperon
```csharp
MEME.CLI.ParserRegPreciseOperon(Microsoft.VisualBasic.CommandLine.CommandLine)
```
这个函数里面默认是按照TF进行分组输出的，假若需要做操纵子的分析，可以添加/corn标记

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### RegulatorMotifs
```csharp
MEME.CLI.RegulatorMotifs(Microsoft.VisualBasic.CommandLine.CommandLine)
```
导出由bbh结果所得到的motif信息

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### RegulatorsBBh
```csharp
MEME.CLI.RegulatorsBBh(Microsoft.VisualBasic.CommandLine.CommandLine)
```
联系需要注释的蛋白质在Regprecise数据库之中的信息

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### RegulonReconstructs
```csharp
MEME.CLI.RegulonReconstructs(Microsoft.VisualBasic.CommandLine.CommandLine)
```
进行Regulon的批量重建工作

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### RegulonReconstructs2
```csharp
MEME.CLI.RegulonReconstructs2(Microsoft.VisualBasic.CommandLine.CommandLine)
```
其实bbh参数的数据类型不一定必须要严格满足bbh，只需要同时具备有query_name和hit_name就可以了

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### SiteMappedBack
```csharp
MEME.CLI.SiteMappedBack(Microsoft.VisualBasic.CommandLine.CommandLine)
```
从当前的基因组之中利用规则得到一些可能的新的Motif之后再使用那个得到的新Motif比对回基因组，发现潜在的调控的联系
 和@``M:MEME.CLI.CompileMast(Microsoft.VisualBasic.CommandLine.CommandLine)``函数所不同的是meme的数据源不同，仅此而已

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### SiteMatches
```csharp
MEME.CLI.SiteMatches(Microsoft.VisualBasic.CommandLine.CommandLine)
```
这个函数是和regprecise的匹配结果

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### SiteRegexScan
```csharp
MEME.CLI.SiteRegexScan(Microsoft.VisualBasic.CommandLine.CommandLine)
```
使用正则表达式来扫描可能的位点

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### SWTomQueryBatch
```csharp
MEME.CLI.SWTomQueryBatch(Microsoft.VisualBasic.CommandLine.CommandLine)
```
bits.level越低则条件越苛刻

|Parameter Name|Remarks|
|--------------|-------|
|args|-|

> 默认参数已经是经过RegulatorsMotif测试过的

#### ToFootprints
```csharp
MEME.CLI.ToFootprints(Microsoft.VisualBasic.CommandLine.CommandLine)
```
3

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### TomQuery
```csharp
MEME.CLI.TomQuery(Microsoft.VisualBasic.CommandLine.CommandLine)
```
对meme的分析结果判断是哪一个motif

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### UnionSimilarity
```csharp
MEME.CLI.UnionSimilarity(Microsoft.VisualBasic.CommandLine.CommandLine)
```
合并相似的Motif进入下一次迭代


